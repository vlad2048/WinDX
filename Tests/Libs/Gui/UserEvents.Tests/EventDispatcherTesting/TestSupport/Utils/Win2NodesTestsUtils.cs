using System.Windows.Forms;
using PowBasics.Geom;
using UserEvents.Structs;

namespace UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;


static class Win2NodesTestsUtils
{
	private static Pt mousePos = Pt.Empty;

	public static void ResetUtilsMousePos() => mousePos = Pt.Empty;


	public static IUserEvt Enter(Pt pt)
	{
		mousePos = pt;
		return new MouseEnterUserEvt(pt);
	}
	public static IUserEvt Leave() => new MouseLeaveUserEvt();
	public static IUserEvt Move(Pt pt)
	{
		mousePos = pt;
		return new MouseMoveUserEvt(pt);
	}
	public static IUserEvt BtnDown() => new MouseButtonDownUserEvt(mousePos, MouseBtn.Left);
	public static IUserEvt BtnUp() => new MouseButtonUpUserEvt(mousePos, MouseBtn.Left);
	public static IUserEvt KeyDown() => new KeyDownUserEvt(Keys.K);
	public static IUserEvt KeyUp() => new KeyUpUserEvt(Keys.K);




	public static NodeEvt Enter(NodeZ node, Pt pt)
	{
		mousePos = pt;
		return new NodeEvt(node.Node, new MouseEnterUserEvt(pt));
	}
	public static NodeEvt Leave(NodeZ node) => new(node.Node, new MouseLeaveUserEvt());
	public static NodeEvt Move(NodeZ node, Pt pt)
	{
		mousePos = pt;
		return new NodeEvt(node.Node, new MouseMoveUserEvt(pt));
	}
	public static NodeEvt BtnDown(NodeZ node) => new(node.Node, new MouseButtonDownUserEvt(mousePos, MouseBtn.Left));
	public static NodeEvt BtnUp(NodeZ node) => new(node.Node, new MouseButtonUpUserEvt(mousePos, MouseBtn.Left));
	public static NodeEvt KeyDown(NodeZ node) => new(node.Node, new KeyDownUserEvt(Keys.K));
	public static NodeEvt KeyUp(NodeZ node) => new(node.Node, new KeyUpUserEvt(Keys.K));
}