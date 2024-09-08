using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SetaroCoin.Coin.Enum;
using SetaroCoin.Coin.Extensions;
using SetaroCoin.Coin.Models;
using SetaroCoin.Network;

namespace SetaroCoin.Wallet;

public class UserWallet
{
    /// <summary>
    /// Address of this wallet. This is the address that will be used to receive and send coins.
    /// </summary>
    public string Address => 
        BitConverter.ToString(_keyPair.ExportSubjectPublicKeyInfo());
    
    /// <summary>
    /// User's public key in bytes.
    /// </summary>
    public byte[] AddressBytes => _keyPair.ExportSubjectPublicKeyInfo();

    /// <summary>
    /// Public and private key pair of this wallet. This is used to sign transactions.
    /// </summary>
    private readonly RSA _keyPair = RSA.Create();
    
    /// <summary>
    /// Balance of SetaroCoins in this wallet.
    /// </summary>
    public float Balance { get; set; }

    public UserWallet()
    {
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
        Console.WriteLine($"Wallet address {Address[..7]}... initiated send to {address[..7]}... of {amount} SC");
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
        byte[] recipientAddressBytes = Encoding.UTF8.GetBytes(recipientAddress);
        byte[] senderAddressBytes = Encoding.UTF8.GetBytes(Address);
        byte[] amountBytes = BitConverter.GetBytes(amount);

        byte[] data = [.. senderAddressBytes, .. recipientAddressBytes, .. amountBytes];
        return _keyPair.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}

