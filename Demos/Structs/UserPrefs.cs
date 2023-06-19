using PowWinForms;

namespace Demos.Structs;

sealed class UserPrefs
{
	public (int, int, int, int) ConR { get; set; }

	public void Save() => Saving?.Invoke(null, EventArgs.Empty);

	public event EventHandler? Saving;
	static UserPrefs() =>
		WinFormsUtils.Tracker.Configure<UserPrefs>()
			.Properties(e => new
			{
				e.ConR,
			})
			.PersistOn(nameof(Saving));
}