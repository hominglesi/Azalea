using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Platform;
using Azalea.Tests.Dummy;

namespace Azalea.Tests;
public class InheritanceTests
{
	private GameHost _host;

	[SetUp]
	public void Setup()
	{
		_host = Host.CreateHost(new HostPreferences());
	}

	[Test]
	public void RemoveChild()
	{
		_host.Run(new DummyGame(initializeAction: () =>
		{
			var x = new Composition();
			var y = new Box();

			x.Add(y);
			x.Remove(y);

			Assert.That(x.Children, Is.Empty);
			Assert.That(y.ChildID, Is.Zero);

			x.Add(y);
			x.Remove(y);

			Assert.That(x.Children, Is.Empty);
			Assert.That(y.ChildID, Is.Zero);

			_host.Window.Close();
		}));
	}
}
