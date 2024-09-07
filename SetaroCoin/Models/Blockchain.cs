using SetaroCoin.Extensions;
using SetaroCoin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetaroCoin.Models;

internal abstract class Blockchain
{
    /// <summary>
    /// The maximum number of transactions allowed per block.
    /// </summary>
    public const int MaxTransactionsPerBlock = 100;

    /// <summary>
    /// The number of leading zeroes a block hash must have to be considered completed.
    /// </summary>
    public const int NumberOfLeadingZeroes = 3;



    /// <summary>
    /// Instance of the blockchain with the genesis block at the root.
    /// </summary>
    public static LinkedList<Block> Ledger => _ledger;
    private static LinkedList<Block> _ledger = new([new GenesisBlock
        {
            PreviousHash = [0],
            Hash = Encoding.UTF8.GetBytes(GenesisBlock.GenesisBlockText),
            Nonce = 0,
            Transactions = []
        }]);



    public bool AddBlock(Block newBlock)
    {
        // Perform transaction validation
        if(TransactionValidationService.ValidateTransactions(newBlock.Transactions))
        {
            // Ensure nonce is correct
            if (newBlock.AssertNonce())
            {
                _ledger.AddLast(newBlock);
                return true;
            }
        }

        return false;
    }
}

