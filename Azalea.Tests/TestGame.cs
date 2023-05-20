using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Tests;

internal class TestGame : AzaleaGame
{
    internal bool OnInitializeRan;

    protected override void OnInitialize()
    {
        OnInitializeRan = true;
    }
}
