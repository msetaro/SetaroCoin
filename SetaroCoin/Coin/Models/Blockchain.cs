using System.Text;
using SetaroCoin.Coin.Extensions;
using SetaroCoin.Coin.Services;
using SetaroCoin.Wallet;

namespace SetaroCoin.Coin.Models;

public abstract class Blockchain
{
    /// <summary>
    /// The maximum number of transactions allowed per block.
    /// </summary>
    public const int MaxTransactionsPerBlock = 10;

    /// <summary>
    /// The number of leading zeroes a block hash must have to be considered completed.
    /// </summary>
    public const int NumberOfLeadingZeroes = 3;

    /// <summary>
    /// The number of SetaroCoin to reward the miner for finding the hash of the latest block.
    /// </summary>
    public const int MiningReward = 100;

    /// <summary>
    /// The fee to charge the sender for a transaction.
    /// </summary>
    public const float TransactionFee = 0.05f;
    
    /// <summary>
    /// Instance of the blockchain with the genesis block at the root.
    /// </summary>
    public static LinkedList<Block> Chain { get; } = new([new GenesisBlock
    {
        PreviousHash = [0],
        Hash = Encoding.UTF8.GetBytes(GenesisBlock.GenesisBlockText),
        Nonce = 0,
        Transactions = []
    }]);

    /// <summary>
    /// Maps an address to the more detailed 'UserWallet'.
    /// </summary>
    public static Ledger PublicLedger { get; } = new();
    
    
    /// <summary>
    /// Event to let miners know when a new block has been added to the chain.
    /// </summary>
    public delegate void NewBlockDelegate(Ledger ledger, LinkedList<Block> newBlockChain);
    public static event NewBlockDelegate? OnNewBlockAdded;
    
    /// <summary>
    /// Add a block to the blockchain.
    /// </summary>
    /// <param name="newBlock">The block to be added to the end of the blockchain.</param>
    /// <param name="minerWalletAddress">The address of the miner that found the hash for the new block.</param>
    /// <returns>True if the block validates successfully, false otherwise.</returns>
    public static bool AddBlock(Block newBlock, string minerWalletAddress)
    {
        // Ensure nonce is correct
        if (!newBlock.AssertNonce()) return false;
        
        // Perform transaction validation
        TransactionValidationService.ValidateTransactions(newBlock.Transactions);
        
        // Update all transactions as confirmed
        newBlock.ConfirmAllTransactions();
        
        // Add block to blockchain
        Chain.AddLast(newBlock);
        Console.WriteLine($"New block added! Hash of new block: {string.Join("", newBlock.Hash)}");
        
        // Reward miner
        PublicLedger.Instance[minerWalletAddress].Balance += MiningReward;
        Console.WriteLine($"Miner with address {minerWalletAddress[..5]}..{minerWalletAddress[^10..]} rewarded {MiningReward} SC");
        
        // Alert other miners
        OnNewBlockAdded?.Invoke(PublicLedger, Chain);
        
        return true;
    }


    /// <summary>
    /// Update the ledger with the newly added wallet.
    /// </summary>
    /// <param name="wallet">The wallet to add to the ledger.</param>
    public static void AddNewWallet(UserWallet wallet)
    {
        PublicLedger.Instance.TryAdd(wallet.Address, wallet);
    }
}

