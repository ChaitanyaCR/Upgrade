# Database Replication & High Availability

**Goal:** Master the strategies for data redundancy and failover in mission-critical systems.

### 1. The Strategic Lesson

Replication is the process of keeping a copy of the same data on multiple machines. This provides **Redundancy** (safety) and **Scalability** (read performance).

#### Primary-Replica (Master-Slave) Architecture

**The Flow:** All "Writes" (e.g., New BSM transactions, PD updates) go to the **Primary** node. The Primary then copies that data to one or more **Replica** nodes.

**The Use Case:** Most fintech apps are "Read-Heavy." Users are constantly viewing reports but only updating them occasionally. You can scale your "Reads" by adding more Replicas.

#### Synchronous vs. Asynchronous Replication

- **Synchronous:** The Primary waits for the Replica to confirm it received the data before telling the user "Success."
  - **Pro:** Zero data loss.
  - **Con:** High latency. If the network is slow, your whole app slows down.
- **Asynchronous:** The Primary saves the data and tells the user "Success" immediately, then sends the update to the Replica in the background.
  - **Pro:** Very fast.
  - **Con:** Risk of "Data Loss" if the Primary crashes before the update reaches the Replica.

#### High Availability (HA) & Failover

In a Lead/Architect role, you must design for **Failover**. If the Primary dies, a monitoring service (like Sentinel in Redis or Zookeeper in distributed systems) must detect the failure, promote a Replica to be the new Primary, and re-route traffic.

### 2. The 1-Hour Practical Task

**Objective:** Model a Primary-Replica synchronization system in C# using an asynchronous event-driven approach.

#### Step 1: Setup (10 mins)

Create a Console App. Define a DatabaseNode class with a List<string> Data and a Name.

#### Step 2: Logic - The Sync Process (40 mins)

1.  **The Primary:** Create a method `WriteData(string record)`.
2.  **The Replication:** When `WriteData` is called, simulate a small delay (`Task.Delay`) to represent network latency, then update a list of Replica objects.
3.  **The Failover Simulator:** Write a "Health Monitor" loop. If a boolean `isPrimaryAlive` is set to `false`, your code should automatically pick the first Replica and designate it as the new Primary.
    - _Print:_ "Primary Down! Promoting Replica-1 to Primary."

#### Step 3: Verification (10 mins)

1.  Send 5 write requests.
2.  Print the contents of both Primary and Replica to ensure they match.
3.  Trigger a "Crash," run a write request, and ensure the system routes it to the new Primary.

### 3. Architect’s Note for the Overseas Market

In Tier-1 interviews, you will be asked about "Replication Lag." This happens when a user updates their balance, but when they refresh the page, they see the old balance because the Replica hasn't caught up yet.

**The Solution:** "Read-your-own-writes" consistency. You track the timestamp of the last write and ensure the user only reads from a replica that is at least that up-to-date, or you force their specific read to the Primary.
