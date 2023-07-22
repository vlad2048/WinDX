using System.Collections.Concurrent;
using System.Drawing;
using ControlSystem.Structs;
using PowBasics.ColorCode;
using PowBasics.ColorCode.Structs;
using PowBasics.Geom;
using PowTrees.Algorithms;

namespace ControlSystem.Logic.Popup_.Structs;

static class PartitionSetPrettyPrinter
{
	public static Txt PrettyPrint(this PartitionSet set)
	{
		var w = new TxtWriter();
		w.WriteLine();
		var ctrlNames = set.GetCtrlNames();
		w.Write(set.PrintFullTree(ctrlNames));
		w.Write(set.PrintPartitionTree(ctrlNames));
		return w.Txt;
	}


	private static Txt PrintFullTree(
		this PartitionSet set,
		IReadOnlyDictionary<Ctrl, string> ctrlNames
	)
	{
		void WriteNode(IMixNode node, ITxtWriter t)
		{
			switch (node)
			{
				case CtrlNode { Ctrl: var ctrl }:
				{
					t.Write(ctrlNames[ctrl], C.CtrlInSet);
					t.SurroundWith(AsciiBoxes.Curved, C.CtrlBox);
					break;
				}

				case StFlexNode { State: var state }:
				{
					var r = set.RMap[state];
					t.Write($"{state}", C.FlexInSet);
					if (set.ExtraCtrlsToRenderOnPop.TryGetValue(state, out var extraCtrls))
					{
						t.Write($" (+{extraCtrls.Length} extras)", C.FlexInSet);
					}
					t.WriteLine();
					t.WriteLine($"{r}", C.FlexR);
					break;
				}

				default:
					throw new ArgumentException("Impossible");
			}
		}

		var w = new TxtWriter();
		w.WriteLine();
		w.Write(
			set.Root.LogColored(WriteNode, opt =>
			{
			})
		);
		return w.Txt;
	}


	private static Txt PrintPartitionTree(
		this PartitionSet set,
		IReadOnlyDictionary<Ctrl, string> ctrlNames
	)
	{
		var partitionNames = set.Partitions
			.Select((p, i) => (p, i switch
			{
				0 => " MainPartition ",
				_ => $" SubPartition {i - 1} "
			}))
			.ToDictionary(e => e.p, e => e.Item2);

		void WriteNode(IMixNode node, ITxtWriter t, Partition partition)
		{
			switch (node)
			{
				case CtrlNode { Ctrl: var ctrl }:
				{
					var isRoot = partition.IsRoot(ctrl);
					var isRender = ctrl == partition.RenderCtrl;
					var isIn = partition.Ctrls.Contains(ctrl);
					if (isRoot) t.Write("(R) ", C.RootNode);
					t.Write(ctrlNames[ctrl], isIn ? C.CtrlInSet : C.CtrlNotInSet);
					t.SurroundWith(AsciiBoxes.Curved, C.CtrlBox, isRender ? "render" : null);
					break;
				}

				case StFlexNode { State: var state }:
				{
					var isRoot = partition.IsRoot(state);
					var isIn = partition.NodeStates.Contains(state);
					if (isRoot) t.Write("(R) ", C.RootNode);
					t.Write($"{state}", isIn ? C.FlexInSet : C.FlexNotInSet);
					break;
				}

				default:
					throw new ArgumentException("Impossible");
			}
		}

		void WritePartition(Partition partition, ITxtWriter t)
		{
			var txt = set.Root.LogColored((n, wr) => WriteNode(n, wr, partition), opt =>
			{

			});
			t.Write(txt);
			t.SurroundWith(AsciiBoxes.Double, C.PartitionBox, partitionNames[partition]);
		}

		var w = new TxtWriter();
		w.WriteLine();
		w.Write(
			set.PartitionRoot.LogColored(WritePartition, opt =>
			{
				opt.GutterSz = new Sz(6, 2);
				opt.ArrowColor = C.PartitionArrow;
			})
		);
		return w.Txt;
	}



	public static class C
	{
		public static readonly Color PartitionArrow = Color.White;
		public static readonly Color PartitionBox = Color.FromArgb(38, 184, 68);

		public static readonly Color RootNode = Color.FromArgb(230, 14, 57);
		public static readonly Color CtrlInSet = Color.FromArgb(70, 159, 226);
		public static readonly Color CtrlNotInSet = Color.FromArgb(40, 85, 118);
		public static readonly Color CtrlBox = Color.FromArgb(135, 193, 237);

		public static readonly Color FlexInSet = Color.FromArgb(201, 67, 215);
		public static readonly Color FlexNotInSet = Color.FromArgb(86, 25, 93);
		public static readonly Color FlexR = Color.FromArgb(0xcc, 0xe6, 0x51);
	}


	private static bool IsRoot(this Partition part, Ctrl ctrl) => part.RootNode == part.Set.Lookups.Ctrl2Nod[ctrl];
	private static bool IsRoot(this Partition part, NodeState state) => part.RootNode == part.Set.Lookups.NodeState2Nod[state];


	private static IReadOnlyDictionary<Ctrl, string> GetCtrlNames(this PartitionSet set)
	{
		var allCtrls =
			set.Root.GetAllCtrls()
				.Concat(
					from ctrls in set.ExtraCtrlsToRenderOnPop.Values
					from ctrl in ctrls
					select ctrl
				)
				.Concat(
					from part in set.Partitions
					from ctrl in part.CtrlsToRender
					select ctrl
				)
				.Distinct()
				.ToArray();

		var cntMap = new Dictionary<string, int>();

		string GetCtrlName(Ctrl ctrl)
		{
			var ctrlName = ctrl.GetType().Name;
			if (cntMap.TryGetValue(ctrlName, out var cnt))
			{
				cntMap[ctrlName]++;
				ctrlName += $"_{cnt}";
			}
			else
			{
				cntMap[ctrlName] = 1;
			}

			return ctrlName;
		}

		var ctrlNames = new Dictionary<Ctrl, string>();

		foreach (var ctrl in allCtrls)
			ctrlNames[ctrl] = GetCtrlName(ctrl);

		return ctrlNames;
	}
}