using System.Drawing;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using DynamicData;
using PowBasics.ColorCode;
using PowBasics.ColorCode.Utils;

namespace ControlSystem.Logic.Rendering_;

class RenderLogger
{
	private readonly bool enabled;
	private readonly Partition part;
	private readonly TxtWriter w = new();
	private int level;
	private void Pad() => w.Write(new string(' ', level * 4), Color.White);

	public RenderLogger(bool enabled, Partition part)
	{
		this.enabled = enabled;
		this.part = part;
	}

	public void Start()
	{
		if (!enabled) return;
		w.WriteLine();
		w.Write("Rendering ", C.StartHeader);
		w.WriteLine(part.GetTitle(), C.StartTitle);
		w.WriteLine(new string('-', $"Rendering {part.GetTitle()}".Length), C.StartHeader);
	}

	public void Finish()
	{
		if (!enabled) return;
		var txt = w.Txt;
		txt.PrintToConsole();
		//txt.RenderToHtml(@"C:\tmp\fmt\rendering.html");
	}

	public void PushCtrl(Ctrl ctrl)
	{
		if (!enabled) return;
		Pad();
		var isOn = part.Ctrls.Contains(ctrl);
		w.WriteLine($"[{ctrl.GetType().Name}]", isOn ? C.CtrlOn : C.CtrlOff);
		level++;
	}

	public void PopCtrl(Ctrl ctrl)
	{
		if (!enabled) return;
		level--;
		Pad();
		var isOn = part.Ctrls.Contains(ctrl);
		w.WriteLine($"[/{ctrl.GetType().Name}]", isOn ? C.CtrlOn : C.CtrlOff);
	}

	public void PushFlex(StFlexNode st)
	{
		if (!enabled) return;
		Pad();
		var isOn = part.NodeStates.Contains(st.State);
		w.WriteLine($"({st.State})", isOn ? C.FlexOn : C.FlexOff);
		level++;
	}

	public void PopFlex(StFlexNode st)
	{
		if (!enabled) return;
		level--;
		Pad();
		var isOn = part.NodeStates.Contains(st.State);
		w.WriteLine($"(/{st.State})", isOn ? C.FlexOn : C.FlexOff);
	}

	public void Draw(string str)
	{
		if (!enabled) return;
		Pad();
		w.WriteLine(str, C.Draw);
	}


	private static class C
	{
		public static readonly Color StartHeader = Color.FromArgb(148, 148, 148);
		public static readonly Color StartTitle = Color.FromArgb(82, 152, 209);

		public static readonly Color CtrlOn = Color.FromArgb(230, 57, 215);
		public static readonly Color CtrlOff = Color.FromArgb(112, 21, 104);

		public static readonly Color FlexOn = Color.FromArgb(235, 240, 139);
		public static readonly Color FlexOff = Color.FromArgb(148, 146, 40);

		public static readonly Color Draw = Color.FromArgb(77, 148, 44);
	}
}



file static class RenderLoggerExt
{
	public static string GetTitle(this Partition part)
	{
		var idx = part.Set.Partitions.IndexOf(part);
		return idx switch
		{
			0 => "MainPartition",
			_ => $"PopupPartition[{idx - 1}]"
		};
	}
}