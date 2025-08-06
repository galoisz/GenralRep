using System;
using System.Collections.Generic;
using System.Linq;

public class FUCache<K, V>
{
    private readonly int _capacity;
    private readonly Dictionary<K, (V Value, int Frequency)> _cache;
    private readonly SortedDictionary<int, LinkedHashSet<K>> _frequencyList;

    public FUCache(int capacity)
    {
        _capacity = capacity;
        _cache = new Dictionary<K, (V, int)>();
        _frequencyList = new SortedDictionary<int, LinkedHashSet<K>>();
    }

    public V Get(K key)
    {
        if (!_cache.ContainsKey(key)) throw new KeyNotFoundException("Key not found in cache.");

        var (value, freq) = _cache[key];

        // Update frequency
        _cache[key] = (value, freq + 1);
        UpdateFrequency(key, freq, freq + 1);

        return value;
    }

    public void Put(K key, V value)
    {
        if (_capacity <= 0) return;

        if (_cache.ContainsKey(key))
        {
            var (_, freq) = _cache[key];
            _cache[key] = (value, freq + 1);
            UpdateFrequency(key, freq, freq + 1);
        }
        else
        {
            if (_cache.Count >= _capacity)
                EvictLeastFrequentlyUsed();

            _cache[key] = (value, 1);
            AddToFrequencyList(key, 1);
        }
    }

    private void UpdateFrequency(K key, int oldFreq, int newFreq)
    {
        _frequencyList[oldFreq].Remove(key);
        if (_frequencyList[oldFreq].Count == 0) _frequencyList.Remove(oldFreq);

        AddToFrequencyList(key, newFreq);
    }

    private void AddToFrequencyList(K key, int freq)
    {
        if (!_frequencyList.ContainsKey(freq))
            _frequencyList[freq] = new LinkedHashSet<K>();

        _frequencyList[freq].Add(key);
    }

    private void EvictLeastFrequentlyUsed()
    {
        if (_frequencyList.Count == 0) return;

        var leastFreq = _frequencyList.Keys.First();
        var keyToRemove = _frequencyList[leastFreq].First();

        _frequencyList[leastFreq].Remove(keyToRemove);
        if (_frequencyList[leastFreq].Count == 0) _frequencyList.Remove(leastFreq);

        _cache.Remove(keyToRemove);
    }
}

// Utility class for ordered key storage
public class LinkedHashSet<T> : LinkedList<T>
{
    private readonly HashSet<T> _hashSet = new();

    public new void Add(T item)
    {
        if (_hashSet.Contains(item)) return;
        _hashSet.Add(item);
        AddLast(item);
    }

    public new bool Remove(T item)
    {
        if (!_hashSet.Remove(item)) return false;
        var node = Find(item);
        if (node != null) Remove(node);
        return true;
    }
}



class Program
{
    static void Main()
    {
        var cache = new FUCache<int, string>(3);

        cache.Put(1, "A");
        cache.Put(2, "B");
        cache.Put(3, "C");

        cache.Get(1); // Access 1 to increase frequency
        cache.Get(2); // Access 2 to increase frequency
        cache.Get(1); // Access 1 again

        cache.Put(4, "D"); // Should evict 3 (least frequently used)

        try { Console.WriteLine(cache.Get(3)); }
        catch (Exception e) { Console.WriteLine(e.Message); } // Expected: "Key not found in cache."

        Console.WriteLine(cache.Get(1)); // Output: "A"
        Console.WriteLine(cache.Get(2)); // Output: "B"
        Console.WriteLine(cache.Get(4)); // Output: "D"
    }
}
