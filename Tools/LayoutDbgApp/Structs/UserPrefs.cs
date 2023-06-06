using PowWinForms;
using RenderLib;

namespace LayoutDbgApp.Structs;

class UserPrefs
{
	public string? LastFolder { get; set; }
	public string? OpenFile { get; set; }
	public int ExternalWindosPosX { get; set; }
	public int ExternalWindosPosY { get; set; }
	public RendererType SelectedRenderer { get; set; }

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
			})
			.PersistOn(nameof(Saving));
}