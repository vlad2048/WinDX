using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Utils;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;
using UserEvents.Structs;
using WinAPI.User32;

namespace Demos.Categories.Ownership;

sealed class DetachCtrlDemo : Win
{
	private enum State
	{
		Attached,
		Detached,
		Disposed,
	}

	public DetachCtrlDemo() : base(opt => opt.R = new(-550, 50, 500, 250))
	{
		var nRoot = new NodeState().D(D);
		Ctrl? ctrlDetach = new DetachCtrlRoot();
		var ctrlNormal = new NormalCtrlRoot().D(D);


		var state = Var.Make(State.Attached).D(D);
		var serD = new SerialDisp<Disp>().D(D);
		Win? winDetached = null;

		// Toggle Attached (if not disposed)
		Evt.WhenKeyDown(VirtualKey.A).Subscribe(_ =>
		{
			switch (state.V)
			{
				case State.Attached:
					if (serD.Value != null || winDetached != null) throw new ArgumentException("Impossible");
					serD.Value = null;
					serD.Value = new Disp();
					winDetached = new DetachedWin(ctrlDetach).D(serD.Value);
					User32Methods.SetForegroundWindow(Handle);
					state.V = State.Detached;
					break;

				case State.Detached:
					if (serD.Value == null || winDetached == null) throw new ArgumentException("Impossible");
					serD.Value = null;
					winDetached = null;
					state.V = State.Attached;
					break;
			}
		}).D(D);

		// Dispose (if not disposed)
		Evt.WhenKeyDown(VirtualKey.D).Subscribe(_ =>
		{
			if (state.V == State.Disposed) return;

			serD.Value = null;
			ctrlDetach.Dispose();
			ctrlDetach = null;
			winDetached = null;

			state.V = State.Disposed;

		}).D(D);

		// Create (if disposed)
		Evt.WhenKeyDown(VirtualKey.C).Subscribe(_ =>
		{
			if (state.V != State.Disposed) return;
			if (serD.Value != null || winDetached != null || ctrlDetach != null) throw new ArgumentException("Impossible");
			ctrlDetach = new DetachCtrlRoot();

			state.V = State.Attached;

		}).D(D);


		state.Subscribe(s =>
		{
			L($"State <- {s}");
			InvalidateAll();
		}).D(D);



		WhenRender.Subscribe(r =>
		{
			using (r[nRoot].StratStack(Dir.Horz).M)
			{
				r.Gfx.FillR(C.BrushWinBack);
				if (state.V == State.Attached)
					using (r[ctrlDetach]) { }
				using (r[ctrlNormal]) { }
			}
		}).D(D);
	}



	private sealed class DetachedWin : Win
	{
		public DetachedWin(Ctrl ctrl) : base(opt => opt.R = new(-550, 350, 300, 250))
		{
			var nRoot = new NodeState().D(D);

			WhenRender.Subscribe(r =>
			{
				using (r[nRoot].StratStack(Dir.Horz).M)
				{
					r.Gfx.FillR(C.BrushWinDetachBack);
					using (r[ctrl]) { }
				}
			}).D(D);
		}
	}




	private sealed class DetachCtrlRoot : Ctrl
	{
		public DetachCtrlRoot()
		{
			var nRoot = new NodeState().D(D);
			var c1 = new DetachCtrlChild().D(D);
			var c2 = new DetachCtrlChild().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r[nRoot].Dim(300, 250).StratStack(Dir.Horz).Marg(5).M)
				{
					r.Gfx.FillR(C.BrushDetachCtrlRoot);
					using (r[c1]) { }
					using (r[c2]) { }
				}
			}).D(D);
		}
	}
	private sealed class DetachCtrlChild : Ctrl
	{
		public DetachCtrlChild()
		{
			var nRoot = new NodeState().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r[nRoot].Marg(20, 40).M)
				{
					r.Gfx.FillR(C.BrushDetachCtrlChild);
				}
			}).D(D);
		}
	}


	private sealed class NormalCtrlRoot : Ctrl
	{
		public NormalCtrlRoot()
		{
			var nRoot = new NodeState().D(D);
			var c1 = new NormalCtrlChild().D(D);
			var c2 = new NormalCtrlChild().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r[nRoot].Dim(200, 250).StratStack(Dir.Vert, Align.Middle).Marg(5).M)
				{
					r.Gfx.FillR(C.BrushNormalCtrlRoot);
					using (r[c1]) { }
					using (r[c2]) { }
				}
			}).D(D);
		}
	}
	private sealed class NormalCtrlChild : Ctrl
	{
		public NormalCtrlChild()
		{
			var nRoot = new NodeState().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r[nRoot].Dim(80, 30).Marg(0, 20).M)
				{
					r.Gfx.FillR(C.BrushNormalCtrlChild);
				}
			}).D(D);
		}
	}


	private static class C
	{
		public static readonly BrushDef BrushWinBack = new SolidBrushDef(Color.FromArgb(2, 17, 41));
		public static readonly BrushDef BrushWinDetachBack = new SolidBrushDef(Color.FromArgb(3, 38, 8));

		public static readonly BrushDef BrushDetachCtrlRoot = new SolidBrushDef(Color.FromArgb(163, 173, 21));
		public static readonly BrushDef BrushDetachCtrlChild = new SolidBrushDef(Color.FromArgb(218, 227, 84));

		public static readonly BrushDef BrushNormalCtrlRoot = new SolidBrushDef(Color.FromArgb(24, 88, 148));
		public static readonly BrushDef BrushNormalCtrlChild = new SolidBrushDef(Color.FromArgb(79, 156, 227));
	}
}