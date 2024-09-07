using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SetaroCoin.Coin.Models;

namespace SetaroCoin.Coin.Extensions;

public static class TransactionEx
{
    /// <summary>
    /// Converts a transaction into bytes.
    /// </summary>
    /// <param name="transaction">The transaction to be converted.</param>
    /// <returns>A byte array representation of the transaction.</returns>
    public static byte[] ToBytes(this Transaction transaction)
    {
        string json = JsonSerializer.Serialize(transaction);
        return Encoding.UTF8.GetBytes(json);
    }
}

