namespace PdPools;

using System;
using System.Collections.Generic;

/// <summary>
///   <para>A stack based Pool.IObjectPool_1.</para>
/// </summary>
public class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
{
  private readonly List<T> _list;
  private readonly Func<T> _createFunc;
  private readonly Action<T> _actionOnGet;
  private readonly Action<T> _actionOnRelease;
  private readonly Action<T> _actionOnDestroy;
  private readonly int _maxSize;
  private readonly bool _collectionCheck;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public int CountAll { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public int CountActive => CountAll - CountInactive;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public int CountInactive => _list.Count;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public ObjectPool(
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    Func<T> createFunc,
    Action<T> actionOnGet = null,
    Action<T> actionOnRelease = null,
    Action<T> actionOnDestroy = null,
    bool collectionCheck = true,
    int defaultCapacity = 10,
    int maxSize = 10000)
  {
    if (maxSize <= 0)
    {
      throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));
    }

    _list = new List<T>(defaultCapacity);
    _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
    _maxSize = maxSize;
    _actionOnGet = actionOnGet;
    _actionOnRelease = actionOnRelease;
    _actionOnDestroy = actionOnDestroy;
    _collectionCheck = collectionCheck;
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public T Get()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  {
    T obj;
    var index = _list.Count - 1;
    if (_list.Count == 0)
    {
      obj = _createFunc();
      ++CountAll;
    }
    else
    {
      obj = _list[index];
      _list.RemoveAt(index);
    }
    _actionOnGet?.Invoke(obj);
    return obj;
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public PooledObject<T> Get(out T v)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  {
    return new PooledObject<T>(v = Get(), this);
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Release(T element)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  {
    if (_collectionCheck && _list.Count > 0)
    {
      foreach (var el in _list)
      {
        if (element == el)
        {
          throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
        }
      }
    }

    _actionOnRelease?.Invoke(element);
    if (CountInactive < _maxSize)
    {
      _list.Add(element);
    }
    else
    {
      _actionOnDestroy?.Invoke(element);
    }
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Clear()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  {
    if (_actionOnDestroy != null)
    {
      foreach (var obj in _list)
      {
        _actionOnDestroy(obj);
      }
    }
    _list.Clear();
    CountAll = 0;
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Dispose() => Clear();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
