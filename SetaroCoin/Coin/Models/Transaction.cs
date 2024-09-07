using System.Security.Cryptography;
using System.Text;
using SetaroCoin.Coin.Extensions;
using SetaroCoin.Wallet;

namespace SetaroCoin.Coin.Models;

public class Transaction
{
    /// <summary>
    /// A hash of the transaction.
    /// </summary>
    public byte[] Hash => this.ToBytes();

    /// <summary>
    /// A Guid for this transaction
    /// </summary>
    public Guid TransactionId { get; } = Guid.NewGuid();

    /// <summary>
    /// Wallet of the sender.
    /// </summary>
    public required UserWallet Sender { get; init; }

    /// <summary>
    /// Wallet of the recipient.
    /// </summary>
    public required UserWallet? Recipient { get; init; }

    /// <summary>
    /// The amount being transacted between sender and recipient.
    /// </summary>
    public required float Amount { get; init; }

    /// <summary>
    /// The digital signature from the sender for this transaction.
    /// </summary>
    public required byte[] SenderSignature { get; init; }

    /// <summary>
    /// The amount being charged as a network fee.
    /// </summary>
    public float Fee { get; set; }

    /// <summary>
    /// If the transaction has been added to a mined block. i.e has been confirmed.
    /// </summary>
    public bool IsConfirmed { get; private set; }

    /// <summary>
    /// The block(s) this transaction was confirmed in.
    /// </summary>
    public List<Block> ConfirmedInBlocks { get; } = [];


    /// <summary>
    /// Waits for confirmation of completion.
    /// </summary>
    private readonly SemaphoreSlim _confirmation = new(1);
    
    
    /// <summary>
    /// Validates the entire transaction.
    /// </summary>
    /// <returns>True if the transaction is valid. False if it's invalid.</returns>
    public bool Validate()
    {
        var result = false;
        
        // Check the signature is legit
        result &= VerifySignature();
        
        // Check the sender address exists in the blockchain
        Blockchain.PublicLedger.Instance.TryGetValue(Sender.Address, out var sender);
        result &= sender != null;
        
        // Check the recipient address exists in the blockchain
        Blockchain.PublicLedger.Instance.TryGetValue(Recipient?.Address ?? "", out var recipient);
        result &= recipient != null;
        
        // Check the sender actually has the tx amount in their wallet
        result &= sender?.Balance >= Amount;
        
        return result;
    }

    /// <summary>
    /// Validate just the signature of the transaction.
    /// </summary>
    /// <returns>True if the signature is valid.</returns>
    private bool VerifySignature()
    {
        // Create RSA public key from sender wallet address (public key)
        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(Sender.Address), out _);

        var addressBytes = Encoding.UTF8.GetBytes(Sender.Address);
        var amountBytes = BitConverter.GetBytes(Amount);
        
        return rsa.VerifyData(
                    addressBytes.Concat(amountBytes).ToArray(), 
                    SenderSignature,
                    HashAlgorithmName.SHA256, 
                    RSASignaturePadding.Pkcs1);
    }

    internal void OnTransactionConfirmed()
    {
        IsConfirmed = true;
        _confirmation.Release();
        
        // Update balances for sender and recipient
        Sender.Balance -= Amount;
        Sender.Balance -= Blockchain.TransactionFee;
        
        Recipient.Balance += Amount - Blockchain.TransactionFee;
    }

    internal void OnTransactionFailed()
    {
        IsConfirmed = false;
        _confirmation.Release();
    }

    public async Task<bool> AwaitConfirmation()
    {
        await _confirmation.WaitAsync();
        return IsConfirmed;
    }
}
