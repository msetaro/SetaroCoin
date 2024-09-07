using SetaroCoin.Coin.Extensions;
using SetaroCoin.Coin.Models;

namespace SetaroCoin.Coin.Services;

/// <summary>
/// Responsible for validating a list of transactions in the block
/// </summary>
internal static class TransactionValidationService
{
    public static void ValidateTransactions(List<Transaction> transactions)
    {
        foreach (var transaction in transactions.ToList().Where(transaction => !transaction.Validate()))
        {
            // Remove the transaction from the list
            transactions.Remove(transaction);
                
            // Set state of transaction
            transaction.OnTransactionFailed();
        }
    }
}
