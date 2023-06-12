using PowRxVar;
using TestBase;
// ReSharper disable EmptyEmbeddedStatement

namespace TreePusherLib.Tests.ConvertExts;

class Events2TreeTests : RxTest
{
	[Test]
	public void _00_Simple()
	{
		using var d = new Disp();
		var (evtSig, evtObs) = TreeEvents<int>.Make().D(d);
		var pusher = new TreePusher<int>(evtSig);

		evtObs.ToTree(() =>
		{
			using (pusher.Push(3))
			{
				using (pusher.Push(5)) ;
				using (pusher.Push(7))
				{
					using (pusher.Push(9)) ;
					using (pusher.Push(11)) ;
				}

				using (pusher.Push(17)) ;
			}
		})
			.ShouldBeSameTree(
				N(3,
					N(5),
					N(7,
						N(9),
						N(11)
					),
					N(17)
				)
			);
	}
}