using Transaction = SetaroCoin.Coin.Models.Transaction;

namespace SetaroCoin.Network;

/// <summary>
/// A pool of transactions awaiting to be confirmed. The first place a transaction goes after its creation.
/// </summary>
public abstract class Mempool
{
    private static readonly List<Transaction> Transactions = [];

    /// <summary>
    /// Adds a transaction to the mempool to be confirmed by miners.
    /// </summary>
    /// <param name="transaction">The transaction to be added to the mempool.</param>
    public static void AddTransaction(Transaction transaction)
    {
        Transactions.Add(transaction);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"Added transaction with ID: {transaction.TransactionId}");
        Console.ResetColor();
    }
    

    /// <summary>
    /// Remove a transaction from the mempool. Called when the transaction has been confirmed.
    /// </summary>
    /// <param name="transaction">The transaction to remove from the mempool.</param>
    public static void RemoveTransaction(Transaction transaction)
    {
        Transactions.Remove(transaction);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"Removed transaction with ID: {transaction.TransactionId}");
        Console.ResetColor();
    }

    /// <summary>
    /// Waits until the mempool reaches the desired number of transactions.
    /// </summary>
    /// <param name="count">The number of transactions to wait for.</param>
    /// <returns>A list of transactions of 'count' size.</returns>
    public static async Task<List<Transaction>> WaitForTransactionsAsync(int count)
    {
        while (Transactions.Count < count)
        {
            await Task.Delay(50);
        }
        
        return Transactions.ToList()[..count];
    }
}