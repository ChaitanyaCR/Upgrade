internal sealed class DatabaseNode(string name)
{
    private readonly object _syncRoot = new();
    private readonly List<ReplicatedRecord> _data = [];

    public string Name { get; } = name;

    public IReadOnlyList<string> Data
    {
        get
        {
            lock (_syncRoot)
            {
                return [.. _data
                    .OrderBy(record => record.SequenceNumber)
                    .Select(record => record.Value)];
            }
        }
    }

    public void AddRecord(string record, long sequenceNumber)
    {
        lock (_syncRoot)
        {
            _data.Add(new ReplicatedRecord(sequenceNumber, record));
        }
    }
}
