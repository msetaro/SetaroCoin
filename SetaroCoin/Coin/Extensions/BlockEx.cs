using System.Security.Cryptography;
using SetaroCoin.Coin.Models;
using SetaroCoin.Coin.Services;

namespace SetaroCoin.Coin.Extensions;

public static class BlockEx
{
    internal static bool AssertNonce(this Block block)
    {
        byte[] nonceBytes = BitConverter.GetBytes(block.Nonce);
        byte[] merkleRoot = MerkleRootService.FindMerkleRoot(block.Transactions);
        byte[] wholeHash = [.. nonceBytes, .. merkleRoot, .. block.PreviousHash];
        byte[] hash = SHA256.HashData(wholeHash);

        return hash[..Blockchain.NumberOfLeadingZeroes].SequenceEqual(new byte[Blockchain.NumberOfLeadingZeroes]);
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

