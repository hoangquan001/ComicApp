using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

public static class ListPool<T>
{
    private static readonly ConcurrentBag<List<T>> _pool = new ConcurrentBag<List<T>>();
    private static readonly Timer _timer;

    private static readonly int MaxPoolSize = 1000;

    static ListPool()
    {
        _timer = new Timer(TrimPool, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
    }

    private static void TrimPool(object? state)
    {
        if (_pool.Count > MaxPoolSize / 2) // Chỉ trim nếu vượt ngưỡng
        {
            int count = _pool.Count / 2;
            for (int i = 0; i < count; i++)
            {
                // Remove List from pool
                _pool.TryTake(out List<T>? list);
            }
        }
    }

    public static List<T> Rent()
    {
        if (_pool.TryTake(out List<T>? list))
        {
            return list;
        }

        return new List<T>();
    }

    public static void Return(List<T> list)
    {
        if (_pool.Count < MaxPoolSize)
        {
            list.Clear();
            _pool.Add(list);
        }
    }
}

