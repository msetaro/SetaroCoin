using SetaroCoin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SetaroCoin.Services;

public static class BlockFactory
{
    public static Block Create(List<Transaction> transactions, byte[] previousBlockHash)
    {
        // obtain merkle root of Tx list
        byte[] merkleRoot = MerkleRootService.FindMerkleRoot(transactions);

        // find nonce
        long nonce = 0;
        byte[] hash = [];

        do
        {
            byte[] nonceBytes = BitConverter.GetBytes(nonce);
            byte[] wholeHash = [.. nonceBytes, .. merkleRoot, .. previousBlockHash];
            hash = SHA256.HashData(wholeHash);
            nonce++;
        } while (hash[..Blockchain.NumberOfLeadingZeroes].SequenceEqual(new byte[Blockchain.NumberOfLeadingZeroes]));

        return new Block
        {
            PreviousHash = previousBlockHash,
            Hash = hash,
            Transactions = transactions,
            Nonce = nonce
        };
    }
}

