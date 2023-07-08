using System.Reactive.Linq;
using PowBasics.Geom;
using UserEvents.Structs;

namespace UserEvents.Utils;

public static class UserEvtTransforms
{
	public static IObservable<IUserEvt> Translate(this IObservable<IUserEvt> obs, Func<Pt> ofsFun) => obs.Select(e => e.TranslateMouse(ofsFun()));
}