using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Tests.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Tests.Platform;

internal class DummyGameHost : GameHost
{
    public override IRenderer Renderer => _renderer;
    private readonly DummyRenderer _renderer;

    public override event Action? Initialized;

    public DummyGameHost()
    {
        _renderer = new DummyRenderer();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        Initialized?.Invoke();
    }
}
