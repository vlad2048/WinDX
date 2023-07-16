using System.Reactive.Linq;
using System.Windows.Forms;
using DynamicData;
using Microsoft.VisualBasic.Devices;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
using UserEvents.Utils;

#pragma warning disable CS8604

#pragma warning disable CS8602

namespace UserEvents.Converters;


public static class UserEventConverter
{
	public static IDisposable MakeForNodes(
		out IRoMayVar<INode> nodeLockN,
		out IRoMayVar<INode> nodeHovrN,
		IRoTracker<NodeZ> nodes,
		IObservable<IUserEvt> winEvt
	)
	{
		var d = new Disp();
		NodeZ[] HitFun(Pt pt) => nodes.ItemsArr.OrderBy(e => e.ZOrder).WhereToArray(e => e.Node.R.V.Contains(pt));

		var nodeLockRw = VarMay.Make<NodeZ>().D(d);
		var nodeLock = nodeLockRw.ToReadOnlyMay();
		nodeLockN = nodeLock.SelectVarMay(e => e.Node);

		var nodeHovrRw = VarMay.Make<NodeZ>().D(d);
		var nodeHovr = nodeHovrRw.ToReadOnlyMay();
		nodeHovrN = nodeHovr.SelectVarMay(e => e.Node);

		//nodeHovrN.Log("Hovr: ").D(d);
		//nodeLockN.LogBW("Lock <- ").D(d);

		HandleMouseMoves(
			winEvt,
			out var mouseFun,
			nodeHovrRw,
			HitFun,
			nodeLock
		).D(d);

		HandleMouseWheel(
			winEvt,
			HitFun,
			mouseFun
		).D(d);

		HandleMouseButtons(
			winEvt,
			nodeHovr,
			nodeLockRw,
			HitFun
		).D(d);

		HandleNodeChanges(
			nodeHovrRw,
			nodeLock,
			mouseFun,
			nodes,
			HitFun
		).D(d);

		HandleMouseLeave(
			winEvt,
			nodeHovrRw
		).D(d);

		return d;
	}


	private static IDisposable HandleMouseMoves(
		IObservable<IUserEvt> winEvt,
		out Func<Pt> mouseFun,
		IRwMayVar<NodeZ> nodeHovrRw,
		Func<Pt, NodeZ[]> hitFun,
		IRoMayVar<NodeZ> nodeLock
	)
	{
		var d = new Disp();

		var mouse = new Pt(-int.MaxValue, -int.MaxValue);

		mouseFun = () => mouse;

		/*var trig = 0;
		winEvt.WhenKeyDown(Keys.T).Subscribe(_ =>
		{
			trig = 1;
			L($"Trig <- {trig}");
		}).D(d);
		winEvt.WhenMouseUp().Where(_ => trig == 1).Subscribe(_ =>
		{
			trig = 2;
			L($"Trig <- {trig}");
		}).D(d);*/

		winEvt.WhenMouseMove().Subscribe(mouse_ =>
		{
			mouse = mouse_;

			/*if (trig == 2)
			{
				var abc = 123;
				trig = 0;
			}*/

			if (nodeLock.V.IsSome(out var nodeLockVal))
			{
				nodeLockVal.Send(new MouseMoveUserEvt(mouse));
			}
			else
			{
				var mayHovNext = hitFun(mouse).FirstOrMaybe();
				var isHovPrev = nodeHovrRw.V.IsSome(out var hovPrev);
				var isHovNext = mayHovNext.IsSome(out var hovNext);
				/*var eqStr = (isHovPrev, isHovNext) switch
				{
					(true, true) => $" eq:{hovNext.Equals(hovPrev)}",
					_ => ""
				};
				L($"(isHovPrev, isHovNext) = ({isHovPrev}, {isHovNext}){eqStr}");*/
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

				if (mayHovNext.AreDifferent(nodeHovrRw.V))
				{
					//L("HovrChange (HandleMouseMoves)");
					nodeHovrRw.V = mayHovNext;
				}
			}

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
		IObservable<IUserEvt> winEvt,
		IRoMayVar<NodeZ> nodeHovr,
		IRwMayVar<NodeZ> nodeLock,
		Func<Pt, NodeZ[]> hitFun
	)
	{
		var d = new Disp();

		winEvt.Where(_ => nodeHovr.V.IsSome()).OfType<MouseButtonDownUserEvt>().Subscribe(evt =>
		{
			var hov = nodeHovr.V.Ensure();
			nodeLock.V = May.Some(hov);
			hov.Send(evt);
		}).D(d);

		winEvt.Where(_ => nodeHovr.V.IsSome()).OfType<MouseButtonUpUserEvt>().Subscribe(evt =>
		{
			var hov = nodeHovr.V.Ensure();

			var hovNext = hitFun(evt.Pos).FirstOrMaybe();



			
			nodeLock.V = May.None<NodeZ>();
			hov.Send(evt);
		}).D(d);

		return d;
	}



	private static IDisposable HandleNodeChanges(
		IRwMayVar<NodeZ> nodeHovrRw,
		IRoMayVar<NodeZ> nodeLock,
		Func<Pt> mouseFun,
		IRoTracker<NodeZ> nodes,
		Func<Pt, NodeZ[]> hitFun
	)
	{
		var d = new Disp();


		void CheckEnterLeave()
		{
			var mouse = mouseFun();

			if (nodeLock.V.IsNone())
			{
				var hovNodeNext = hitFun(mouse).FirstOrMaybe();
				var isHovPrev = nodeHovrRw.V.IsSome(out var hovPrev);
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

				if (hovNodeNext.AreDifferent(nodeHovrRw.V))
				{
					//L("HovrChange (HandleNodeChanges)");
					nodeHovrRw.V = hovNodeNext;
				}
			}
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
		IObservable<IUserEvt> winEvt,
		IRwMayVar<NodeZ> nodeHovrRw
	)
	{
		var d = new Disp();

		winEvt.Where(_ => nodeHovrRw.V.IsSome()).OfType<MouseLeaveUserEvt>().Subscribe(e =>
		{
			var hov = nodeHovrRw.V.Ensure();
			hov.Send(e);

			var mayHovNext = May.None<NodeZ>();
			if (mayHovNext.AreDifferent(nodeHovrRw.V))
			{
				//L("HovrChange (HandleMouseLeave)");
				nodeHovrRw.V = mayHovNext;
			}
		}).D(d);

		return d;
	}
	


	private static bool Send(this NodeZ node, IUserEvt evt)
	{
		var n = node.Node;
		if (n.D.IsDisposed) return false;
		n.DispatchEvt(evt);
		return evt.Handled;
	}

	// sends MouseWheel event to all hit nodes
	private static void Send(this NodeZ[] nodes, IUserEvt evt)
	{
		foreach (var node in nodes)
		{
			if (node.Send(evt))
				break;
		}
	}


	private static bool AreDifferent<T>(this Maybe<T> ma, Maybe<T> mb) => (ma.IsSome(out var a), mb.IsSome(out var b)) switch
	{
		(true, true) => !a.Equals(b),
		(false, false) => false,
		_ => true
	};
}