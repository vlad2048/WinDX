using PowWinForms;
using RenderLib;

namespace FlexBuilder.Structs;

sealed class UserPrefs
{
	public string? LastFolder { get; set; }
	public string? OpenFile { get; set; }
	public int ExternalWindosPosX { get; set; }
	public int ExternalWindosPosY { get; set; }
	public RendererType SelectedRenderer { get; set; }
	public (int, int, int, int) ConR { get; set; }

	public void Save() => Saving?.Invoke(null, EventArgs.Empty);

	public event EventHandler? Saving;
	static UserPrefs() =>
		WinFormsUtils.Tracker.Configure<UserPrefs>()
			.Properties(e => new
			{
				e.LastFolder,
				e.OpenFile,
				e.ExternalWindosPosX,
				e.ExternalWindosPosY,
				e.SelectedRenderer,
				e.ConR,
			})
			.PersistOn(nameof(Saving));
}