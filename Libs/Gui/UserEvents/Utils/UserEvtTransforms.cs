using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Utils;

static class UserEvtTransforms
{
	public static IUIEvt Map(
		this IUIEvt uiEvt,
		Func<IObservable<IUserEvt>, IObservable<IUserEvt>> mapFun
	)
		=> new UIEvt(
			uiEvt.WinHandle,
			mapFun(uiEvt.Evt)
		);


	public static IUIEvt Translate(
		this IUIEvt uiEvt,
		Func<Pt> ofsFun
	)
		=> uiEvt.Map(
			evt => evt.Select(e => e.TranslateMouse(ofsFun()))
		);


	public static IUIEvt MakeHot(
		this IUIEvt uiEvt,
		IRoDispBase d
	)
		=> new UIEvt(
			uiEvt.WinHandle,
			uiEvt.Evt.MakeHot(d)
		);
}