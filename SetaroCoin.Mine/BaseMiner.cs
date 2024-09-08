using SetaroCoin.Coin.Models;
using SetaroCoin.Wallet;

namespace SetaroCoin.Mine;

public abstract class BaseMiner
{
    /// <summary>
    /// The wallet associated with this miner.
    /// </summary>
    public UserWallet MinerWallet { get; } = new();

    /// <summary>
    /// Guid of the miner.
    /// </summary>
    public Guid MinerId { get; } = Guid.NewGuid();
    
    /// <summary>
    /// Miner's copy of the ledger.
    /// </summary>
    protected Ledger MinerLedger { get; set; }
    
    /// <summary>
    /// Miner's copy of the blockchain.
    /// </summary>
    protected LinkedList<Block> MinerBlockchain { get; set; }
    
    /// <summary>
    /// Subscribe to blockchain events for when another miner successfully finds a new block.
    /// </summary>
    /// <returns></returns>
    protected abstract void OnNewBlockAdded(Ledger ledger, LinkedList<Block> newBlockchain);
}