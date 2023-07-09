using PowWinForms;

namespace Demos;

sealed class DemoPrefs
{
    public (int, int, int, int) ConR { get; set; }


    public void Save() => Saving?.Invoke(null, EventArgs.Empty);

    public event EventHandler? Saving;
    static DemoPrefs() =>
        WinFormsUtils.Tracker.Configure<DemoPrefs>()
            .Properties(e => new
            {
                e.ConR,
            })
            .PersistOn(nameof(Saving));
}