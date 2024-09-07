using SetaroCoin.Models;
using SetaroCoin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SetaroCoin.Extensions;

public static class BlockEx
{
    public static bool AssertNonce(this Block block)
    {
        byte[] nonceBytes = BitConverter.GetBytes(block.Nonce);
        byte[] merkleRoot = MerkleRootService.FindMerkleRoot(block.Transactions);
        byte[] wholeHash = [.. nonceBytes, .. merkleRoot, .. block.PreviousHash];
        byte[] hash = SHA256.HashData(wholeHash);

        return hash[..Blockchain.NumberOfLeadingZeroes].SequenceEqual(new byte[Blockchain.NumberOfLeadingZeroes]);
    }
}

