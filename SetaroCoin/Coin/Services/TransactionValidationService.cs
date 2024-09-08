using SetaroCoin.Coin.Enum;
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
        try
        {
            foreach (var transaction in transactions.ToList())
            {
                var status = transaction.Validate();
                if (status == TransactionStatus.Success) continue;
            
                // Remove the transaction from the list
                transactions.Remove(transaction);
                
                // Set state of transaction
                transaction.OnTransactionFailed(status);

                Console.WriteLine($"Transaction Validation Failed with ID: {transaction.TransactionId} | Reason: {status}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
