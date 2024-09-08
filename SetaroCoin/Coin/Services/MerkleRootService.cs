using System.Security.Cryptography;
using SetaroCoin.Coin.Models;

namespace SetaroCoin.Coin.Services;

public class MerkleRootService
{
    public static byte[] FindMerkleRoot(List<Transaction> list)
    {
        try
        {
            int numOfIterations = list.Count / 2;
            List<List<byte[]>> generations = [];
            generations.AddRange(Enumerable.Range(0, numOfIterations + 1).Select(num => new List<byte[]>()));

            // add generations lists

            // Get the first iteration
            for (int j = 0; j < list.Count; j += 2)
            {
                byte[] h1 = SHA256.HashData(list[j].Hash);
          
                byte[] h2 = (j + 1) < list.Count 
                    ? SHA256.HashData(list[j + 1].Hash)
                    : SHA256.HashData(list[j].Hash);

                byte[] merkleProduct = SHA256.HashData(h1.Concat(h2).ToArray());
                
                generations[1].Add(merkleProduct);
            }

            for(int i = 1; i < numOfIterations; i++)
            {
                for(int j = 0; j < generations[i].Count; j += 2)
                {
                    byte[] h1 = SHA256.HashData(generations[i][j]);
                    byte[] h2 = (j + 1) < generations[i].Count
                        ? SHA256.HashData(generations[i][j + 1])
                        : SHA256.HashData(generations[i][j]);

                    byte[] merkleProduct = SHA256.HashData(h1.Concat(h2).ToArray());
                    
                    generations[i + 1].Add(merkleProduct);
                }
            }

            // the last array in 'produts' will always be the merkle root
            return generations.Last().Last();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

