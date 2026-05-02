using System.Security.Cryptography;
using System.Text;

class ConsistentHashing
{
    private static readonly SortedDictionary<int, string> ring = [];

    public static void Hash()
    {
        // Step 1: Add initial servers
        AddServer("Server A");
        AddServer("Server B");
        AddServer("Server C");

        var keys = new List<string>
        {
            "Report_1", "Report_2", "Report_3", "Report_4", "Report_5",
            "Report_6", "Report_7", "Report_8", "Report_9", "Report_10"
        };

        Console.WriteLine("=== Initial Mapping ===");
        var initialMapping = MapKeys(keys);

        // Step 2: Add new server
        Console.WriteLine("\nAdding Server D...\n");
        AddServer("Server D");

        Console.WriteLine("=== After Adding Server D ===");
        var newMapping = MapKeys(keys);

        // Step 3: Compare redistribution
        Console.WriteLine("\n=== Keys That Moved ===");
        foreach (var key in keys)
        {
            if (initialMapping[key] != newMapping[key])
            {
                Console.WriteLine($"{key} moved from {initialMapping[key]} → {newMapping[key]}");
            }
        }
    }

    static void AddServer(string serverName)
    {
        int hash = ComputeHash(serverName);

        // Handle rare collision
        while (ring.ContainsKey(hash))
        {
            hash++;
        }

        ring[hash] = serverName;
        Console.WriteLine($"Added {serverName} at position {hash}");
    }

    static string GetServerForKey(string key)
    {
        int hash = ComputeHash(key);

        // Find first server >= hash
        foreach (var entry in ring)
        {
            if (entry.Key >= hash)
                return entry.Value;
        }

        // Wrap around
        return ring.First().Value;
    }

    static Dictionary<string, string> MapKeys(List<string> keys)
    {
        var mapping = new Dictionary<string, string>();

        foreach (var key in keys)
        {
            var server = GetServerForKey(key);
            mapping[key] = server;
            Console.WriteLine($"{key} → {server}");
        }

        return mapping;
    }

    static int ComputeHash(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));

        // Convert first 4 bytes to int
        return BitConverter.ToInt32(bytes, 0) & 0x7fffffff;
    }
}