using PowWinForms;
using RenderLib;

namespace FlexBuilder.Structs;

sealed class UserPrefs
{
	public string? LastFolder { get; set; }
	public string? OpenFile { get; set; }
	public bool ExternalWindowVisible { get; set; }
	public (int, int) ExternalWindosPos { get; set; }
	public RendererType SelectedRenderer { get; set; }
	public (int, int, int, int) ConR { get; set; }
	public TabName SelectedTab { get; set; }
	public int DetailsSplitterPos { get; set; }

	public void Save() => Saving?.Invoke(null, EventArgs.Empty);

	public event EventHandler? Saving;
	static UserPrefs() =>
		WinFormsUtils.Tracker.Configure<UserPrefs>()
			.Properties(e => new
			{
				e.LastFolder,
				e.OpenFile,
				e.ExternalWindowVisible,
				e.ExternalWindosPos,
				e.SelectedRenderer,
				e.ConR,
				e.SelectedTab,
				e.DetailsSplitterPos,
			})
			.PersistOn(nameof(Saving));
}