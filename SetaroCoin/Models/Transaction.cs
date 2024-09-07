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
    /// Wallet of the sender.
    /// </summary>
    public UserWallet Sender { get; set; }

    /// <summary>
    /// Wallet of the recipient.
    /// </summary>
    public UserWallet Recipient { get; set; }
}
