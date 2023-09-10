using Azalea.Graphics;
using Azalea.Platform;
using Azalea.Tests.Platform;

namespace Azalea.Tests
{
	public class Tests
	{
		private GameHost _host;

		[SetUp]
		public void Setup()
		{
			_host = new DummyGameHost();
		}

		[Test]
		public void DoesOnInitializeGetCalled()
		{
			var game = new TestGame();
			_host.Run(game);
			Assert.That(game.OnInitializeRan, Is.True);
		}

		[Test]
		public void RemovingChildren()
		{
			var game = new RemovingChildrenGame();
			_host.Run(game);

			Assert.That(game.Children, Is.Empty);

			game.AddChild();
			Assert.That(game.Children, Has.Count.EqualTo(1));

			game.RemoveChild();
			Assert.That(game.Children, Is.Empty);
		}

		private class RemovingChildrenGame : AzaleaGame
		{
			public int ChildCount => Children.Count;
			private GameObject? _child;
			protected override void OnInitialize()
			{
				_child = new DummyObject();
			}

			public void AddChild() => Add(_child);
			public void RemoveChild() => Remove(_child);
		}
	}
}