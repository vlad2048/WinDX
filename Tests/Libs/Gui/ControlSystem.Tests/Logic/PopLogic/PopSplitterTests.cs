using ControlSystem.Logic.PopupLogic;
using ControlSystem.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using PowTrees.Algorithms;
using Shouldly;
using TestBase;
#pragma warning disable CS8618

namespace ControlSystem.Tests.Logic.PopLogic;

sealed class PopSplitterTests : RxTest
{
	/*
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──C3──<p3>──C4──n5

	==============================================

	Partition[0]
	------------
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤
	           └──⬤

	Partition[1]
	------------
	                    ┌──n3──⬤
	              <p1>──┤
	                    └──C3──⬤

	Partition[2]
	------------
	                           <p2>──n4

	Partition[3]
	------------
	                           <p3>──C4──n5

	*/
	[Test]
	public void _01_PartitionPopNodes()
	{
		LogMixTree(tree, "Tree", 0);
		L("");

		CheckPartitions(
			tree.PartitionPopNodes(),

			C(c1,
				N(n1),
				N(n2,
					C(c2)
				)
			),

			NPop(p1,
				N(n3),
				C(c3)
			),

			NPop(p2,
				N(n4)
			),

			NPop(p3,
				C(c4,
					N(n5)
				)
			)
		);
	}





	/*
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──C3──<p3>──C4──n5
	==============================================

	Partition[0] (root)
	------------
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤
	           └──⬤
	--extend-->
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──⬤

	Partition[1] (<p1>)
	------------
	                    ┌──n3──⬤
	              <p1>──┤
	                    └──C3──⬤
	--extend-->
	    ┌──n1
	C1──┤      ┌──⬤
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──C3──<p3>──⬤

	Partition[2] (<p2>)
	------------
	                           <p2>──n4
	--extend-->
	    ┌──n1
	C1──┤      ┌──⬤
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──⬤

	Partition[3] (<p3>)
	------------
	                           <p3>──C4──n5
	--extend-->
	                       C3──<p3>──C4──n5

	*/
	[Test]
	public void _02_ExtendToControls()
	{
		LogMixTree(tree, "Tree", 0);
		L("");

		var backMap = tree.ToDictionary(e => e.V);

		CheckPartitions(
			tree.PartitionPopNodes()
				.SelectToArray(e => e
					.ExtendToCtrlUp(backMap)
					.ExtendToCtrlDown(backMap)
				),

			C(c1,
				N(n1),
				N(n2,
					C(c2),
					NPop(p1,
						N(n3,
							NPop(p2,
								N(n4)
							)
						)
					)
				)
			),

			C(c1,
				N(n1),
				N(n2,
					NPop(p1,
						N(n3,
							NPop(p2,
								N(n4)
							)
						),
						C(c3,
							NPop(p3)
						)
					)
				)
			),

			C(c1,
				N(n1),
				N(n2,
					NPop(p1,
						N(n3,
							NPop(p2,
								N(n4)
							)
						)
					)
				)
			),

			C(c3,
				NPop(p3,
					C(c4,
						N(n5)
					)
				)
			)
		);
	}


	[Test]
	public void _03_Split()
	{
		const bool T = true;
		const bool F = false;

		CheckLayoutPartitions(
			PopupSplitter.Split(tree, rMap),

			Cb(c1,
				Nb(n1, T),
				Nb(n2, T,
					Cb(c2),
					NPopb(p1, F,
						Nb(n3, F,
							NPopb(p2, F,
								Nb(n4, F)
							)
						)
					)
				)
			),

			Cb(c1,
				Nb(n1, F),
				Nb(n2, F,
					NPopb(p1, T,
						Nb(n3, T,
							NPopb(p2, F,
								Nb(n4, F)
							)
						),
						Cb(c3,
							NPopb(p3, F)
						)
					)
				)
			),

			Cb(c1,
				Nb(n1, F),
				Nb(n2, F,
					NPopb(p1, F,
						Nb(n3, F,
							NPopb(p2, T,
								Nb(n4, T)
							)
						)
					)
				)
			),

			Cb(c3,
				NPopb(p3, T,
					Cb(c4,
						Nb(n5, T)
					)
				)
			)
		);

	}



