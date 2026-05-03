List<string> entityList = ["Entity1", "Entity2", "Entity3", "Entity4", "Entity5"];

const int transactionCount = 200;

for (int i = 0; i < transactionCount; i++)
{
    var transaction = TransactionGenerator.GenerateRandomTransaction(entityList);
    ShardingManager.AddTransaction(transaction);
}

Console.WriteLine("Transactions per shard:");
Console.WriteLine($"Shard 1: {ShardingManager.Shard1.Count}");
Console.WriteLine($"Shard 2: {ShardingManager.Shard2.Count}");
Console.WriteLine($"Shard 3: {ShardingManager.Shard3.Count}");

string sampleEntityId = entityList[0];
List<Transaction> entityTransactions = ShardingManager.GetTransactionsForEntity(sampleEntityId);

Console.WriteLine();
Console.WriteLine($"Transactions for {sampleEntityId}: {entityTransactions.Count}");

foreach (Transaction transaction in entityTransactions)
{
    Console.WriteLine(
        $"{transaction.Id} | {transaction.EntityId} | {transaction.Amount:C} | {transaction.Date:g}");
}
