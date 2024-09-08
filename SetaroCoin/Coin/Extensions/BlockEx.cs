using System.Security.Cryptography;
using SetaroCoin.Coin.Models;
using SetaroCoin.Coin.Services;

namespace SetaroCoin.Coin.Extensions;

public static class BlockEx
{
    internal static bool AssertNonce(this Block block)
    {
        byte[] hash = BlockFactory.HashBlock(block);

        bool assert = hash[..Blockchain.NumberOfLeadingZeroes]
            .SequenceEqual(new byte[Blockchain.NumberOfLeadingZeroes]);
        
        Console.WriteLine($"Assert Nonce -- IsValid: {assert} | Hash: {string.Join(',', hash[..10])}...");
        
        return assert;
    }

    internal static void ConfirmAllTransactions(this Block block)
    {
        foreach (var transaction in block.Transactions)
        {
            transaction.OnTransactionConfirmed();
            transaction.ConfirmedInBlocks.Add(block);
        }
    }
}

