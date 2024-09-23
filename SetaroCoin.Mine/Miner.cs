using System.Reflection.Metadata;
using SetaroCoin.Coin.Models;
using SetaroCoin.Coin.Services;
using SetaroCoin.Network;

namespace SetaroCoin.Mine;

public class Miner : BaseMiner
{
    /// <summary>
    /// Used by the miner to start and stop mining.
    /// </summary>
    private CancellationTokenSource _startStopMiningCancellationToken;
    
    /// <summary>
    /// Used by the network to stop the current mining iteration and start a new one.
    /// </summary>
    private CancellationTokenSource _restartMiningCancellationToken;
    
    private readonly TaskFactory _taskFactory = new(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
    
    public Miner()
    {
        _startStopMiningCancellationToken = new CancellationTokenSource();
        _restartMiningCancellationToken = new CancellationTokenSource();
        Blockchain.OnNewBlockAdded += OnNewBlockAdded;
    }


    /// <summary>
    /// Starts mining.
    /// </summary>
    /// <returns>The mining task.</returns>
    public Task StartAsync()
    {
        _startStopMiningCancellationToken = new CancellationTokenSource();
        _restartMiningCancellationToken = new CancellationTokenSource();
        Console.WriteLine($"Miner with ID {MinerId} started mining.");
        var task = _taskFactory.StartNew(_Mine, _startStopMiningCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        return task;
    }

    
    /// <summary>
    /// Stops mining
    /// </summary>
    public void Stop()
    {
        _startStopMiningCancellationToken.Cancel();
        _restartMiningCancellationToken.Cancel();
        Console.WriteLine($"Miner with ID {MinerId} stopped mining.");
    }
    
    /// <summary>
    /// Internally-used function to handle the mining process.
    /// </summary>
    private async Task _Mine()
    {
        while (!_startStopMiningCancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"Miner with ID {MinerId} waiting for transactions...");
            // Get transactions from the mempool
            var transactions = await Mempool.WaitForTransactionsAsync(Blockchain.MaxTransactionsPerBlock);

            if (_restartMiningCancellationToken.IsCancellationRequested)
            {
                RestartMining();
                continue;
            }

            // Get previous block hash
            var previousHash = Blockchain.Chain.Last!.Value.Hash;
        
            if (_restartMiningCancellationToken.IsCancellationRequested)
            {
                RestartMining();
                continue;
            }
        
            // Brute force nonce
            Console.WriteLine($"Miner with ID {MinerId} calculating hash...");
            var minedBlock = BlockFactory.Create(transactions, previousHash);
        
            if (_restartMiningCancellationToken.IsCancellationRequested)
            {
                RestartMining();
                continue;
            }

            // Publish block
            if (Blockchain.AddBlock(minedBlock, MinerWallet.Address))
            {
                Console.WriteLine($"Miner with ID {MinerId} successfully found a block hash: {string.Join("", minedBlock.Hash[..5])}...");
            }
        }
    }

    private void RestartMining()
    {
        _restartMiningCancellationToken = new CancellationTokenSource();
    }
    
    
    /// <summary>
    /// Cancel current block mining if a new block was added to the blockchain.
    /// </summary>
    /// <returns>Both the new instance of the blockchain and the latest version of the ledger.</returns>
    protected override void OnNewBlockAdded(Ledger ledger, LinkedList<Block> newBlockchain)
    {
        Console.WriteLine($"OnNewBlockAdded called, Miner with ID {MinerId} restarting mining.");
        _restartMiningCancellationToken.Cancel();
        
        // Update miner's copy of ledger and blockchain
        MinerLedger = ledger;
        MinerBlockchain = newBlockchain;
    }

}