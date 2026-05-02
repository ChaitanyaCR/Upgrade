# 90-Day Transition Plan: Developer to Lead/Architect

## Revision Manual: Days 1 – 22

---

### 📅 PHASE 1: HIGH-LEVEL DESIGN (HLD)

#### Day 1: Load Balancers (LB)
- **The Strategic Lesson:** L4 (Transport) is fast but "blind" (IP/Port), whereas L7 (Application) is "smart" (HTTP headers/cookies). For Fintech, L7 is crucial for session persistence.
- **Task:** Build a Round Robin simulator in C# that uses a health-check flag to skip offline servers.

#### Day 2: Consistent Hashing
- **The Strategic Lesson:** Traditional hashing (n % nodes) causes a "Cache Storm" when a node fails. Consistent Hashing ensures that only 1/n keys need to be remapped, keeping Redis clusters stable.
- **Task:** Implement a Hash Ring with Virtual Nodes in .NET using a SortedDictionary.

#### Day 3: Database Sharding & Partitioning
- **The Strategic Lesson:** Horizontal sharding splits data by rows (e.g., TenantID). Essential for scaling apps with millions of users.
- **Task:** Simulate a sharded data access layer in C# that routes queries based on a CustomerRegion key.

#### Day 4: Database Replication & High Availability
- **The Strategic Lesson:** Sync replication guarantees no data loss but slows writes. Async is fast but risks "Lag." Use Semi-Sync for local replicas and Async for DR regions.
- **Task:** Create a heartbeat monitor that detects Primary DB failure and promotes a Replica.

#### Day 5: The CAP Theorem & PACELC
- **The Strategic Lesson:** In a network partition (P), you choose Consistency (C) for transactions or Availability (A) for reports. PACELC adds: "Else, choose Latency (L) or Consistency (C)."
- **Task:** Write a "Distributed Toggle" that fails a write-request if it cannot reach all nodes (CP Mode).

#### Day 6: Rate Limiting & Throttling
- **The Strategic Lesson:** Protect APIs from DDoS. Token Bucket is the gold standard because it handles bursts while maintaining a steady long-term rate.
- **Task:** Implement a thread-safe Token Bucket using timestamp-based refills.

#### Day 7: DNS, CDNs & Edge Computing
- **The Strategic Lesson:** Move logic closer to the user. Use "Edge Logic" (Wasm) to run input validation at the CDN level.
- **Task:** Simulate a Cache-Aside pattern where the "Edge" checks a local cache before hitting the origin.

#### Day 8: Storage Engines (LSM vs B-Trees)
- **The Strategic Lesson:** SQL uses B-Trees (Read-heavy). NoSQL like Cassandra uses LSM-Trees (Write-heavy). Choose LSM for high-volume audit logs.
- **Task:** Implement a Mini-LSM path: Write to a MemTable and flush to an SSTable file when full.

#### Day 9: Distributed Transactions (2PC vs Sagas)
- **The Strategic Lesson:** 2PC is "Strong" but slow. Sagas use "Compensating Transactions" to undo steps. Sagas are the standard for microservices.
- **Task:** Build a Saga Orchestrator that triggers a Revert() method if the second step of a transaction fails.

#### Day 10: Distributed Caching
- **The Strategic Lesson:** Cache-Aside (Lazy), Write-Through (Sync), Write-Behind (Async). For Fintech ticks, Write-Behind is king for performance.
- **Task:** Implement Cache-Aside logic and observe the 0ms cache hits on subsequent calls.

#### Day 11: API Gateway vs Service Mesh
- **The Strategic Lesson:** Gateway handles "North-South" (External). Service Mesh handles "East-West" (Internal mTLS/Retries). YARP is the .NET standard.
- **Task:** Set up a YARP proxy that automatically injects a X-Correlation-ID header.

#### Day 12: Message Queues & Event-Driven Architecture
- **The Strategic Lesson:** Decouple services. Kafka (Distributed Log) vs RabbitMQ (Smart Broker). Use Kafka for event sourcing your balance sheet history.
- **Task:** Implement an async producer-consumer using .NET System.Threading.Channels.

#### Day 13: Observability & Distributed Tracing
- **The Strategic Lesson:** OpenTelemetry is the 2026 standard. Use TraceIDs to follow a request through multiple microservices.
- **Task:** Instrument a calculation with ActivitySource to observe trace propagation in the console.

#### Day 14: Security & Identity
- **The Strategic Lesson:** In 2026, PKCE is mandatory. JWTs should be short-lived with Refresh Token rotation.
- **Task:** Implement Policy-based Authorization in .NET 10 requiring a specific scope (e.g., read:pd_score).

#### Day 15: HA & Disaster Recovery
- **The Strategic Lesson:** RTO (Time) and RPO (Data). Active-Active across regions is the goal for Tier-1 banks.
- **Task:** Create a monitor that re-routes traffic to a "Secondary Region" after 3 health-check failures.

---

### 📅 PHASE 2: LOW-LEVEL DESIGN (LLD)

#### Day 16: SOLID Principles (Part 1)
- **The Strategic Lesson:** OCP is the most important for Fintech—add new risk strategies without changing the core loan processor.
- **Task:** Refactor a monolithic TransactionManager using .NET 10 Primary Constructors.

#### Day 17: SOLID (Part 2) & Dependency Injection
- **The Strategic Lesson:** Master service lifetimes (Transient, Scoped, Singleton). Avoid "Captive Dependencies" (Scoped inside Singleton).
- **Task:** Create a DI test to observe how instances are shared or recreated across requests.

#### Day 18: Strategy & Factory Patterns
- **The Strategic Lesson:** Use Strategy for algorithms. .NET 10 Keyed Services make manual Factory classes obsolete.
- **Task:** Use AddKeyedSingleton to implement an InterestCalculator that switches logic based on a key.

#### Day 19: Observer & Mediator Patterns
- **The Strategic Lesson:** Decouple communication. MediatR is the standard for "Thin Controllers" and CQRS.
- **Task:** Build a pipeline where a Command triggers multiple independent handlers (Audit and Email).

#### Day 20: Proxy & Adapter Patterns
- **The Strategic Lesson:** Wrap legacy APIs. Adapter changes the interface; Proxy adds logic (Caching/Security) while keeping the interface the same.
- **Task:** Create an Adapter for a legacy SOAP API and wrap it in a CachingProxy.

#### Day 21: Milestone Review - HLD to LLD
- **The Strategic Lesson:** Connect the dots. How a code-level Strategy pattern supports high-level Microservices scalability.
- **Task:** Build a "Loan Approval Core" combining DI, Strategy, and Mediator patterns.