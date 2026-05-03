public record Transaction(Guid Id, string EntityId, decimal Amount, DateTime Date);

public static class TransactionGenerator
{
    private static readonly Random random = new Random();

    public static Transaction GenerateRandomTransaction(List<string> entityList)
    {
        var id = Guid.NewGuid();
        var entityId = entityList[random.Next(entityList.Count)];
        var amount = (decimal)(random.NextDouble() * 1000);
        var date = DateTime.Now.AddDays(-random.Next(30));

        return new Transaction(id, entityId, amount, date);
    }
}

public static class ShardingManager
{
    private static readonly List<Transaction> shard1 = [];
    private static readonly List<Transaction> shard2 = [];
    private static readonly List<Transaction> shard3 = [];

    public static IReadOnlyList<Transaction> Shard1 => shard1;
    public static IReadOnlyList<Transaction> Shard2 => shard2;
    public static IReadOnlyList<Transaction> Shard3 => shard3;

    public static void AddTransaction(Transaction transaction)
    {
        GetShard(transaction.EntityId).Add(transaction);
    }

    public static List<Transaction> GetTransactionsForEntity(string entityId)
    {
        return [.. GetShard(entityId).Where(transaction => transaction.EntityId == entityId)];
    }

    private static List<Transaction> GetShard(string entityId)
    {
        int shardNumber = GetShardNumber(entityId);

        return shardNumber switch
        {
            0 => shard1,
            1 => shard2,
            _ => shard3
        };
    }

    private static int GetShardNumber(string entityId)
    {
        // Use a deterministic hash so the same entity always routes to the same shard
        // and the console demo behaves consistently across runs.
        int hash = 0;

        foreach (char character in entityId)
        {
            hash += character;
        }

        return hash % 3;
    }
}
