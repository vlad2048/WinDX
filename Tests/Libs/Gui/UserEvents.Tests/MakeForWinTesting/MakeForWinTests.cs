using PowBasics.Geom;
using PowRxVar;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.MakeForWinTesting.TestSupport;
using static UserEvents.Tests.MakeForWinTesting.TestSupport.GeomData;
using static UserEvents.Tests.MakeForWinTesting.MakeForWinTestsUtils;
#pragma warning disable CS8618

namespace UserEvents.Tests.MakeForWinTesting;

sealed class MakeForWinTests : RxTest
{
	[Test]
	public void _00_Basics()
	{
		Send_Move(Dst.Main, pMain);

		Check(
			Enter(pMain),
			Move(pMain)
		);

		Send_ActivateApp(Dst.Pop1);
		Send_ActivateApp(Dst.Pop0);
		Send_ActivateApp(Dst.Main);

		Check(
			ActivateApp()
		);
	}


	private MakeForWinTester tester;
	
	private void Check(params IUserEvt[] evts) => tester.Check(evts);

	private void Send_ActivateApp(Dst dst) => tester.Send_ActivateApp(dst);
	private void Send_InactivateApp(Dst dst) => tester.Send_InactivateApp(dst);
	private void Send_Activate(Dst dst, bool withMouseClick) => tester.Send_Activate(dst, withMouseClick);
	private void Send_Inactivate(Dst dst) => tester.Send_Inactivate(dst);
	private void Send_GotFocus(Dst dst) => tester.Send_GotFocus(dst);
	private void Send_LostFocus(Dst dst) => tester.Send_LostFocus(dst);
	private void Send_BtnDown(Dst dst) => tester.Send_BtnDown(dst);
	private void Send_BtnUp(Dst dst) => tester.Send_BtnUp(dst);
	private void Send_Move(Dst dst, Pt pt) => tester.Send_Move(dst, pt);
	private void Send_Leave(Dst dst) => tester.Send_Leave(dst);


	[SetUp]
	public new void Setup()
	{
		ResetUtilsMousePos();
		tester = new MakeForWinTester().D(D);
		Check();
	}
}



file static class MakeForWinTestsUtils
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

    public static IUserEvt ActivateApp() => new ActivateAppUserEvt();
    public static IUserEvt InactivateApp() => new InactivateAppUserEvt();
    public static IUserEvt Activate(bool withMouseClick) => new ActivateUserEvt(withMouseClick);
    public static IUserEvt Inactivate() => new InactivateUserEvt();
    public static IUserEvt GotFocus() => new GotFocusUserEvt();
    public static IUserEvt LostFocus() => new LostFocusUserEvt();
}