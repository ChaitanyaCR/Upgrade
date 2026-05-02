using System.Security.Cryptography;
using System.Text;

class ConsistentHashingWithVNodes
{
    // Hash ring: hash → server name
    private static readonly SortedDictionary<int, string> ring = [];

    // Actual storage: server → (key → value)
    private static readonly Dictionary<string, Dictionary<string, string>> servers = [];

    public static void Hash()
    {
        // Add initial servers with virtual nodes
        AddServer("Server A", 50);
        AddServer("Server B", 50);
        AddServer("Server C", 50);

        var keys = Enumerable.Range(1, 10).Select(i => $"Report_{i}").ToList();

        Console.WriteLine("\n=== Storing Data ===");
        foreach (var key in keys)
        {
            Set(key, $"Data for {key}");
        }

        Console.WriteLine("\n=== Initial Mapping ===");
        var initialMapping = PrintKeyLocations(keys);

        // Add new server
        Console.WriteLine("\n>>> Adding Server D...\n");
        AddServer("Server D", 50);

        Console.WriteLine("=== New Mapping (After Adding Server D) ===");
        var newMapping = PrintKeyLocations(keys);

        // Show which keys moved
        Console.WriteLine("\n=== Keys That Moved ===");
        foreach (var key in keys)
        {
            if (initialMapping[key] != newMapping[key])
            {
                Console.WriteLine($"{key}: {initialMapping[key]} → {newMapping[key]}");

                // Simulate data movement
                var oldServer = initialMapping[key];
                var newServer = newMapping[key];

                if (servers[oldServer].TryGetValue(key, out var value))
                {
                    servers[newServer][key] = value;
                    servers[oldServer].Remove(key);
                }
            }
        }

        Console.WriteLine("\n=== Final Data Distribution ===");
        PrintServerData();
    }

    // Add server with virtual nodes
    static void AddServer(string serverName, int replicas)
    {
        servers[serverName] = [];

        for (int i = 0; i < replicas; i++)
        {
            string vnodeKey = $"{serverName}#{i}";
            int hash = ComputeHash(vnodeKey);

            while (ring.ContainsKey(hash))
            {
                hash++;
            }

            ring[hash] = serverName;
        }

        Console.WriteLine($"{serverName} added with {replicas} virtual nodes");
    }

    // Store data
    static void Set(string key, string value)
    {
        var server = GetServerForKey(key);
        servers[server][key] = value;
        Console.WriteLine($"{key} stored in {server}");
    }

    // Retrieve data
    static string? Get(string key)
    {
        var server = GetServerForKey(key);
        return servers[server].TryGetValue(key, out var value)
            ? value
            : null;
    }

    // Find correct server
    static string GetServerForKey(string key)
    {
        int hash = ComputeHash(key);

        foreach (var entry in ring)
        {
            if (entry.Key >= hash)
                return entry.Value;
        }

        // Wrap around
        return ring.First().Value;
    }

    // Print mapping
    static Dictionary<string, string> PrintKeyLocations(List<string> keys)
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

    // Debug: print stored data
    static void PrintServerData()
    {
        foreach (var server in servers)
        {
            Console.WriteLine($"\n{server.Key}:");
            foreach (var kvp in server.Value)
            {
                Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
            }
        }
    }

    // Hash function
    static int ComputeHash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToInt32(bytes, 0) & 0x7fffffff;
    }
}