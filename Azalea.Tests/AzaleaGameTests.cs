using Azalea;
using Azalea.Platform;

namespace Azalea.Tests
{
    public class Tests
    {
        private IGameHost _host;
        private TestGame _game;

        [SetUp]
        public void Setup()
        {
            _host = Host.CreateHost();
            _game = new TestGame();
        }

        [Test]
        public void DoesOnInitializeGetCalled()
        {
            _host.Run(_game);
            Assert.That(_game.OnInitializeRan, Is.True);
        }
    }
}