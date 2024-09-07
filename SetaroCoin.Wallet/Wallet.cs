using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SetaroCoin.Wallet;

public class UserWallet
{
    public string Address => BitConverter.ToString(_address);
    private readonly byte[] _address = SHA256.HashData(Guid.NewGuid().ToByteArray());
}

