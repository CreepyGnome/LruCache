using System.Collections.Generic;

namespace LruCache
{
    /// <summary>
    /// A reference implementation for a Least Recently Used (LRU) cache in C#.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the cache.</typeparam>
    /// <typeparam name="TEntry">The type of the entries in the cache.</typeparam>
    public class LeastRecentlyUsedCache<TKey, TEntry>
    {
        private const int DefaultMinCapacity = 10;
        
        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TEntry>>> _cacheMap;
        private readonly LinkedList<KeyValuePair<TKey, TEntry>> _cacheList;
        
        /// <summary>
        /// The maximum capacity of the cache.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// The number of entries in the cache.
        /// </summary>
        public int Count => _cacheMap.Count;

        /// <summary>
        /// Creates the cache with the specified capacity.
        /// </summary>
        /// <remarks>
        /// If the specified capacity is less than 10 then the capacity
        /// for the cache will be set to the 10 value.
        /// </remarks>
        /// <param name="capacity">The maximum number of entries allowed in the cache. (Default = 10)</param>
        public LeastRecentlyUsedCache(int capacity = DefaultMinCapacity)
        {
            Capacity = capacity >= DefaultMinCapacity ? capacity : DefaultMinCapacity;
            _cacheMap = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TEntry>>>(Capacity);
            _cacheList = new LinkedList<KeyValuePair<TKey, TEntry>>();
        }
        
        /// <summary>
        /// Obtain the entry for the specified key, or the entries default value if the key is not found.
        /// </summary>
        /// <remarks>
        /// NOTE: If the key exists it will be promoted in the cache to be the most recently used entry.
        /// </remarks>
        /// <param name="key">The key for the entry to obtain.</param>
        /// <returns>The entry for the specified key.</returns>
        public TEntry GetEntry(TKey key)
        {
            if (!_cacheMap.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TEntry>> node))
                return default;

            _cacheList.Remove(node);
            _cacheList.AddFirst(node);
            return node.Value.Value;
        }

        /// <summary>
        /// Adds the specified entry for the specified key to the cache.
        /// </summary>
        /// <remarks>
        /// NOTE: If the key already exists its entry will be replaced by the new one.
        /// If the key does not exist then it will be added with the new entry.
        /// In all cases the new entry will be the most recently used entry.
        /// </remarks>
        /// <param name="key">The key for the entry to add.</param>
        /// <param name="entry">The entry to add.</param>
        public void AddEntry(TKey key, TEntry entry)
        {
            if (_cacheMap.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TEntry>> node))
            {
                _cacheList.Remove(node);
                node.Value = new KeyValuePair<TKey, TEntry>(key, entry);
                _cacheList.AddFirst(node);
                return;
            }

            if (_cacheList.Count == Capacity)
            {
                var leastUsedNode = _cacheList.Last;
                _cacheList.Remove(leastUsedNode);
                _cacheMap.Remove(leastUsedNode.Value.Key);
            }

            node = new LinkedListNode<KeyValuePair<TKey, TEntry>>(new KeyValuePair<TKey, TEntry>(key, entry));
            _cacheList.AddFirst(node);
            _cacheMap[key] = node;
        }

        /// <summary>
        /// Removes the key and its entry from the cache.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void RemoveEntry(TKey key)
        {
            if (!_cacheMap.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TEntry>> node))
                return;

            _cacheMap.Remove(key);
            _cacheList.Remove(node);
        }

        /// <summary>
        /// Returns true if the key exists in the cache; otherwise false.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key exists in the cache; otherwise false.</returns>
        public bool ContainsKey(TKey key) => _cacheMap.ContainsKey(key);

        /// <summary>
        /// Returns the most recently used key and entry in the cache.
        /// </summary>
        /// <returns>The most recently used key and entry in the cache.</returns>
        public (TKey Key, TEntry Entry) GetMostRecentlyUsedEntry()
        {
            var node = _cacheList.First;

            if (node == null)
                return (default, default);

            var kvp = node.Value;
            return (kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Returns the least recently used key and entry in the cache.
        /// </summary>
        /// <returns>The least recently used key and entry in the cache.</returns>
        public (TKey Key, TEntry Entry) GetLeastRecentlyUsedEntry()
        {
            var node = _cacheList.Last;

            if (node == null)
                return (default, default);

            var kvp = node.Value;
            return (kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Clears the cache of all data.
        /// </summary>
        public void Clear()
        {
            _cacheMap.Clear();
            _cacheList.Clear();
        }
    }
}

