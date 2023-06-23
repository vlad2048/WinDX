using PowBasics.CollectionsExt;
using PowRxVar;
using TestBase;
using TreePusherLib.ConvertExts;
// ReSharper disable EmptyEmbeddedStatement
#pragma warning disable CS0642


namespace TreePusherLib.Tests.ConvertExts;

sealed class Events2TreeTests : RxTest
{
	[Test]
	public void _00_Simple()
	{
		using var d = new Disp();
		var (evtSig, evtObs) = TreeEvents<int>.Make().D(d);
		var pusher = new TreePusher<int>(evtSig);

		evtObs.ToTree(
			_ => { },
			() =>
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
			}
		)
			.ShouldBeSameReconstructedTree(
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



	[Test]
	public void _01_Missing()
	{
		using var d = new Disp();
		var (evtSig, evtObs) = TreeEvents<int>.Make().D(d);
		var pusher = new TreePusher<int>(evtSig);

		evtObs.ToTree(
			_ => { },
			() =>
			{
				using (pusher.Push(3))
				{
					using (pusher.Push(5)) ;
					using (pusher.Push(7))
					{
						pusher.Push(9);
						using (pusher.Push(11)) ;
					}

					using (pusher.Push(17)) ;
				}
			}
		)
			.ShouldBeSameReconstructedTree(
				N(3,
					N(5),
					N(7),
					N(17)
				),
				(7, 9)
			);
	}



	[Test]
	public void _02_MissingTwoLevels()
	{
		using var d = new Disp();
		var (evtSig, evtObs) = TreeEvents<int>.Make().D(d);
		var pusher = new TreePusher<int>(evtSig);

		evtObs.ToTree(
			_ => { },
			() =>
			{
				using (pusher.Push(3))
				{
					using (pusher.Push(5)) ;
					pusher.Push(7);
					using (pusher.Push(9)) ;
					using (pusher.Push(11)) ;
					using (pusher.Push(17)) ;
				}
			}
		)
			.ShouldBeSameReconstructedTree(
				N(3),
				(3, 7)
			);
	}

	

	[Test]
	public void _03_TwoMissingOnSameParent()
	{
		using var d = new Disp();
		var (evtSig, evtObs) = TreeEvents<int>.Make().D(d);
		var pusher = new TreePusher<int>(evtSig);

		// TODO: understand why the expected data looks/is wrong
		evtObs.ToTree(
			_ => { },
			() =>
			{
				using (pusher.Push(3))
				{
					using (pusher.Push(5)) ;
					pusher.Push(7);
					using (pusher.Push(9)) ;
					pusher.Push(11);
					using (pusher.Push(17)) ;
				}
			}
		)
			.ShouldBeSameReconstructedTree(
				N(3),
				(3, 7)
			);
	}
}


file static class ShouldExts
{
	public static void ShouldBeSameReconstructedTree<T>(
		this ReconstructedTree<T> reconstructredTree,
		TNod<T> expRoot,
		params (T, T)[] expIncomplete
	)
	{
		var actRoot = reconstructredTree.Root;
		var actIncomplete = reconstructredTree.IncompleteNodes.SelectToArray(e => (e.ParentNod.V, e.ChildNode));
		TreeLogger.L($"Actual   incomplete nodes: [{actIncomplete.JoinText("; ")}]");
		TreeLogger.L($"Expected incomplete nodes: [{expIncomplete.JoinText("; ")}]");
		TreeLogger.L("");
		actRoot.ShouldBeSameTree(expRoot);

		CollectionAssert.AreEqual(expIncomplete, actIncomplete);
	}
}