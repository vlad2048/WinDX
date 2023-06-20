using System.Diagnostics;
using System.Reactive.Linq;
using ControlSystem.Logic.PopLogic;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using Logging;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
using RenderLib;
using RenderLib.Renderers;
using RenderLib.Renderers.Dummy;
using Structs;
using SysWinLib;
using SysWinLib.Structs;
using TreePusherLib;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem;


public class Win : Ctrl
{
	public Win(Action<WinOpt>? optFun = null)
	{
		var opt = WinOpt.Build(optFun);
		var sysWin = WinUtils.MakeWin(opt).D(D);
		this.D(sysWin.D);

		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(D);
		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);
		G.WinMan.AddWin(this);


		SlaveWin[]? slaveWins = null;
		const int SlaveCnt = 2560;

		sysWin.WhenMsg.WhenKEYDOWN().Where(e => e.Key == VirtualKey.D1).Subscribe(_ =>
		{
			slaveWins = new SlaveWin[SlaveCnt];
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < SlaveCnt; i++)
				slaveWins[i] = new SlaveWin(sysWin.Handle).D(D);
			L($"Init time: {sw.Elapsed.TotalMilliseconds:F3}ms");
		}).D(D);

		sysWin.WhenMsg.WhenKEYDOWN().Where(e => e.Key == VirtualKey.D2).Subscribe(_ =>
		{
			if (slaveWins == null) return;

			var sw = Stopwatch.StartNew();
			for (var i = 0; i < SlaveCnt; i++)
				User32Methods.ShowWindow(slaveWins[i].Handle, ShowWindowCommands.SW_SHOW);
			L($"Show time: {sw.Elapsed.TotalMilliseconds:F3}ms");

		}).D(D);

		sysWin.WhenMsg.WhenKEYDOWN().Where(e => e.Key == VirtualKey.D3).Subscribe(_ =>
		{
			if (slaveWins == null) return;

			var sw = Stopwatch.StartNew();
			for (var i = 0; i < SlaveCnt; i++)
				slaveWins[i].Dispose();
			L($"Dispose time: {sw.Elapsed.TotalMilliseconds:F3}ms");
			slaveWins = null;

		}).D(D);

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			WinUtils.BuildTree(out var mixRoot, treeEvtSig, treeEvtObs, this);
			WinUtils.AssignWinToCtrls(this, mixRoot);
			WinUtils.SolveTree(out var mixLayout, this, mixRoot, sysWin.ClientR.V.Size);
			G.WinMan.SetWinLayout(mixLayout);

			var (mainPartition, slavePartitions) = PopSplitter.Split(mixLayout.MixRoot, mixLayout.RMap);

			//WinUtils.BuildGfxWithRMap(out var gfx, mixLayout.RMap, treeEvtObs, renderer).D(d);
			//WinUtils.Render(gfx, treeEvtSig, this).D(d);
		}).D(D);
	}

	public override string ToString() => GetType().Name;
}



