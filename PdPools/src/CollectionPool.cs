namespace PdPools;

using System;
using System.Collections.Generic;

/// <summary>
///   <para>A Collection such as List, HashSet, Dictionary etc can be pooled and reused by using a CollectionPool.</para>
/// </summary>
public class CollectionPool<TCollection, TItem> : IDisposable where TCollection : class, ICollection<TItem>, new()
{
  internal static readonly ObjectPool<TCollection> _pool = new(() => [], actionOnRelease: l => l.Clear());

  public static TCollection Get() => _pool.Get();

  public static PooledObject<TCollection> Get(out TCollection value)
  {
    return _pool.Get(out value);
  }

  public static void Release(TCollection toRelease)
  {
    _pool.Release(toRelease);
  }

  public void Dispose() => _pool.Clear();
}