	private void CheckLayoutPartitions(
		PartitionSet actPartitionSet,
		params TNod<MixOnOff>[] expLayoutPartitions
	)
	{
		var actLayoutPartitionsWithMaps = actPartitionSet.SubPartitions.Prepend(actPartitionSet.MainPartition).ToArray();
		var actLayoutPartitions = actLayoutPartitionsWithMaps.SelectToArray(ConvertLayoutPartition);
		LogLayoutPartitions(expLayoutPartitions, "Expected");
		LogLayoutPartitions(actLayoutPartitions, "Actual");

		actLayoutPartitions.Length.ShouldBe(expLayoutPartitions.Length);

		for (var i = 0; i < actLayoutPartitions.Length; i++)
		{
			var actLayoutPartition = actLayoutPartitions[i];
			var expLayoutPartition = expLayoutPartitions[i];
			actLayoutPartition.ShouldBeSameTree(expLayoutPartition);
		}
	}

	private static TNod<MixOnOff> ConvertLayoutPartition(Partition partition) =>
		partition.Root.Map(node => new MixOnOff(
			node,
			node switch
			{
				StFlexNode { State: var state } => partition.RMap.ContainsKey(state),
				CtrlNode => true,
				_ => false
			}
		));

	private void LogLayoutPartitions(TNod<MixOnOff>[] roots, string title)
	{
		LTitle($"{title} layout partitions ({roots.Length})");
		for (var i = 0; i < roots.Length; i++)
		{
			var str = $"partition[{i}]";
			L($"  {str}");
			L($"  {new string('-', str.Length)}");
			LogOnOffMixTree(roots[i], null, 4);
			L("");
		}
		L("");
	}



	private void CheckPartitions(MixNode[] actPartitions, params MixNode[] expPartitions)
	{
		LogPartitions(expPartitions, "Expected");
		LogPartitions(actPartitions, "Actual");

		actPartitions.Length.ShouldBe(expPartitions.Length);

		for (var i = 0; i < actPartitions.Length; i++)
		{
			var actPartition = actPartitions[i];
			var expPartition = expPartitions[i];
			actPartition.ShouldBeSameTree(expPartition);
		}
	}

	private void LogPartitions(MixNode[] partitions, string title)
	{
		LTitle($"{title} partitions ({partitions.Length})");
		for (var i = 0; i < partitions.Length; i++)
		{
			var str = $"partition[{i}]";
			L($"  {str}");
			L($"  {new string('-', str.Length)}");
			LogMixTree(partitions[i], null, 4);
			L("");
		}
		L("");
	}





	private Disp d;
	private MixNode tree;
	private IReadOnlyDictionary<NodeState, R> rMap;
	private NodeState n1, n2, n3, n4, n5, p1, p2, p3;
	private Ctrl c1, c2, c3, c4;

	private string Fmt(IMixNode n) => n switch
	{
		StFlexNode { State: var s } when s == n1 => "n1",
		StFlexNode { State: var s } when s == n2 => "n2",
		StFlexNode { State: var s } when s == n3 => "n3",
		StFlexNode { State: var s } when s == n4 => "n4",
		StFlexNode { State: var s } when s == n5 => "n5",
		StFlexNode { State: var s } when s == p1 => "p1",
		StFlexNode { State: var s } when s == p2 => "p2",
		StFlexNode { State: var s } when s == p3 => "p3",
		CtrlNode { Ctrl: var c } when c == c1 => "C1",
		CtrlNode { Ctrl: var c } when c == c2 => "C2",
		CtrlNode { Ctrl: var c } when c == c3 => "C3",
		CtrlNode { Ctrl: var c } when c == c4 => "C4",
		_ => "Unkown!"
	};

	[SetUp]
	public new void Setup()
	{
		d = new Disp();
		(n1, n2, n3, n4, n5, p1, p2, p3) = NodeStateMaker.Make8(d);
		(c1, c2, c3, c4) = CtrlMaker.Make4(d);

		tree =
			C(c1,
				N(n1),
				N(n2,
					C(c2),
					NPop(p1,
						N(n3,
							NPop(p2,
								N(n4)
							)
						),
						C(c3,
							NPop(p3,
								C(c4,
									N(n5)
								)
							)
						)
					)
				)
			);

		rMap = new[] { n1, n2, n3, n4, n5, p1, p2, p3 }
			.ToDictionary(e => e, _ => R.Empty);
	}

	[TearDown]
	public new void Teardown()
	{
		d.Dispose();
	}

	private void LogMixTree(MixNode root, string? title, int spaceCount)
	{
		if (title != null)
			LTitle(title);
		var lines = root.LogToStrings(opt => opt.FormatFun = Fmt);
		var pad = new string(' ', spaceCount);
		foreach (var line in lines)
			L($"{pad}{line}");
	}

	private void LogOnOffMixTree(TNod<MixOnOff> root, string? title, int spaceCount)
	{
		if (title != null)
			LTitle(title);
		var lines = root.LogToStrings(opt => opt.FormatFun = e => $"{Fmt(e.MixNode)}/{e.Enabled}");
		var pad = new string(' ', spaceCount);
		foreach (var line in lines)
			L($"{pad}{line}");
	}





