using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Popup_.Structs;
using PowBasics.ColorCode;
using PowBasics.ColorCode.Utils;
using PowBasics.Geom;
using RenderLib.Renderers;
using PowRxVar;
using RenderLib.Structs;
using TreePusherLib;
using ControlSystem.Structs;
using Microsoft.VisualBasic.ApplicationServices;

namespace ControlSystem.Logic.Rendering_;



public sealed class RenderArgs : IGfx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISubject<Ctrl> whenCtrlPushPrev;
	private readonly ISubject<Ctrl> whenCtrlPushNext;
	private readonly ISubject<Ctrl> whenCtrlPopPrev;
	private readonly ISubject<Ctrl> whenCtrlPopNext;
	private readonly ISubject<StFlexNode> whenFlexPushPrev;
	private readonly ISubject<StFlexNode> whenFlexPushNext;
	private readonly ISubject<StFlexNode> whenFlexPopPrev;
	private readonly ISubject<StFlexNode> whenFlexPopNext;
	private readonly ISubject<(string, Color?)> whenDraw;

	public TreePusher<IMixNode> Pusher { get; }
	public IObservable<Ctrl> WhenCtrlPushPrev => whenCtrlPushPrev.AsObservable();
	public IObservable<Ctrl> WhenCtrlPushNext => whenCtrlPushNext.AsObservable();
	public IObservable<Ctrl> WhenCtrlPopPrev => whenCtrlPopPrev.AsObservable();
	public IObservable<Ctrl> WhenCtrlPopNext => whenCtrlPopNext.AsObservable();
	public IObservable<StFlexNode> WhenFlexPushPrev => whenFlexPushPrev.AsObservable();
	public IObservable<StFlexNode> WhenFlexPushNext => whenFlexPushNext.AsObservable();
	public IObservable<StFlexNode> WhenFlexPopPrev => whenFlexPopPrev.AsObservable();
	public IObservable<StFlexNode> WhenFlexPopNext => whenFlexPopNext.AsObservable();
	public IObservable<(string, Color?)> WhenDraw => whenDraw.AsObservable();

	public RenderStFlexNodeFluent this[NodeState nodeState] => new(this, nodeState);

	public IGfx Gfx { get; }

	internal RenderArgs(
		IGfx gfx,
		TreePusher<IMixNode> pusher
	)
	{
		Gfx = gfx;
		Pusher = pusher;

		whenCtrlPushPrev = new Subject<Ctrl>().D(d);
		whenCtrlPushNext = new Subject<Ctrl>().D(d);
		whenCtrlPopPrev = new Subject<Ctrl>().D(d);
		whenCtrlPopNext = new Subject<Ctrl>().D(d);

		whenFlexPushPrev = new Subject<StFlexNode>().D(d);
		whenFlexPushNext = new Subject<StFlexNode>().D(d);
		whenFlexPopPrev = new Subject<StFlexNode>().D(d);
		whenFlexPopNext = new Subject<StFlexNode>().D(d);

		whenDraw = new Subject<(string, Color?)>().D(d);
	}


	// *************
	// * Push Ctrl *
	// *************
	public IDisposable this[Ctrl ctrl] {
		get {
			whenCtrlPushPrev.OnNext(ctrl);
			var disp = Pusher.Push(new CtrlNode(ctrl));
			whenCtrlPushNext.OnNext(ctrl);

			return Disposable.Create(() =>
			{
				whenCtrlPopPrev.OnNext(ctrl);
				disp.Dispose();
				whenCtrlPopNext.OnNext(ctrl);
			});
		}
	}

	// *************
	// * Push Flex *
	// *************
	internal IDisposable Flex(StFlexNode f)
	{
		//f.State.SetNameIFN($"{f.Flex}");

		whenFlexPushPrev.OnNext(f);
		var disp = Pusher.Push(f);
		whenFlexPushNext.OnNext(f);

		return Disposable.Create(() => {
			whenFlexPopPrev.OnNext(f);
			disp.Dispose();
			whenFlexPopNext.OnNext(f);
		});
	}


	public void DrawText(string text, FontDef font, Color color)
	{
		var sz = Gfx.MeasureText_(text, font);
		Pusher.Push(new TextMeasureNode(sz)).Dispose();
		Gfx.DrawText_(text, font, color);
	}



	// IGfx
	// ====
	public R R { get => Gfx.R; set => Gfx.R = value; }

	public bool DrawDisabled => Gfx.DrawDisabled;

	private void EvtDraw(string str, Color? col = null)
	{
		if (DrawDisabled) return;
		whenDraw.OnNext((str, col));
	}

	public void PushClip(R clipR)
	{
		EvtDraw($"PushClip {clipR}");
		Gfx.PushClip(clipR);
	}

	public void PopClip()
	{
		EvtDraw("Pop");
		Gfx.PopClip();
	}

	public void FillR(R r, BrushDef brush)
	{
		EvtDraw($"FillR {r}", brush.GetColor());
		Gfx.FillR(r, brush);
	}

	public void DrawR(R r, PenDef pen)
	{
		EvtDraw($"DrawR {r}");
		Gfx.DrawR(r, pen);
	}

	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
		EvtDraw($"DrawLine {a}-{b}");
		Gfx.DrawLine(a, b, penDef);
	}

	public void DrawBmp(Bitmap bmp)
	{
		EvtDraw("DrawBmp");
		Gfx.DrawBmp(bmp);
	}

	public Sz MeasureText_(string text, FontDef fontDef) => Gfx.MeasureText_(text, fontDef);

	public void DrawText_(string text, FontDef fontDef, Color color)
	{
		EvtDraw($"DrawText '{text}'");
		Gfx.DrawText_(text, fontDef, color);
	}
}





