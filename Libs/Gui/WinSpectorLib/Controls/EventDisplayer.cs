using System.Reactive;
using System.Reactive.Linq;
using ControlSystem;
using ControlSystem.Structs;
using DynamicData;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using UserEvents.Structs;
using UserEvents.Utils;
using WinAPI.User32;

namespace WinSpectorLib.Controls;

sealed partial class EventDisplayer : UserControl
{
	private const int MAX_ITEMS = 20;

	private interface ITrk {
		string Name { get;  }
		IObservable<IUserEvt> Evt { get; }
		bool IsState(NodeState state);
		bool IsCtrl(Ctrl ctrl);
	}
	private sealed record NodeStateTrk(string Name, NodeState NodeState) : ITrk
	{
		public IObservable<IUserEvt> Evt => NodeState.Evt;
		public bool IsState(NodeState state) => NodeState == state;
		public bool IsCtrl(Ctrl ctrl) => false;
	}
	private sealed record WinTrk(string Name, Win Win) : ITrk
	{
		public IObservable<IUserEvt> Evt => Win.Evt;
		public bool IsState(NodeState state) => false;
		public bool IsCtrl(Ctrl ctrl) => ctrl.Win.V.IsSome(out var w) && w == Win;
	}

	private readonly ISourceList<ITrk> trksSrc;
	private readonly IObservableList<ITrk> trks;

	private readonly IRwVar<bool> isEmpty;
	private IRwVar<bool> showEvents = null!;
	private IRoMayVar<MixLayout> selLayout = null!;

	public EventDisplayer()
	{
		InitializeComponent();

		trksSrc = new SourceList<ITrk>().D(this);
		trks = trksSrc.AsObservableList().D(this);
		var isPaused = Var.Make(false).D(this);
		isEmpty = Var.Make(false).D(this);

		this.InitRX(d => {
			var trksObs = trksSrc.Connect();

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

			//var objLock = new object();

			var evt = trksObs
				.MergeMany(trk => PrettyPrintAggregator.Transform(trk.Evt/*.Synchronize(objLock)*/).Select(e => (trk, e)))
				.Where(_ => !isPaused.V);

			evt.Subscribe(e => {
				var str = $"[{e.trk.Name}] - {e.e}";
				if (e.e is MouseMoveUpdatePrettyUserEvt { IsSubsequent: true } && eventListBox.Items.Count > 0) {
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

	public bool IsAnyNodeTracked() => trks.Count > 0;

	public bool IsNodeTracked(IMixNode node) => node switch
	{
		StFlexNode {State: var state} => trks.Items.Any(e => e.IsState(state)),
		CtrlNode {Ctrl: var ctrl} => trks.Items.Any(e => e.IsCtrl(ctrl)),
		_ => false
	};

	public string GetTrackedNodeName(IMixNode node)
	{
		var trk = GetTrk(node);
		return trk switch
		{
			null => string.Empty,
			not null => trk.Name
		};
	}


	public void TrackNode(IMixNode node)
	{
		if (IsNodeTracked(node)) return;
		var name = GetNewName(node);
		ITrk trk = node switch
		{
			StFlexNode { State: var state } => new NodeStateTrk(name, state),
			CtrlNode { Ctrl.Win.V: var mayWin } => new WinTrk(name, mayWin.Ensure()),
			_ => throw new ArgumentException()
		};

		trksSrc.Add(trk);
	}

	public void StopTrackingNode(IMixNode node)
	{
		var trk = GetTrk(node);
		if (trk == null) return;
		trksSrc.Edit(upd => upd.Remove(trk));
	}

	public void StopAllTracking()
	{
		if (!IsAnyNodeTracked()) return;
		trksSrc.Clear();
	}

		
	private ITrk? GetTrk(IMixNode node) => node switch
	{
		StFlexNode { State: var state } => trks.Items.FirstOrDefault(e => e.IsState(state)),
		CtrlNode { Ctrl: var ctrl } => trks.Items.FirstOrDefault(e => e.IsCtrl(ctrl)),
		_ => null
	};

	private string GetNewName(IMixNode node)
	{
		var baseName = node switch
		{
			StFlexNode { State: var state } => "N",
			CtrlNode => "Win",
			_ => throw new ArgumentException()
		};

		var arr = trks.Items.ToArray();
		var idx = 0;
		var name = $"{baseName}_{idx}";
		while (arr.Any(e => e.Name == name))
			name = $"{baseName}_{++idx}";
		return name;
	}
}
