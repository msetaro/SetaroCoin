using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
    /// Public and private key pair of this wallet. This is used to sign transactions.
    /// </summary>
    private readonly RSA _keyPair = RSA.Create();
    
    /// <summary>
    /// Balance of SetaroCoins in this wallet.
    /// </summary>
    public float Balance { get; set; }

    /// <summary>
    /// Send SetaroCoins to another wallet.
    /// </summary>
    /// <param name="address">The address of the wallet to send to.</param>
    /// <param name="amount">The amount of coins to send to that wallet.</param>
    public async Task<bool> Send(string address, float amount)
    {
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
        return await transaction.AwaitConfirmation();
    }

    /// <summary>
    /// Sign a transaction to confirm the identity of the sender.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="amount"></param>
    private byte[] SignTransaction(string address, float amount)
    {
        byte[] addressBytes = Encoding.UTF8.GetBytes(address);
        byte[] amountBytes = BitConverter.GetBytes(amount);
        
        return _keyPair.SignData([.. addressBytes, .. amountBytes], HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}

