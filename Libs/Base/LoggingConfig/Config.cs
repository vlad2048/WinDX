namespace LoggingConfig;

public sealed record CfgLog(
	bool Redraws = false
);

public sealed record CfgTweaks(
	bool DisableHover = false
);


public sealed record Config(
	CfgLog Log,
	CfgTweaks Tweaks
)
{
	public static Config Default = new(
		new CfgLog(),
		new CfgTweaks()
	);
}
