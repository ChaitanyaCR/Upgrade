var replica1 = new DatabaseNode("Replica-1");
var replica2 = new DatabaseNode("Replica-2");
var cluster = new ReplicationCluster(
    new DatabaseNode("Primary-1"),
    [replica1, replica2]);

using var monitorCts = new CancellationTokenSource();
var monitorTask = cluster.StartHealthMonitorAsync(monitorCts.Token);

Console.WriteLine("=== Initial Writes ===");
for (var i = 1; i <= 5; i++)
{
    await cluster.WriteDataAsync($"Transaction-{i}");
}

await cluster.WaitForReplicationAsync();
cluster.PrintNodeContents();

Console.WriteLine();
Console.WriteLine("=== Simulating Crash ===");
cluster.SimulatePrimaryCrash();
await Task.Delay(500);

Console.WriteLine();
Console.WriteLine("=== Write After Failover ===");
await cluster.WriteDataAsync("Transaction-6");
await cluster.WaitForReplicationAsync();
cluster.PrintNodeContents();

monitorCts.Cancel();
await monitorTask;
