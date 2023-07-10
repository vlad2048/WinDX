using System.Drawing;
using System.Reactive.Disposables;
using PowBasics.ColorCode;
using PowBasics.ColorCode.Utils;
using PowBasics.Geom;
using RenderLib.Renderers;
using PowRxVar;
using RenderLib.Structs;
using TreePusherLib;

namespace ControlSystem.Structs;


public sealed class RenderArgs : IGfx, IDisposable
{
	private static readonly Color c = Color.White;

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IGfx gfx;
	private readonly TreePusher<IMixNode> pusher;
	private readonly TxtWriter? w;

	internal RenderArgs(IGfx gfx, TreePusher<IMixNode> pusher, bool log, string title)
	{
		this.gfx = gfx;
		this.pusher = pusher;
		w = log switch
		{
			false => null,
			true => new TxtWriter()
		};
		w?.WriteLine();
		title = $"Rendering {title}";
		w?.WriteLine(title, c);
		w?.WriteLine(new string('-', title.Length), c);
		Disposable.Create(() =>
		{
			if (w == null) return;
			w.Txt.PrintToConsole();
		}).D(d);
	}

	public RenderStFlexNodeFluent this[NodeState nodeState] => new(this, nodeState);

	public IDisposable this[Ctrl ctrl]
	{
		get
		{
			w?.Push();
			var disp = pusher.Push(new CtrlNode(ctrl));
			return Disposable.Create(() =>
			{
				w?.Pop();
				disp.Dispose();
			});
		}
	}

	internal IDisposable Flex(StFlexNode f)
	{
		f.State.SetNameIFN($"{f.Flex}");
		w?.Push();
		var disp = pusher.Push(f);
		return Disposable.Create(() =>
		{
			w?.Pop();
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
		w?.WriteLine($"PushClip {clipR}", c);
		gfx.PushClip(clipR);
	}

	public void PopClip()
	{
		w?.WriteLine("Pop", c);
		gfx.PopClip();
	}

	public void FillR(R r, BrushDef brush)
	{
		w?.WriteLine($"FillR {r}", c);
		gfx.FillR(r, brush);
	}

	public void DrawR(R r, PenDef pen)
	{
		w?.WriteLine($"DrawR {r}", c);
		gfx.DrawR(r, pen);
	}

	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
		w?.WriteLine($"DrawLine {a}-{b}", c);
		gfx.DrawLine(a, b, penDef);
	}

	public void DrawBmp(Bitmap bmp)
	{
		gfx.DrawBmp(bmp);
	}

	public Sz MeasureText_(string text, FontDef fontDef) => gfx.MeasureText_(text, fontDef);

	public void DrawText_(string text, FontDef fontDef, Color color)
	{
		w?.WriteLine($"DrawText '{text}'", c);
		gfx.DrawText_(text, fontDef, color);
	}
}
