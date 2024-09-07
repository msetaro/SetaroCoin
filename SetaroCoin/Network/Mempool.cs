using Transaction = SetaroCoin.Coin.Models.Transaction;

namespace SetaroCoin.Network;

/// <summary>
/// A pool of transactions awaiting to be confirmed. The first place a transaction goes after its creation.
/// </summary>
public abstract class Mempool
{
    private static readonly List<Transaction> Transactions = [];

    public static void AddTransaction(Transaction transaction)
    {
        Transactions.Add(transaction);
    }

    public static List<Transaction> GetTransactions()
    {
        return Transactions;
    }

    public static void Clear()
    {
        Transactions.Clear();
    }
}