file static class RenderArgsExt
{
	public static Color? GetColor(this BrushDef brush) => brush switch
	{
		SolidBrushDef { Color: var color } => color,
		_ => null
	};
}





/*
public sealed class RenderArgs : IGfx
{
	private static readonly Color c = Color.White;

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IGfx gfx;
	private readonly TreePusher<IMixNode> pusher;
	private readonly Partition part;
	private readonly RenderLogger lgr;

	internal RenderArgs(
		IGfx gfx,
		TreePusher<IMixNode> pusher,
		bool log,
		Partition part
	)
	{
		this.gfx = gfx;
		this.pusher = pusher;
		this.part = part;
		lgr = new RenderLogger(log, part);
		lgr.Start();
		Disposable.Create(lgr.Finish).D(d);
	}

	public RenderStFlexNodeFluent this[NodeState nodeState] => new(this, nodeState);

	public IDisposable this[Ctrl ctrl] {
		get {
			lgr.PushCtrl(ctrl);
			var disp = pusher.Push(new CtrlNode(ctrl));
			if (part != null!)
			{
				if (part.CtrlsToRender.Contains(ctrl)) ctrl.SignalRender(this);
			}

			return Disposable.Create(() =>
			{
				lgr.PopCtrl(ctrl);
				disp.Dispose();
			});
		}
	}

	internal IDisposable Flex(StFlexNode f)
	{
		f.State.SetNameIFN($"{f.Flex}");
		lgr.PushFlex(f);
		var disp = pusher.Push(f);
		return Disposable.Create(() => {
			lgr.PopFlex(f);
			disp.Dispose();
		});
	}


	public void DrawText(string text, FontDef font, Color color)
	{
		var sz = gfx.MeasureText_(text, font);
		pusher.Push(new TextMeasureNode(sz)).Dispose();
		gfx.DrawText_(text, font, color);
	}



	// IGfx
	// ====
	public R R { get => gfx.R; set => gfx.R = value; }

	public void PushClip(R clipR)
	{
		lgr.Draw($"PushClip {clipR}");
		gfx.PushClip(clipR);
	}

	public void PopClip()
	{
		lgr.Draw("Pop");
		gfx.PopClip();
	}

	public void FillR(R r, BrushDef brush)
	{
		lgr.Draw($"FillR {r}");
		gfx.FillR(r, brush);
	}

	public void DrawR(R r, PenDef pen)
	{
		lgr.Draw($"DrawR {r}");
		gfx.DrawR(r, pen);
	}

	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
		lgr.Draw($"DrawLine {a}-{b}");
		gfx.DrawLine(a, b, penDef);
	}

	public void DrawBmp(Bitmap bmp)
	{
		gfx.DrawBmp(bmp);
	}

	public Sz MeasureText_(string text, FontDef fontDef) => gfx.MeasureText_(text, fontDef);

	public void DrawText_(string text, FontDef fontDef, Color color)
	{
		lgr.Draw($"DrawText '{text}'");
		gfx.DrawText_(text, fontDef, color);
	}
}
*/
