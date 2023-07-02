using System.Reactive;
using System.Reactive.Linq;
using ControlSystem.Structs;
using DynamicData;
using PowRxVar;
using PowWinForms;
using UserEvents.Structs;
using UserEvents.Utils;
using WinAPI.User32;

namespace WinSpectorLib.Controls;

sealed partial class EventDisplayer : UserControl
{
	private const int MAX_ITEMS = 20;

	private readonly ISourceCache<TrackedNode, StFlexNode> nodesSrc;
	private readonly IObservableCache<TrackedNode, StFlexNode> nodes;
	private readonly IRwVar<bool> isEmpty;
	private IRwVar<bool> showEvents = null!;
	private IRoMayVar<MixLayout> selLayout = null!;

	public EventDisplayer()
	{
		InitializeComponent();

		nodesSrc = new SourceCache<TrackedNode, StFlexNode>(e => e.Node).D(this);
		nodes = nodesSrc.AsObservableCache().D(this);
		var isPaused = Var.Make(false).D(this);
		isEmpty = Var.Make(false).D(this);

		this.InitRX(d => {
			var ctrlsObs = nodesSrc.Connect();

			var winEvt = selLayout.SelectMaySwitch(e => e.Win.Evt);

			IObservable<Unit> WhenKey(Keys key) => Obs.Merge(
				ParentForm.Events().KeyDown.Where(e => e.KeyCode == key).ToUnit(),
				winEvt.WhenKeyDown((VirtualKey)key)
			);

			WhenKey(Keys.H).Subscribe(_ => showEvents.V = !showEvents.V).D(d);
			WhenKey(Keys.C).Subscribe(_ => Clear()).D(d);
			WhenKey(Keys.P).Subscribe(_ => isPaused.V = !isPaused.V).D(d);

			isPaused.Subscribe(paused => pauseBtn.Text = paused ? "Resume (P)" : "Pause (P)").D(d);
			pauseBtn.Events().Click.Subscribe(_ => isPaused.V = !isPaused.V).D(d);

			isEmpty.Subscribe(e => clearBtn.Enabled = pauseBtn.Enabled = !e).D(d);
			clearBtn.Events().Click.Subscribe(_ => Clear()).D(d);

			hideBtn.Events().Click.Subscribe(_ => showEvents.V = false).D(d);

			var evt = ctrlsObs
				.MergeManyItems(trackedNode => PrettyPrintAggregator.Transform(trackedNode.Node.State.Evt))
				.Where(_ => !isPaused.V);

			evt.Subscribe(e => {
				var str = $"[{e.Item.Name}] - {e.Value}";
				if (e.Value is MouseMoveUpdatePrettyUserEvt { IsSubsequent: true } && eventListBox.Items.Count > 0) {
					eventListBox.Items[^1] = str;
				} else {
					eventListBox.Items.Add(str);
				}
				if (eventListBox.Items.Count > MAX_ITEMS)
					eventListBox.Items.RemoveAt(0);

				eventListBox.TopIndex = eventListBox.Items.Count - 1;
				isEmpty.V = false;
			}).D(d);

		});
	}

	public void SetShowEvents(IRwVar<bool> showEvents_, IRoMayVar<MixLayout> selLayout_)
	{
		showEvents = showEvents_;
		selLayout = selLayout_;
	}

	public void Clear()
	{
		eventListBox.Items.Clear();
		isEmpty.V = true;
	}

	public bool IsAnyNodeTracked() => nodes.Count > 0;

	public bool IsNodeTracked(StFlexNode node) => nodes.Items.Any(e => e.Node == node);

	public string GetTrackedNodeName(StFlexNode node)
	{
		var trackedNode = nodes.Items.FirstOrDefault(e => e.Node == node);
		return trackedNode switch {
			null => string.Empty,
			not null => trackedNode.Name
		};
	}

	public void TrackNode(StFlexNode node)
	{
		if (IsNodeTracked(node)) return;
		var trackedNode = new TrackedNode(GetNewName(node), node);
		nodesSrc.AddOrUpdate(trackedNode);
	}

	public void StopTrackingNode(StFlexNode node)
	{
		if (!IsNodeTracked(node)) return;
		nodesSrc.Remove(node);
	}

	public void StopAllTracking()
	{
		if (!IsAnyNodeTracked()) return;
		nodesSrc.Clear();
	}


	private sealed record TrackedNode(
		string Name,
		StFlexNode Node
	)
	{
		public override string ToString() => Name;
	}

	private string GetNewName(StFlexNode node)
	{
		var arr = nodes.Items.ToArray();
		var idx = 0;
		var name = $"N_{idx}";
		while (arr.Any(e => e.Name == name))
			name = $"N_{++idx}";
		return name;
	}
}
