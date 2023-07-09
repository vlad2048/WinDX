/*
using DynamicData;
using PowRxVar;

namespace UserEvents;


public interface IRoMap<K, V> where K : notnull
{
	Disp D { get; }
	IObservable<IChangeSet<V, K>> Map { get; }
	IObservableCache<V, K> MapCache { get; }
}

public interface IRwMap<K, V> : IRoMap<K, V>, IDisposable where K : notnull
{
	IObservableCache<V, K> Src { get; }
}

public class RxMap
{
	
}
*/