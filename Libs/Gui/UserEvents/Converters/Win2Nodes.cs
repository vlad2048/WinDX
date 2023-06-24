using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
using UserEvents.Utils;

namespace UserEvents.Converters;


public static class UserEventConverter
{
	public static IDisposable MakeForNodes<N>(
		IUIEvt winEvt,
		IObservable<IChangeSet<N>> nodes,
		Func<Pt, Maybe<N>> hitFun
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();

		var nodesList = nodes.AsObservableList().D(d);

		var hovNode = VarMay.Make<N>().D(d);
		var mouse = new Pt(-int.MaxValue, -int.MaxValue);

		void CheckEnterLeave()
		{
			var hovNodeNext = hitFun(mouse);
			var isHovPrev = hovNode.V.IsSome(out var hovPrev);
			var isHovNext = hovNodeNext.IsSome(out var hovNext);
			switch (isHovPrev, isHovNext)
			{
				case (false, true):
					hovNext.EvtSrc.
					break;
			}
		}


		return d;
	}


	//private static IObservable<N>





	private static bool Is<T>(this Maybe<T> may, T obj) => may.IsSome(out var val) && val.Equals(obj);



	public static IDisposable MakeForNodes_Old<N>(
		IUIEvt winEvt,
		IObservable<IChangeSet<N>> nodes,
		Func<Pt, Maybe<N>> hitFun
	)
		where N : INodeStateUserEventsSupport
	{
		var d = new Disp();
		nodes
			.Transform(node =>
			{
				var nodeD = new Disp();

				var isOver = Var.Make(false).D(nodeD);
				ISubject<IUserEvt> whenLeave = new Subject<IUserEvt>().D(nodeD);
				

				// ***********************************
				// * Synthesize the MouseLeave event *
				// ***********************************
				isOver
					.DistinctUntilChanged()
					.Select(_isOver => _isOver switch
					{
						false => winEvt.WhenMouseMove().Where(pt => hitFun(pt).Is(node)).Select(_ => true),
						true =>
							Obs.Merge(
									winEvt.WhenMouseLeave().ToUnit(),
									winEvt.WhenMouseMove().Where(pt => !hitFun(pt).Is(node)).ToUnit()
								)
								.Select(_ => false)
					})
					.Switch()
					.DistinctUntilChanged()
					.Subscribe(isOverNext =>
					{
						if (!isOverNext)
							whenLeave.OnNext(new MouseLeaveUserEvt());
						isOver.V = isOverNext;
					}).D(nodeD);

				
				// ***********************
				// * Generate the events *
				// ***********************
				node.EvtSrc.V = winEvt
					.Map(evt =>
						{
							var evtNoLeaveAndNoMove = evt.Where(e => e is not MouseLeaveUserEvt && e is not MouseMoveUserEvt);
							var mouseMoveInside = evt.Where(e => e is MouseMoveUserEvt mv && hitFun(mv.Pos).Is(node));

							return Obs.Merge(
								
									//evtNoLeaveAndNoMove
									//	.Where(e => e is IUserEvtKeyboard)
									//	.Where(_ => focusFun(area).Foc.V.HasFocus()),
									whenLeave
										.AsObservable(),
									evtNoLeaveAndNoMove
										.Where(e => e is IUserEvtMouse)
										.Where(_ => isOver.V),
									mouseMoveInside
								)
								;
						}
					)
					.Translate(() => -node.R.V.Pos)
					
					.MakeHot(nodeD);

				return nodeD;
			})
			.DisposeMany()
			.MakeHot(d);
		return d;
	}
}