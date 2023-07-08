using System.Reactive.Linq;
using DynamicData;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
#pragma warning disable CS8602
#pragma warning disable CS8631

namespace UserEvents.Converters;


public static class UserEventConverter
{
	public static IDisposable MakeForNodes<N>(
		IObservable<IChangeSet<N>> nodes,
		IObservable<IUserEvt> winEvt,
		Func<Pt, N[]> hitFun
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();
		HandleMouseMoves(out var mayHovVar, out var mouseFun, winEvt, hitFun).D(d);
		HandleMouseWheel(winEvt, hitFun, mouseFun).D(d);
		HandleMouseButtons(mayHovVar, winEvt).D(d);
		HandleNodeChanges(mayHovVar, mouseFun, nodes, hitFun).D(d);
		HandleMouseLeave(mayHovVar, winEvt).D(d);
		return d;
	}


	private static IDisposable HandleMouseMoves<N>(
		out IRwMayVar<N> mayHovVar,
		out Func<Pt> mouseFun,
		IObservable<IUserEvt> winEvt,
		Func<Pt, N[]> hitFun
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();

		var mayHovPrevVar = VarMay.Make<N>().D(d);
		mayHovVar = mayHovPrevVar;

		var mouse = new Pt(-int.MaxValue, -int.MaxValue);

		mouseFun = () => mouse;

		winEvt.WhenMouseMove().Subscribe(mouse_ =>
		{
			mouse = mouse_;

			var mayHovNext = hitFun(mouse).FirstOrMaybe();
			var isHovPrev = mayHovPrevVar.V.IsSome(out var hovPrev);
			var isHovNext = mayHovNext.IsSome(out var hovNext);
			switch (isHovPrev, isHovNext)
			{
				case (false, true):
					hovNext.Send(new MouseEnterUserEvt(mouse));
					hovNext.Send(new MouseMoveUserEvt(mouse));
					break;
				case (true, false):
					hovPrev.Send(new MouseLeaveUserEvt());
					break;
				case (true, true) when !hovNext.Equals(hovPrev):
					hovPrev.Send(new MouseLeaveUserEvt());
					hovNext.Send(new MouseEnterUserEvt(mouse));
					hovNext.Send(new MouseMoveUserEvt(mouse));
					break;
				case (true, true):
					hovNext.Send(new MouseMoveUserEvt(mouse));
					break;
			}

			mayHovPrevVar.V = mayHovNext;

		}).D(d);


		return d;
	}

	private static IDisposable HandleMouseWheel<N>(
		IObservable<IUserEvt> winEvt,
		Func<Pt, N[]> hitFun,
		Func<Pt> mouseFun
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();

		winEvt
			.OfType<MouseWheelUserEvt>()
			.Subscribe(e =>
			{
				var pos = mouseFun();
				var nodes = hitFun(pos).Distinct().ToArray();
				var evt = e with { Pos = pos };
				nodes.Send(evt);
			}).D(d);

		return d;
	}

	private static IDisposable HandleMouseButtons<N>(
		IRoMayVar<N> mayHovVar,
		IObservable<IUserEvt> winEvt
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();

		winEvt.Where(_ => mayHovVar.V.IsSome()).OfType<MouseButtonDownUserEvt>().Subscribe(mayHovVar.Send).D(d);
		winEvt.Where(_ => mayHovVar.V.IsSome()).OfType<MouseButtonUpUserEvt>().Subscribe(mayHovVar.Send).D(d);

		return d;
	}



	private static IDisposable HandleNodeChanges<N>(
		IRwMayVar<N> mayHovVar,
		Func<Pt> mouseFun,
		IObservable<IChangeSet<N>> nodes,
		Func<Pt, N[]> hitFun
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();


		void CheckEnterLeave()
		{
			var mouse = mouseFun();
			var hovNodeNext = hitFun(mouse).FirstOrMaybe();
			var isHovPrev = mayHovVar.V.IsSome(out var hovPrev);
			var isHovNext = hovNodeNext.IsSome(out var hovNext);
			switch (isHovPrev, isHovNext)
			{
				case (false, true):
					hovNext.Send(new MouseEnterUserEvt(mouse));
					hovNext.Send(new MouseMoveUserEvt(mouse));
					break;
				case (true, false):
					hovPrev.Send(new MouseLeaveUserEvt());
					break;
				case (true, true) when !hovNext.Equals(hovPrev):
					hovPrev.Send(new MouseLeaveUserEvt());
					hovNext.Send(new MouseEnterUserEvt(mouse));
					hovNext.Send(new MouseMoveUserEvt(mouse));
					break;
			}

			mayHovVar.V = hovNodeNext;
		}

		Obs.Merge(
				nodes.ToUnit(),
				nodes.MergeMany(e => e.R).ToUnit()
			)
			.Subscribe(_ =>
			{
				CheckEnterLeave();
			}).D(d);


		return d;
	}



	private static IDisposable HandleMouseLeave<N>(
		IRwMayVar<N> mayHovVar,
		IObservable<IUserEvt> winEvt
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();

		winEvt.Where(_ => mayHovVar.V.IsSome()).OfType<MouseLeaveUserEvt>().Subscribe(e =>
		{
			mayHovVar.Send(e);
			mayHovVar.V = May.None<N>();
		}).D(d);

		return d;
	}



	private static void Send<N>(this IRoMayVar<N> node, IUserEvt evt) where N : INodeStateUserEventsSupport
	{
		var v = node.V.Ensure();
		v.Send(evt);
	}

	private static void Send<N>(this N[] nodes, IUserEvt evt) where N : INodeStateUserEventsSupport
	{
		foreach (var node in nodes)
		{
			if (node.Send(evt))
				break;
		}
	}

	private static bool Send<N>(this N node, IUserEvt evt) where N : INodeStateUserEventsSupport
	{
		if (node.D.IsDisposed) return false;
		var evtTr = evt.TranslateMouse(-node.R.V.Pos);
		node.DispatchEvt(evtTr);
		return evtTr.Handled;
	}
}