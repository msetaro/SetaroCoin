using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetaroCoin.Models;
public class Block
{
    /// <summary>
    /// A list of transactions corresponding to this block.
    /// </summary>
    public required List<Transaction> Transactions { get; init; } = [];


    /// <summary>
    /// A string of the hash of the block that came before this one in the Blockchain.
    /// </summary>
    public required byte[] PreviousHash { get; init; } = [];

    /// <summary>
    /// This instance's hash
    /// </summary>
    public required byte[] Hash { get; init; } = [];

    /// <summary>
    /// The nonce that has been calculated via PoW.
    /// </summary>
    public required long Nonce { get; init; } = 0;

    /// <summary>
    /// A base-64 representation of the block hash string.
    /// </summary>
    public string HashString => Convert.ToBase64String(Hash);
}
