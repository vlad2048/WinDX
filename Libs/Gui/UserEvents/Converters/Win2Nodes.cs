using System.Reactive.Linq;
using DynamicData;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
#pragma warning disable CS8602

namespace UserEvents.Converters;


public static class UserEventConverter
{
	public static IDisposable MakeForNodes(
		IRoTracker<NodeZ> nodes,
		IObservable<IUserEvt> winEvt
	)
	{
		NodeZ[] HitFun(Pt pt) => nodes.ItemsArr.Where(e => e.Node.R.V.Contains(pt)).OrderBy(e => e.ZOrder).ToArray();

		var d = new Disp();
		HandleMouseMoves(out var mayHovVar, out var mouseFun, winEvt, HitFun).D(d);
		HandleMouseWheel(winEvt, HitFun, mouseFun).D(d);
		HandleMouseButtons(mayHovVar, winEvt).D(d);
		HandleNodeChanges(mayHovVar, mouseFun, nodes, HitFun).D(d);
		HandleMouseLeave(mayHovVar, winEvt).D(d);
		return d;
	}


	private static IDisposable HandleMouseMoves(
		out IRwMayVar<NodeZ> mayHovVar,
		out Func<Pt> mouseFun,
		IObservable<IUserEvt> winEvt,
		Func<Pt, NodeZ[]> hitFun
	)
	{
		var d = new Disp();

		var mayHovPrevVar = VarMay.Make<NodeZ>().D(d);
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

	private static IDisposable HandleMouseWheel(
		IObservable<IUserEvt> winEvt,
		Func<Pt, NodeZ[]> hitFun,
		Func<Pt> mouseFun
	)
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

	private static IDisposable HandleMouseButtons(
		IRoMayVar<NodeZ> mayHovVar,
		IObservable<IUserEvt> winEvt
	)
	{
		var d = new Disp();

		winEvt.Where(_ => mayHovVar.V.IsSome()).OfType<MouseButtonDownUserEvt>().Subscribe(mayHovVar.Send).D(d);
		winEvt.Where(_ => mayHovVar.V.IsSome()).OfType<MouseButtonUpUserEvt>().Subscribe(mayHovVar.Send).D(d);

		return d;
	}



	private static IDisposable HandleNodeChanges(
		IRwMayVar<NodeZ> mayHovVar,
		Func<Pt> mouseFun,
		IRoTracker<NodeZ> nodes,
		Func<Pt, NodeZ[]> hitFun
	)
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
				nodes.Items.ToUnit(),
				nodes.Items.MergeMany(e => e.Node.R).ToUnit()
			)
			.Subscribe(_ =>
			{
				CheckEnterLeave();
			}).D(d);


		return d;
	}



	private static IDisposable HandleMouseLeave(
		IRwMayVar<NodeZ> mayHovVar,
		IObservable<IUserEvt> winEvt
	)
	{
		var d = new Disp();

		winEvt.Where(_ => mayHovVar.V.IsSome()).OfType<MouseLeaveUserEvt>().Subscribe(e =>
		{
			mayHovVar.Send(e);
			mayHovVar.V = May.None<NodeZ>();
		}).D(d);

		return d;
	}



	private static void Send(this IRoMayVar<NodeZ> node, IUserEvt evt)
	{
		var v = node.V.Ensure();
		v.Send(evt);
	}

	private static void Send(this NodeZ[] nodes, IUserEvt evt)
	{
		foreach (var node in nodes)
		{
			if (node.Send(evt))
				break;
		}
	}

	private static bool Send(this NodeZ node, IUserEvt evt)
	{
		if (node.Node.D.IsDisposed) return false;
		var evtTr = evt.TranslateMouse(-node.Node.R.V.Pos);
		node.Node.DispatchEvt(evtTr);
		return evtTr.Handled;
	}
}