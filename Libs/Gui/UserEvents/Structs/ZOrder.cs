namespace UserEvents.Structs;

public sealed record ZOrder(
	int Win,
	bool IsSys,
	int Node
) : IComparable<ZOrder>
{
	public int CompareTo(ZOrder? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (ReferenceEquals(null, other)) return 1;
		var winComparison = -Win.CompareTo(other.Win);
		if (winComparison != 0) return winComparison;
		var isSysComparison = -IsSys.CompareTo(other.IsSys);
		if (isSysComparison != 0) return isSysComparison;
		return -Node.CompareTo(other.Node);
	}

	public static bool operator <(ZOrder? left, ZOrder? right) => Comparer<ZOrder>.Default.Compare(left, right) < 0;
	public static bool operator >(ZOrder? left, ZOrder? right) => Comparer<ZOrder>.Default.Compare(left, right) > 0;
	public static bool operator <=(ZOrder? left, ZOrder? right) => Comparer<ZOrder>.Default.Compare(left, right) <= 0;
	public static bool operator >=(ZOrder? left, ZOrder? right) => Comparer<ZOrder>.Default.Compare(left, right) >= 0;
}