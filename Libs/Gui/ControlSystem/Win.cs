using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
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

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var mixRoot = WinUtils.BuildTree(treeEvtSig, treeEvtObs, this);
			WinUtils.AssignWinToCtrls(this, mixRoot);
			var mixLayout = WinUtils.SolveTree(this, mixRoot, sysWin.ClientR.V.Size);
			G.WinMan.SetWinLayout(mixLayout);
			var gfx = WinUtils.BuildGfxWithRMap(mixLayout.RMap, treeEvtObs, renderer).D(d);
			WinUtils.Render(gfx, treeEvtSig, this).D(d);
		}).D(D);
	}

	public override string ToString() => GetType().Name;
}



file static class WinUtils
{
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


	public static ReconstructedTree<IMixNode> BuildTree(
		ITreeEvtSig<IMixNode> treeEvtSig,
		ITreeEvtObs<IMixNode> treeEvtObs,
		Ctrl rootCtrl
	)
	{
		using var d = new Disp();
		var gfx = new Dummy_Gfx().D(d);
		var renderArgs = new RenderArgs(gfx, treeEvtSig).D(d);
		var reconstructedTree = treeEvtObs.ToTree(() =>
		{
			using (renderArgs.Ctrl(rootCtrl))
			{
			}
		});
		return reconstructedTree;
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


	public static MixLayout SolveTree(
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

		return new MixLayout(
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

	public static (IGfx, IDisposable) BuildGfxWithRMap(
		IReadOnlyDictionary<NodeState, R> rMap,
		ITreeEvtObs<IMixNode> treeEvtObs,
		IRenderWinCtx renderer
	)
	{
		var d = new Disp();
		var gfx = renderer.GetGfx().D(d);
		treeEvtObs.WhenPush.OfType<StFlexNode, NodeState>(e => e.State).Subscribe(st =>
		{
			gfx.R = rMap.TryGetValue(st, out var r) switch
			{
				true => r,
				false => R.Empty
			};
		}).D(d);
		return (gfx, d);
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
}