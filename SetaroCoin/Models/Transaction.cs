using SetaroCoin.Extensions;
using SetaroCoin.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetaroCoin.Models;

public class Transaction
{
    /// <summary>
    /// A hash of the transaction.
    /// </summary>
    public byte[] Hash => this.ToBytes();

    /// <summary>
    /// A Guid for this transaction
    /// </summary>
    public Guid TransactionId => _guid;
    private readonly Guid _guid = Guid.NewGuid();

    /// <summary>
    /// Wallet of the sender.
    /// </summary>
    public required UserWallet Sender { get; init; }

    /// <summary>
    /// Wallet of the recipient.
    /// </summary>
    public required UserWallet Recipient { get; init; }

    /// <summary>
    /// The amount being transacted between sender and recipient.
    /// </summary>
    public required float Amount { get; init; }

    /// <summary>
    /// The digital signature from the sender for this transaction.
    /// </summary>
    public required byte[] SenderSignature { get; init; }

    /// <summary>
    /// The amount being charged as a network fee.
    /// </summary>
    public float Fee { get; set; }

    /// <summary>
    /// If the transaction has been added to a mined block. i.e has been confirmed.
    /// </summary>
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// The block(s) this transaction was confirmed in.
    /// </summary>
    public Block[] ConfirmedIn { get; set; } = [];


}
