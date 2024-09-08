namespace SetaroCoin.Coin.Enum;

public enum TransactionStatus
{
    Null,
    InvalidSignature,
    InvalidSenderAddress,
    InvalidRecipientAddress,
    InsufficientFunds,
    Success
}