file static class WinUtils
{
	/*
	public static IDisposable RenderSimple(
		MixLayout layout,
		ITreeEvtObs<IMixNode> treeEvtObs,
		ITreeEvtSig<IMixNode> treeEvtSig,
		IRenderWinCtx renderer,
		Win win
	)
	{
		var d = new Disp();
		var gfx = renderer.GetGfx().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var renderArgs = new RenderArgs(gfx, pusher).D(d);

		treeEvtObs.WhenPush.OfType<CtrlNode, Ctrl>(e => e.Ctrl).Subscribe(ctrl =>
		{
			ctrl.SignalRender(renderArgs);
		}).D(d);

		treeEvtObs.WhenPush.OfType<StFlexNode, NodeState>(e => e.State).Subscribe(st =>
		{
			gfx.R = layout.RMap.TryGetValue(st, out var r) switch
			{
				true => r,
				false => R.Empty
			};
		}).D(d);


		using (renderArgs.Ctrl(win))
		{

		}


		return d;
	}
	*/



	private const int DEFAULT = (int)CreateWindowFlags.CW_USEDEFAULT;

	public static SysWin MakeWin(WinOpt opt)
	{
		var win = new SysWin(e =>
		{
			e.CreateWindowParams = new CreateWindowParams
			{
				Name = opt.Title,
				X = opt.Pos?.X ?? DEFAULT,
				Y = opt.Pos?.Y ?? DEFAULT,
				Width = opt.Size?.Width ?? DEFAULT,
				Height = opt.Size?.Height ?? DEFAULT,
			};
		});
		win.Init();
		return win;
	}



	public static void BuildTree(
		out ReconstructedTree<IMixNode> reconstructedTree,
		ITreeEvtSig<IMixNode> treeEvtSig,
		ITreeEvtObs<IMixNode> treeEvtObs,
		Ctrl rootCtrl
	)
	{
		using var d = new Disp();
		var gfx = new Dummy_Gfx().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var renderArgs = new RenderArgs(gfx, pusher).D(d);
		reconstructedTree = treeEvtObs.ToTree(
			mixNode =>
			{
				if (mixNode is not CtrlNode { Ctrl: var ctrl }) return;
				//using var __ = Nst.Log($"WhenPush->SignalRender {ctrl.GetType().Name}");
				ctrl.SignalRender(renderArgs);
			},
			() =>
			{
				using (renderArgs.Ctrl(rootCtrl)) { }
			}
		);
	}


	public static void AssignWinToCtrls(
		Win win,
		ReconstructedTree<IMixNode> reconstructedTree
	) =>
		reconstructedTree.Root
			.Select(e => e.V)
			.OfType<CtrlNode>()
			.SelectToArray(e => e.Ctrl)
			.ForEach(ctrl => ctrl.WinRW.V = May.Some(win));


	public static void SolveTree(
		out MixLayout mixLayout,
		Win win,
		ReconstructedTree<IMixNode> reconstructedTree,
		Sz winSz
	)
	{
		var mixRoot = reconstructedTree.Root;

		var stFlexRoot = mixRoot.OfTypeTree<IMixNode, StFlexNode>();
		var flexRoot = stFlexRoot.Map(e => e.Flex);
		var freeSz = FreeSzMaker.FromSz(winSz);
		var layout = FlexSolver.Solve(flexRoot, freeSz);

		var flex2st = flexRoot.Zip(stFlexRoot).ToDictionary(e => e.First, e => e.Second.V.State);

		mixLayout = new MixLayout(
			win,
			freeSz,
			mixRoot,
			layout.RMap.MapKeys(flex2st),
			layout.WarningMap.MapKeys(flex2st),

			reconstructedTree.IncompleteNodes
				.GroupBy(e => e.ParentNod)
				.Select(e => e.First())
				.ToDictionary(
					e => e.ParentNod.FindCtrlContaining(),
					e => e.ChildNode
				),

			mixRoot
				.Where(e => e.V is CtrlNode)
				.ToDictionary(
					e => ((CtrlNode)e.V).Ctrl,
					e => e
				)
		);
	}

	private static Ctrl FindCtrlContaining(this MixNode nod)
	{
		while (nod.V is not CtrlNode { Ctrl: not null })
			nod = nod.Parent ?? throw new ArgumentException("Cannot find containing Ctrl");
		return ((CtrlNode)nod.V).Ctrl;
	}

	/*
	public static IDisposable BuildGfxForPopSubTree(
		out IGfx gfx,
		PopSubTree tree,
		ITreeEvtObs<IMixNode> treeEvtObs,
		IRenderWinCtx renderer
	)
	{
		var d = new Disp();
		gfx = renderer.GetGfx().D(d);
		var gfx_ = gfx;



		return d;
	}


	
	public static IDisposable BuildGfxWithRMap(
		out IGfx gfx,
		IReadOnlyDictionary<NodeState, R> rMap,
		ITreeEvtObs<IMixNode> treeEvtObs,
		IRenderWinCtx renderer
	)
	{
		var d = new Disp();
		gfx = renderer.GetGfx().D(d);
		var gfx_ = gfx;
		treeEvtObs.WhenPush.OfType<StFlexNode, NodeState>(e => e.State).Subscribe(st =>
		{
			gfx_.R = rMap.TryGetValue(st, out var r) switch
			{
				true => r,
				false => R.Empty
			};
		}).D(d);
		return d;
	}


	public static IDisposable Render(
		IGfx gfx,
		ITreeEvtSig<IMixNode> treeEvtSig,
		Ctrl rootCtrl
	)
	{
		var d = new Disp();
		var renderArgs = new RenderArgs(gfx, treeEvtSig).D(d);
		using (renderArgs.Ctrl(rootCtrl))
		{
		}
		return d;
	}
	*/
}