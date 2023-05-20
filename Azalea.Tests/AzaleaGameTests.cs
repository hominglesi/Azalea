using Azalea;
using Azalea.Platform;
using Azalea.Tests.Platform;

namespace Azalea.Tests
{
    public class Tests
    {
        private IGameHost _host;
        private TestGame _game;

        [SetUp]
        public void Setup()
        {
            _host = new DummyGameHost();
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