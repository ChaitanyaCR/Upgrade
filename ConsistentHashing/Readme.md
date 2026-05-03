# Goal: Understand how to scale distributed caches (Redis) and NoSQL databases horizontally.

## 1. The Strategic Lesson

In a standard hashing system (e.g., `server = hash(key) % n`), if you add one server ($n+1$) or one server crashes ($n-1$), almost all your keys map to different servers. 
For a Fintech app, this means a "Cache Storm"—suddenly your database is hit with thousands of requests because the cache "missed" everything.

Consistent Hashing solves this by mapping both "Servers" and "Data Keys" onto a virtual circle (the Hash Ring).

### Core Concepts:
* **The Ring**: Imagine a circle with a range of $[0, 2^{32}-1]$.
* **Server Placement**: Servers are hashed and placed at specific points on this ring.
* **Key Placement**: A key (like a UserID for a BSM report) is hashed and placed on the ring. It is served by the first server it encounters moving clockwise.
* **The Benefit**: If a server is added or removed, only a small fraction ($\frac{1}{n}$) of keys need to be remapped.
* **Virtual Nodes**: To prevent "Hotspots" (where one server gets too much data), we use Virtual Nodes. One physical server is represented by multiple points on the ring.

## 2. The 1-Hour Practical Task

**Objective**: Build a simplified Consistent Hashing implementation in .NET 10 to see how keys redistribute when "Server C" is added.

### Step 1: Setup (15 mins)
Create a C# Console App. Use `SortedDictionary<int, string>` to represent your "Ring." The `int` is the hash value, and the `string` is the Server Name.

### Step 2: The Logic (30 mins)
* **Hash Function**: Use a simple hash (like MD5 or `string.GetHashCode()`).
* **AddServer**: Map "Server A", "Server B", and "Server C" to the dictionary.
* **GetServerForKey**: 
  * Hash the input key (e.g., "Report_123").
  * Find the first entry in the `SortedDictionary` whose key is $\ge$ the hash of the input.
  * If no such key exists, wrap around to the first entry in the dictionary (the "Ring" behavior).

### Step 3: Verification (15 mins)
* Map 10 keys to your servers and print the distribution.
* Add "Server D" to the ring.
* Remap the same 10 keys. Notice how most keys stay on their original servers, and only a few move to Server D.