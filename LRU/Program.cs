using System;
using System.Collections.Generic;

public class LRUCache<K, V>
{
    private readonly int capacity;
    private readonly Dictionary<K, LinkedListNode<CacheItem>> cacheMap;
    private readonly LinkedList<CacheItem> lruList;

    public LRUCache(int capacity)
    {
        this.capacity = capacity;
        this.cacheMap = new Dictionary<K, LinkedListNode<CacheItem>>();
        this.lruList = new LinkedList<CacheItem>();
    }

    public V Get(K key)
    {
        if (!cacheMap.ContainsKey(key))
            throw new KeyNotFoundException("Key not found");

        var node = cacheMap[key];
        lruList.Remove(node);
        lruList.AddFirst(node); // Move to front (most recently used)

        return node.Value.Value;
    }

    public void Put(K key, V value)
    {
        if (cacheMap.ContainsKey(key))
        {
            var node = cacheMap[key];
            node.Value.Value = value;
            lruList.Remove(node);
            lruList.AddFirst(node);
        }
        else
        {
            if (cacheMap.Count >= capacity)
            {
                // Remove the least recently used item (last node in the list)
                var lruNode = lruList.Last;
                if (lruNode != null)
                {
                    cacheMap.Remove(lruNode.Value.Key);
                    lruList.RemoveLast();
                }
            }

            var newItem = new CacheItem(key, value);
            var newNode = new LinkedListNode<CacheItem>(newItem);
            lruList.AddFirst(newNode);
            cacheMap[key] = newNode;
        }
    }

    public void DisplayCache()
    {
        foreach (var item in lruList)
        {
            Console.Write($"[{item.Key}: {item.Value}] ");
        }
        Console.WriteLine();
    }

    private class CacheItem
    {
        public K Key { get; }
        public V Value { get; set; }

        public CacheItem(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}

class Program
{
    static void Main()
    {
        LRUCache<int, string> lruCache = new LRUCache<int, string>(3);

        lruCache.Put(1, "A");
        lruCache.Put(2, "B");
        lruCache.Put(3, "C");
        lruCache.DisplayCache(); // Output: [3: C] [2: B] [1: A]

        lruCache.Get(1);
        lruCache.DisplayCache(); // Output: [1: A] [3: C] [2: B]

        lruCache.Put(4, "D");
        lruCache.DisplayCache(); // Output: [4: D] [1: A] [3: C] (2 was evicted)

        lruCache.Put(5, "E");
        lruCache.DisplayCache(); // Output: [5: E] [4: D] [1: A] (3 was evicted)
    }
}
