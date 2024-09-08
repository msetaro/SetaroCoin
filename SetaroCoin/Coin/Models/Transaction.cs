using System.Security.Cryptography;
using System.Text;
using SetaroCoin.Coin.Enum;
using SetaroCoin.Coin.Extensions;
using SetaroCoin.Network;
using SetaroCoin.Wallet;

namespace SetaroCoin.Coin.Models;

public class Transaction
{
    /// <summary>
    /// A hash of the transaction.
    /// </summary>
    public byte[] Hash { get; }
    
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
    /// The time in which the transaction was created.
    /// </summary>
    public DateTime TimeOfCreation { get; init; }
    
    /// <summary>
    /// Waits for confirmation of completion.
    /// </summary>
    private readonly TaskCompletionSource<TransactionStatus> _confirmation = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Transaction()
    {
        Hash = this.ToBytes();
        TimeOfCreation = DateTime.Now;
    }
    
    /// <summary>
    /// Validates the entire transaction.
    /// </summary>
    /// <returns>True if the transaction is valid. False if it's invalid.</returns>
    public TransactionStatus Validate()
    {
        // Check the signature is legit
        if (!VerifySignature()) return TransactionStatus.InvalidSignature;
        
        // Check the sender address exists in the blockchain
        Blockchain.PublicLedger.Instance.TryGetValue(Sender.Address, out var sender);
        if(sender == null) return TransactionStatus.InvalidSenderAddress;
        
        // Check the recipient address exists in the blockchain
        Blockchain.PublicLedger.Instance.TryGetValue(Recipient?.Address ?? "", out var recipient);
        if (recipient == null) return TransactionStatus.InvalidRecipientAddress;

        // Check the sender actually has the tx amount in their wallet
        if (sender.Balance >= Amount) return TransactionStatus.InsufficientFunds;
        
        return TransactionStatus.Success;
    }

    /// <summary>
    /// Validate just the signature of the transaction.
    /// </summary>
    /// <returns>True if the signature is valid.</returns>
    private bool VerifySignature()
    {
        // Create RSA public key from sender wallet address (public key)
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Sender.AddressBytes, out _);
        
        var senderAddressBytes = Sender.AddressBytes;
        var recipientAddressBytes = Recipient?.AddressBytes;
        var amountBytes = BitConverter.GetBytes(Amount);
        
        byte[] data = [.. senderAddressBytes, .. recipientAddressBytes, .. amountBytes];
        
        return rsa.VerifyData(
                    data,
                    SenderSignature,
                    HashAlgorithmName.SHA256, 
                    RSASignaturePadding.Pkcs1);
    }

    internal void OnTransactionConfirmed()
    {
        IsConfirmed = true;
        _confirmation.TrySetResult(TransactionStatus.Success);
        
        // Update balances for sender and recipient
        Sender.Balance -= Amount;
        Sender.Balance -= Blockchain.TransactionFee;

        Fee = Blockchain.TransactionFee;
        
        Recipient.Balance += Amount - Blockchain.TransactionFee;
        
        // Remove itself from the mempool
        Mempool.RemoveTransaction(this);

        Console.WriteLine($"Transaction confirmed with ID: {TransactionId}");
    }

    internal void OnTransactionFailed(TransactionStatus status)
    {
        IsConfirmed = false;
        _confirmation.TrySetResult(status);

        Console.WriteLine($"Transaction failed with ID: {TransactionId} | Reason: {status}");
    }

    public async Task<TransactionStatus> AwaitConfirmation()
    {
        return await _confirmation.Task;
    }
}
