using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using SetaroCoin.Coin.Enum;
using SetaroCoin.Coin.Extensions;
using SetaroCoin.Coin.Models;
using SetaroCoin.Network;

namespace SetaroCoin.Wallet;

public class UserWallet
{
    /// <summary>
    /// The algorithm used for transaction signatures.
    /// </summary>
    public const string DigitalSignatureAlgorithm = "SHA256withRSA";
    
    /// <summary>
    /// Address of this wallet. This is the address that will be used to receive and send coins.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Public and private key pair of this wallet. This is used to sign transactions.
    /// </summary>
    private readonly AsymmetricCipherKeyPair _keyPair;
    
    /// <summary>
    /// Balance of SetaroCoins in this wallet.
    /// </summary>
    public float Balance { get; set; }

    public UserWallet()
    {
        // Set up key pair
        var gen = new RsaKeyPairGenerator();
        gen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        _keyPair = gen.GenerateKeyPair();

        // Set up address
        Address = Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_keyPair.Public).GetDerEncoded());
        
        Blockchain.AddNewWallet(this);
    }

    /// <summary>
    /// Send SetaroCoins to another wallet.
    /// </summary>
    /// <param name="address">The address of the wallet to send to.</param>
    /// <param name="amount">The amount of coins to send to that wallet.</param>
    public async Task<TransactionStatus> Send(string address, float amount)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Wallet address {Address[^10..]}... initiated send to {address[^10..]}... of {amount} SC");
        Console.ResetColor();
        
        // Get the user wallet instance of the recipient
        Blockchain.PublicLedger.Instance.TryGetValue(address, out var recipient);
        
        // Create transaction
        var transaction = new Transaction()
        {
            Amount = amount,
            Sender = this,
            Recipient = recipient,
            SenderSignature = SignTransaction(address, amount)
        };
        
        // Send transaction to be processed by the network
        Mempool.AddTransaction(transaction);
        
        // Wait for the transaction to complete
        var res = await transaction.AwaitConfirmation();
        if (res != TransactionStatus.Success) Console.WriteLine($"Transaction failed | Reason: {res}");

        return res;
    }

    /// <summary>
    /// Sign a transaction to confirm the identity of the sender.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="amount"></param>
    private byte[] SignTransaction(string recipientAddress, float amount)
    {
        byte[] recipientAddressBytes = Convert.FromBase64String(recipientAddress);
        byte[] senderAddressBytes = Convert.FromBase64String(Address);
        byte[] amountBytes = BitConverter.GetBytes(amount);

        byte[] data = [.. senderAddressBytes, .. recipientAddressBytes, .. amountBytes];
        
        // BC signature
        var signer = SignerUtilities.GetSigner(DigitalSignatureAlgorithm);
        signer.Init(true, _keyPair.Private);
        signer.BlockUpdate(data, 0, data.Length);
        
        return signer.GenerateSignature();
    }
}

