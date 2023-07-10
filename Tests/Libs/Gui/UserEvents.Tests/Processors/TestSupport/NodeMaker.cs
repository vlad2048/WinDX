using Moq;
using PowBasics.Geom;
using PowRxVar;

namespace UserEvents.Tests.Processors.TestSupport;

static class NodeMaker
{
	public static (INode, Action<R>, IDisposable) Make()
	{
		var d = new Disp();
		var rVar = Var.Make(R.Empty).D(d);

		var mockNode = new Mock<INode>();

		mockNode
			.Setup(e => e.R)
			.Returns(rVar);

		return (
			mockNode.Object,
			v => rVar.V = v,
			d
		);
	}
}