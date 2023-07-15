namespace ControlSystem.Utils;

static class QueueExt
{
	public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> source)
	{
		foreach (var elt in source)
			queue.Enqueue(elt);
	}
}