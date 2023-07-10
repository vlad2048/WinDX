using PowRxVar;
using NUnit.Framework;
using PowBasics.CollectionsExt;

namespace TestBase;

public class ObservableChecker<T> : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly string name;
	private readonly Func<T, T> evtDisplayFun;
	private readonly bool noLoggingIfEmpty;
	private readonly List<T> list = new();

	public ObservableChecker(IObservable<T> obs, string name, bool noLoggingIfEmpty = false, Func<T, T>? evtDisplayFun = null)
	{
		obs.Subscribe(list.Add).D(d);
		this.name = name;
		this.noLoggingIfEmpty = noLoggingIfEmpty;
		this.evtDisplayFun = evtDisplayFun ?? (_ => _);
	}

	public T[] Clear()
	{
		var res = list.ToArray();
		list.Clear();
		return res;
	}

	public void Check(params T[] expArr)
	{
		var actArr = list.ToArray();
		if (noLoggingIfEmpty && expArr.Length == 0 && actArr.Length == 0) return;

		actArr = actArr.SelectToArray(evtDisplayFun);
		expArr = expArr.SelectToArray(evtDisplayFun);

		var isSame = expArr.Length == actArr.Length && expArr.Zip(actArr).All(t => t.First!.Equals(t.Second));
		if (isSame)
		{
			LArr(expArr, $"{name} events");
		}
		else
		{
			LArr(expArr, $"Expected {name} events");
			LArr(actArr, $"Actual   {name} events");
		}

		CollectionAssert.AreEqual(expArr, actArr);

		list.Clear();
	}
}