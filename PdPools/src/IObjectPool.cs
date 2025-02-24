namespace PdPools;

public interface IObjectPool<T> where T : class
{
  public int CountInactive { get; }

  public T Get();

  public PooledObject<T> Get(out T v);

  public void Release(T element);

  public void Clear();
}
