using System.Security.Cryptography;
using SetaroCoin.Coin.Models;

namespace SetaroCoin.Coin.Services;

public static class BlockFactory
{
    private static readonly byte[] HashComparison = new byte[Blockchain.NumberOfLeadingZeroes];
    public static Block Create(List<Transaction> transactions, byte[] previousBlockHash)
    {
        // obtain merkle root of Tx list
        byte[] merkleRoot = MerkleRootService.FindMerkleRoot(transactions);

        // find nonce
        long nonce = -1;
        byte[] hash = [];

        do
        {
            nonce++;
            hash = HashBlock(nonce, merkleRoot, previousBlockHash);
        } while (!hash[..Blockchain.NumberOfLeadingZeroes].SequenceEqual(HashComparison));

        return new Block
        {
            PreviousHash = previousBlockHash,
            Hash = hash,
            Transactions = transactions,
            Nonce = nonce
        };
    }

    public static byte[] HashBlock(long nonce, byte[] merkleRoot, byte[] previousBlockHash)
    {
        byte[] nonceBytes = BitConverter.GetBytes(nonce);
        byte[] wholeHash = [.. nonceBytes, .. merkleRoot, .. previousBlockHash];
        return SHA256.HashData(wholeHash);
    }

    public static byte[] HashBlock(Block blockToHash)
    {
        return HashBlock(blockToHash.Nonce, MerkleRootService.FindMerkleRoot(blockToHash.Transactions), blockToHash.PreviousHash);
    }
}

