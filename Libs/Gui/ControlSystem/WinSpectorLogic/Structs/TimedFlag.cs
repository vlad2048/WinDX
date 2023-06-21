namespace ControlSystem.WinSpectorLogic.Structs;

sealed class TimedFlag
{
	private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(100);
	private bool value;
	private DateTime expiryTime;

	public void Set()
	{
		value = true;
		expiryTime = DateTime.Now + Delay;
	}

	public bool IsNotSet()
	{
		if (DateTime.Now >= expiryTime) value = false;
		return !value;
	}
}