	/*
	─
	│
	┌
	└
	┤
	    ┌──n1
	    │
	C1──┤
	    │      ┌──C2
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──C3──<p3>──C4──n5

	--------------------------------------------

	root
	----
	    ┌──n1
	    │  ▀▀
	 C1─┤
	 ▀▀ │      ┌──C2
	    └──n2──┤  ▀▀    ┌──n3──<p2>──n4
	       ▀▀  └─▎<p1>──┤
	                    └──⬤

	<p1>
	----
	    ┌──n1
	    │
	C1──┤
	    │      ┌──C2
	    └──n2──┤        ┌──n3─▎<p2>──n4
	           └──<p1>──┤  ▀▀
	               ▀▀   └──C3─▎<p3>──C4──n5
	                       ▀▀
	<p2>
	----
	    ┌──n1
	    │
	C1──┤
	    │      ┌──⬤
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤       ▀▀   ▀▀
	                    └──⬤

	<p3>
	----
                           C3──<p3>──C4──n5
	                            ▀▀   ▀▀  ▀▀


	 */


	/*
	[Test]
	public static void _00_Basic()
	{
		using var d = new Disp();
		var (n1, n2, n3, n4, n5, p1, p2, p3) = NodeStateMaker.Make8(d);
		var (c1, c2, c3, c4) = CtrlMaker.Make4(d);

		string Fmt(IMixNode n) => n switch
		{
			StFlexNode { State: var s } when s == n1 => "n1",
			StFlexNode { State: var s } when s == n2 => "n2",
			StFlexNode { State: var s } when s == n3 => "n3",
			StFlexNode { State: var s } when s == n4 => "n4",
			StFlexNode { State: var s } when s == n5 => "n5",
			StFlexNode { State: var s } when s == p1 => "p1",
			StFlexNode { State: var s } when s == p2 => "p2",
			StFlexNode { State: var s } when s == p3 => "p3",
			CtrlNode { Ctrl: var c } when c == c1 => "C1",
			CtrlNode { Ctrl: var c } when c == c2 => "C2",
			CtrlNode { Ctrl: var c } when c == c3 => "C3",
			CtrlNode { Ctrl: var c } when c == c4 => "C4",
			_ => "Unkown!"
		};

		var root =
			C(c1,
				N(n1),
				N(n2,
					C(c2),
					NPop(p1,
						N(n3,
							NPop(p2,
								N(n4)
							)
						),
						C(c3,
							NPop(p3,
								C(c4,
									N(n5)
								)
							)
						)
					)
				)
			);

		void LogTree(MixNode r, string title)
		{
			L(title);
			L(new string('=', title.Length));
			L(r.LogToString(opt => opt.FormatFun = Fmt));
			L("");
		}

		LogTree(root, "Main tree");
	}

	private static void L(string s) => Console.WriteLine(s);
	*/

	private sealed record MixOnOff(
		IMixNode MixNode,
		bool Enabled
	);

	private static TNod<MixOnOff> Cb(Ctrl ctrl, params TNod<MixOnOff>[] kids) => Nod.Make(new MixOnOff(new CtrlNode(ctrl), true), kids);
	private static TNod<MixOnOff> Nb(NodeState nodeState, bool on, params TNod<MixOnOff>[] kids) => Nod.Make(new MixOnOff(new StFlexNodeFluent(nodeState).StBuild(), on), kids);
	private static TNod<MixOnOff> NPopb(NodeState nodeState, bool on, params TNod<MixOnOff>[] kids) => Nod.Make(new MixOnOff(new StFlexNodeFluent(nodeState).Pop().StBuild(), on), kids);



	private static MixNode C(Ctrl ctrl, params MixNode[] kids) => Nod.Make(new CtrlNode(ctrl), kids);
	private static MixNode N(NodeState nodeState, params MixNode[] kids) => Nod.Make(new StFlexNodeFluent(nodeState).StBuild(), kids);
	private static MixNode NPop(NodeState nodeState, params MixNode[] kids) => Nod.Make(new StFlexNodeFluent(nodeState).Pop().StBuild(), kids);
}

file static class CtrlMaker
{
	public static (Ctrl, Ctrl, Ctrl, Ctrl) Make4(IRoDispBase d)
	{
		var c0 = new Ctrl().D(d);
		var c1 = new Ctrl().D(d);
		var c2 = new Ctrl().D(d);
		var c3 = new Ctrl().D(d);
		return (c0, c1, c2, c3);
	}
}
