using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
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
		var sysWin = WinUtils.MakeWin(opt);
		this.D(sysWin.D);
		sysWin.D(D);
		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(D);
		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);
		G.WinMan.AddWin(this);

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var mixRoot = WinUtils.BuildTree(treeEvtSig, treeEvtObs, this);
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


	public static MixNode BuildTree(
		ITreeEvtSig<IMixNode> treeEvtSig,
		ITreeEvtObs<IMixNode> treeEvtObs,
		Ctrl rootCtrl
	)
	{
		using var d = new Disp();
		var gfx = new Dummy_Gfx().D(d);
		var renderArgs = new RenderArgs(gfx, treeEvtSig).D(d);
		return treeEvtObs.ToTree(() =>
		{
			using (renderArgs.Ctrl(rootCtrl))
			{
			}
		});
	}


	public static MixLayout SolveTree(
		Win win,
		MixNode mixRoot,
		Sz winSz
	)
	{
		var stFlexRoot = mixRoot.OfTypeTree<IMixNode, StFlexNode>();
		var flexRoot = stFlexRoot.Map(e => e.Flex);
		var layout = FlexSolver.Solve(flexRoot, FreeSzMaker.FromSz(winSz));

		var flex2st = flexRoot.Zip(stFlexRoot).ToDictionary(e => e.First, e => e.Second.V.State);

		return new MixLayout(
			win,
			mixRoot,
			layout.RMap.MapKeys(flex2st),
			layout.WarningMap.MapKeys(flex2st)
		);
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
			gfx.R = rMap[st];
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