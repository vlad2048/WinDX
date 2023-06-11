using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
using RenderLib;
using RenderLib.Renderers.Dummy;
using SysWinLib;
using SysWinLib.Structs;
using TreePusherLib;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem;

public class WinOpt
{
	public string Title { get; set; } = string.Empty;
	public Pt? Pos { get; set; }
	public Sz? Size { get; set; }

	public R R
	{
		set
		{
			Pos = value.Pos;
			Size = value.Size;
		}
	}

	private WinOpt() { }

	public static WinOpt Build(Action<WinOpt>? optFun)
	{
		var opt = new WinOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}

public class Win : Ctrl
{
	public Win(Action<WinOpt>? optFun = null)
	{
		var opt = WinOpt.Build(optFun);
		var sysWin = WinUtilsLocal.MakeWin(opt).D(D);
		var (treeEvtSig, treeEvtObs) = TreeEvents<StFlexNode>.Make().D(D);
		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			StNode stRoot;

			// Retrieve the StFlexNode tree
			// ============================
			{
				using var subD = new Disp();
				var argsGfx = new RenderArgs(new TreePusher<StFlexNode>(treeEvtSig), new Dummy_Gfx());
				treeEvtObs.WhenPush.Subscribe(stNode =>
				{
					if (stNode.Ctrl.IsSome(out var ctrl))
						ctrl.SignalRender(argsGfx);
				}).D(subD);
				stRoot = treeEvtObs.ToTree(() => SignalRender(argsGfx));
			}

			// Compute its layout
			// ==================
			var root = stRoot.Map(e => e.Flex);
			var layout = FlexSolver.Solve(root, FreeSzMaker.FromSz(sysWin.ClientR.V.Size));
			var st2r = WinUtilsLocal.Build_St2R_Map(stRoot, root, layout);

			// Go through the tree and set every Ctrl.Win
			// ==========================================
			stRoot.Select(e => e.V.Ctrl).WhereSome().ForEach(ctrl => ctrl.WinRW.V = May.Some(this));

			{
				using var paintD = new Disp();
				var gfx = renderer.GetGfx().D(paintD);
				var argsGfx = new RenderArgs(new TreePusher<StFlexNode>(treeEvtSig), gfx);

				// Use the layout to update IGfx.R for each node
				// =============================================
				treeEvtObs.WhenPush.Subscribe(stNode =>
				{
					gfx.R = st2r[stNode.State];
					if (stNode.Ctrl.IsSome(out var ctrl))
						ctrl.SignalRender(argsGfx);
				}).D(paintD);

				// Render
				// ======
				SignalRender(argsGfx);
			}
		}).D(D);
	}
}



file static class WinUtilsLocal
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

	public static IReadOnlyDictionary<NodeState, R> Build_St2R_Map(
		StNode stRoot,
		Node root,
		Layout layout
	)
	{
		var st2node = root.Zip(stRoot).ToDictionary(e => e.Second.V.State, e => e.First);
		var map = new Dictionary<NodeState, R>();
		foreach (var (st, node) in st2node)
			map[st] = layout.RMap[node];
		return map;
	}
}