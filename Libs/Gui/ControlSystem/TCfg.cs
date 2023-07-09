namespace ControlSystem;

sealed record TCfgLog(
	bool Redraws
);

sealed record TCfg(
	TCfgLog Log
)
{
	public static TCfg Default = new(
		new TCfgLog(
			false
		)
	);
}
