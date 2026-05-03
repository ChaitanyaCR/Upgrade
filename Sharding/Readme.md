# Goal: Learn how to scale your BSM or PD calculation systems when a single database instance hits its physical limits.

## 1. The Strategic Lesson
In a Balance Sheet Management (BSM) system, you might have millions of transaction records for thousands of legal entities. A single SQL Server or PostgreSQL instance will eventually struggle with disk I/O and query latency.

### Vertical vs. Horizontal Scaling
* **Vertical (Scaling Up)**: Adding more RAM/CPU to your existing DB. There is a "hard ceiling" and it becomes exponentially expensive.
* **Horizontal (Scaling Out)**: Splitting one large dataset into smaller chunks across multiple database servers.

### Sharding vs. Partitioning
* **Partitioning (Logical)**: Splitting a large table within the same database instance (e.g., by Year or Region). It helps query performance but doesn't solve hardware limits.
* **Sharding (Physical)**: Distributing data across separate database instances. This is what companies like Uber, Stripe, and large banks use to handle global scale.

### Common Sharding Strategies
* **Key/Hash-Based Sharding**: You take a Shard Key (like EntityID), hash it, and use the result to determine the shard. (Similar to the logic we learned yesterday!)
* **Range-Based Sharding**: Shard 1 holds IDs 1–10,000; Shard 2 holds 10,001–20,000. Risk: Can lead to "Hot Shards" if most new activity happens in the newest range.
* **Directory-Based Sharding**: A separate lookup table tracks which EntityID lives on which shard. Flexible, but the lookup table becomes a Single Point of Failure (SPOF).

## 2. The 1-Hour Practical Task
**Objective**: Simulate a Sharded Data Access Layer in C# to understand how an application must "route" queries to the correct database instance.

### Step 1: Setup (10 mins)
Create a Console App. Define a Transaction record:

```csharp
public record Transaction(Guid Id, string EntityId, decimal Amount, DateTime Date);
```

### Step 2: The Logic (40 mins)
* **Simulate Shards**: Create three Lists of Transactions (Shard1, Shard2, Shard3) to represent three separate database instances.
* **The Shard Manager**: Create a class that accepts a Transaction.
* **Implement Routing**: Use a simple modulo or hash on the EntityId to decide which "Shard List" the transaction should be saved to.
* **Query Simulation**: Implement a method `GetTransactionsForEntity(string entityId)` that knows exactly which shard to query without checking all three.

### Step 3: Verification (10 mins)
* Insert 20 transactions for 5 different EntityIds.
* Print the count of transactions in each "Shard List" to verify the data is distributed.

## 3. Architect’s Note for the Overseas Market
In interviews for Lead roles, always discuss the trade-offs. Sharding isn't free. Mention these "Sharding Pains":
* **Joins across shards**: Almost impossible or very slow. You often have to "denormalize" your data.
* **Fixed Shard Keys**: Once you pick a shard key (like EntityId), changing it later is a nightmare. It requires a total migration.
* **Distributed Transactions**: Ensuring data consistency across two shards (e.g., transferring money from an account on Shard A to Shard B) requires complex patterns like Sagas or Two-Phase Commit (2PC).