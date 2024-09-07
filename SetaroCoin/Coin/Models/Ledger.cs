using SetaroCoin.Wallet;

namespace SetaroCoin.Coin.Models;

public class Ledger
{
    public readonly Dictionary<string, UserWallet> Instance = new();
}