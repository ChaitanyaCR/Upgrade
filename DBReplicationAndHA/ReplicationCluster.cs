using System.Collections.Concurrent;

internal sealed class ReplicationCluster(DatabaseNode primary, IEnumerable<DatabaseNode> replicas)
{
    private readonly object _stateLock = new();
    private readonly Random _random = new();
    private readonly List<DatabaseNode> _replicas = [.. replicas];
    private readonly ConcurrentBag<Task> _replicationTasks = [];
    private long _writeSequence;

    public DatabaseNode Primary { get; private set; } = primary;

    public bool IsPrimaryAlive { get; private set; } = true;

    public async Task WriteDataAsync(string record)
    {
        DatabaseNode currentPrimary;
        List<DatabaseNode> replicasSnapshot;

        lock (_stateLock)
        {
            if (!IsPrimaryAlive)
            {
                throw new InvalidOperationException("No active primary is available to accept writes.");
            }

            currentPrimary = Primary;
            replicasSnapshot = _replicas.ToList();
        }

        var sequenceNumber = Interlocked.Increment(ref _writeSequence);
        currentPrimary.AddRecord(record, sequenceNumber);
        Console.WriteLine($"{currentPrimary.Name} accepted write: {record}");

        foreach (var replica in replicasSnapshot)
        {
            var replicationTask = ReplicateAsync(replica, record, sequenceNumber);
            _replicationTasks.Add(replicationTask);
        }

        await Task.Yield();
    }

    public void SimulatePrimaryCrash()
    {
        lock (_stateLock)
        {
            IsPrimaryAlive = false;
            Console.WriteLine($"{Primary.Name} has crashed.");
        }
    }

    public async Task StartHealthMonitorAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(200, cancellationToken);

                lock (_stateLock)
                {
                    if (IsPrimaryAlive)
                    {
                        continue;
                    }

                    if (_replicas.Count == 0)
                    {
                        Console.WriteLine("No replicas available for promotion.");
                        continue;
                    }

                    var promotedReplica = _replicas[0];
                    _replicas.RemoveAt(0);
                    _replicas.Add(Primary);
                    Primary = promotedReplica;
                    IsPrimaryAlive = true;

                    Console.WriteLine($"Primary Down! Promoting {promotedReplica.Name} to Primary.");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task WaitForReplicationAsync()
    {
        while (_replicationTasks.TryTake(out var replicationTask))
        {
            await replicationTask;
        }
    }

    public void PrintNodeContents()
    {
        lock (_stateLock)
        {
            Console.WriteLine($"Primary ({Primary.Name}): [{string.Join(", ", Primary.Data)}]");

            foreach (var replica in _replicas)
            {
                Console.WriteLine($"Replica ({replica.Name}): [{string.Join(", ", replica.Data)}]");
            }
        }
    }

    private async Task ReplicateAsync(DatabaseNode replica, string record, long sequenceNumber)
    {
        var delay = _random.Next(150, 400);
        await Task.Delay(delay);
        replica.AddRecord(record, sequenceNumber);
        Console.WriteLine($"Replicated '{record}' to {replica.Name} after {delay} ms.");
    }
}
