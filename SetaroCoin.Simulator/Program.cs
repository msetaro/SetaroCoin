// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Concurrent;
using SetaroCoin.Coin.Models;
using SetaroCoin.Mine;
using SetaroCoin.Wallet;

namespace SetaroCoin.Simulator;
public class Program
{
    private static Miner miner1;
    private static Miner miner2;
    private static Miner miner3;
    
    private static UserWallet user1 = new() { Balance = 100 };
    private static UserWallet user2 = new() { Balance = 200 };
    private static UserWallet user3 = new() { Balance = 300 };
    private static UserWallet user4 = new() { Balance = 400 };
    private static UserWallet user5 = new() { Balance = 500 };


    
    private static List<UserWallet> users = [user1, user2, user3, user4, user5];

    public static async Task Main(string[] args)
    {
        // Create miners
        miner1 = new Miner();
        var miner1Thread = new Thread(() => { miner1.StartAsync(); });
        miner1Thread.Start();
        
        miner2 = new Miner();
        var miner2Thread = new Thread(() => { miner2.StartAsync(); });
        miner2Thread.Start();
        
        miner3 = new Miner();
        var miner3Thread = new Thread(() => { miner3.StartAsync(); });
        miner3Thread.Start();

        // Randomly create transactions
        while (true)
        {
            try
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
            
                // Choose a random wallet
                var sender = users[new Random().Next(0, users.Count)];
            
                // Choose a recipient
                UserWallet recipient = users[new Random().Next(0, users.Count)];
                while (recipient.Address == sender.Address)
                {
                    recipient = users[new Random().Next(0, users.Count)];
                }
                
                Task.Run(async () => await sender.Send(recipient.Address, new Random().Next(0, 5)));
            }
            catch (Exception e)
            {
                Console.WriteLine($"This is in here {e}");
            }
        }
    }
}


