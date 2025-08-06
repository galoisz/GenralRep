using StackExchange.Redis;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        string redisConnection = "localhost:6379";
        var redis = await ConnectionMultiplexer.ConnectAsync(redisConnection);
        Console.WriteLine("Connected to Redis!");

        var db = redis.GetDatabase();

        // Add your Redis commands here

        //var pingResult = await db.PingAsync();
        //Console.WriteLine($"Ping: {pingResult}");

        //await Op1(db);
        //await Op2(db);
        //await Op3(db);
        //await GetList(db);
        //await SetOp(db);
        await OpSortedSetLimitCapacity(db);
    }

    private static async Task GetList(IDatabase db)
    {

        string listKey = "myList";

        // Clear the list (for demonstration)
        await db.KeyDeleteAsync(listKey);

        // LPUSH - Add items to the beginning of the list
        await db.ListLeftPushAsync(listKey, "Item1");
        await db.ListLeftPushAsync(listKey, "Item2");
        await db.ListLeftPushAsync(listKey, "Item3");

        // RPUSH - Add items to the end of the list
        await db.ListRightPushAsync(listKey, "Item4");
        await db.ListRightPushAsync(listKey, "Item5");

        // Retrieve all elements in the list
        RedisValue[] listItems = await db.ListRangeAsync(listKey, 0, -1);
        Console.WriteLine("Current List:");
        foreach (var item in listItems)
        {
            Console.WriteLine(item);
        }

        // LPOP - Remove the first element
        RedisValue firstItem = await db.ListLeftPopAsync(listKey);
        Console.WriteLine($"\nPopped First Item: {firstItem}");

        // RPOP - Remove the last element
        RedisValue lastItem = await db.ListRightPopAsync(listKey);
        Console.WriteLine($"Popped Last Item: {lastItem}");

        // Get list length
        long listLength = await db.ListLengthAsync(listKey);
        Console.WriteLine($"\nList Length: {listLength}");

    }


    private static async Task SetOp(IDatabase db)
    {

        string setKey = "uniqueUsers";

        // Clear the set (for demonstration)
        await db.KeyDeleteAsync(setKey);

        // SADD - Add values to the set
        await db.SetAddAsync(setKey, "Alice");
        await db.SetAddAsync(setKey, "Bob");
        await db.SetAddAsync(setKey, "Bob");
        await db.SetAddAsync(setKey, "Charlie");
        await db.SetAddAsync(setKey, "Alice"); // Duplicate - will not be added

        // Retrieve all elements
        RedisValue[] users = await db.SetMembersAsync(setKey);
        Console.WriteLine("Users in Set:");
        foreach (var user in users)
        {
            Console.WriteLine(user);
        }

        // SISMEMBER - Check if a user exists
        bool exists = await db.SetContainsAsync(setKey, "Bob");
        Console.WriteLine($"\nIs 'Bob' in the set? {exists}");

        // SREM - Remove a user
        await db.SetRemoveAsync(setKey, "Charlie");
        Console.WriteLine("\n'Charlie' removed from the set.");

        // SCARD - Get set size
        long count = await db.SetLengthAsync(setKey);
        Console.WriteLine($"Total users in set: {count}");

    }

    private static async Task OpSortedSetLimitCapacity(IDatabase db)
    {
        string sortedSetKey = "events";

        // Clear the sorted set (for demonstration)
        await db.KeyDeleteAsync(sortedSetKey);

        // Function to add event with current timestamp as score
        async Task AddEvent(string eventName)
        {
            double timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            await db.SortedSetAddAsync(sortedSetKey, eventName, timestamp);
            Console.WriteLine($"Added event: {eventName} with timestamp: {timestamp}");

            // Ensure the sorted set contains only 10 items (remove older items if necessary)
            await db.SortedSetRemoveRangeByRankAsync(sortedSetKey, 0, -5); // Remove all items beyond the 10 most recent
        }

        // Add events
        await AddEvent("Event 1");
        await Task.Delay(1000); // Simulate delay
        await AddEvent("Event 2");
        await Task.Delay(1000);
        await AddEvent("Event 3");
        await Task.Delay(1000);
        await AddEvent("Event 4");
        await Task.Delay(1000);
        await AddEvent("Event 5");
        await Task.Delay(1000);
        await AddEvent("Event 6");
        await Task.Delay(1000);
        await AddEvent("Event 7");
        await Task.Delay(1000);
        await AddEvent("Event 8");
        await Task.Delay(1000);
        await AddEvent("Event 9");
        await Task.Delay(1000);
        await AddEvent("Event 10");
        await Task.Delay(1000);
        await AddEvent("Event 11");

        // Retrieve and display the sorted set (events ordered by timestamp)
        RedisValue[] events = await db.SortedSetRangeByRankAsync(sortedSetKey, 0, -1);
        Console.WriteLine("\nCurrent events in the sorted set (latest first):");
        foreach (var ev in events)
        {
            Console.WriteLine(ev);
        }


    }

    private static async Task OpSortedSet(IDatabase db)
    {
        string sortedSetKey = "leaderboard";

        // Clear the sorted set (for demonstration)
        await db.KeyDeleteAsync(sortedSetKey);

        // ZADD - Add members with scores
        await db.SortedSetAddAsync(sortedSetKey, "Alice", 50);  // Alice has score 50
        await db.SortedSetAddAsync(sortedSetKey, "Bob", 75);    // Bob has score 75
        await db.SortedSetAddAsync(sortedSetKey, "Charlie", 60); // Charlie has score 60

        // ZRANGE - Get members in ascending order of score
        RedisValue[] topPlayers = await db.SortedSetRangeByRankAsync(sortedSetKey, 0, -1);
        Console.WriteLine("Players in Ascending Order of Score:");
        foreach (var player in topPlayers)
        {
            Console.WriteLine(player);
        }

        // ZINCRBY - Increase a player's score
        await db.SortedSetIncrementAsync(sortedSetKey, "Alice", 10); // Alice's score increases by 10

        // ZREVRANGE - Get members in descending order of score
        RedisValue[] topPlayersDescending = await db.SortedSetRangeByRankAsync(sortedSetKey, 0, -1, Order.Descending);
        Console.WriteLine("\nPlayers in Descending Order of Score:");
        foreach (var player in topPlayersDescending)
        {
            Console.WriteLine(player);
        }

        // ZREM - Remove a player from the sorted set
        await db.SortedSetRemoveAsync(sortedSetKey, "Charlie");
        Console.WriteLine("\n'Charlie' removed from the leaderboard.");

        // ZRANK - Get the rank of a player
        long rank = await db.SortedSetRankAsync(sortedSetKey, "Alice") ?? 0;
        Console.WriteLine($"\nRank of Alice: {rank}");

        // ZSCORE - Get the score of a player
        double score = await db.SortedSetScoreAsync(sortedSetKey, "Alice") ?? 0;
        Console.WriteLine($"\nAlice's Score: {score}");
    }

        private static async Task Op1(IDatabase db)
    {

        await db.StringSetAsync("testKey", "Hello, Redis!");
        string value = await db.StringGetAsync("testKey");
        Console.WriteLine($"Value for 'testKey': {value}");

    }

    private static async Task Op2(IDatabase db)
    {
        await db.ListRightPushAsync("myList", "Value1");
        await db.ListRightPushAsync("myList", "Value2");

        var listLength = await db.ListLengthAsync("myList");
        Console.WriteLine($"List length: {listLength}");

        var firstElement = await db.ListRightPopAsync("myList");
        Console.WriteLine($"First element: {firstElement}");

    }

    private static async Task Op3(IDatabase db)
    {
        await db.HashSetAsync("user:1", new HashEntry[]
 {
    new HashEntry("name", "John1 Doe"),
    new HashEntry("email", "john1.doe@example.com")
 });

        var userName = await db.HashGetAsync("user:1", "name");
        Console.WriteLine($"User name: {userName}");


    }

}
