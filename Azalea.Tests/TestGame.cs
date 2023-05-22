namespace Azalea.Tests;

internal class TestGame : AzaleaGame
{
    internal bool OnInitializeRan;

    protected override void OnInitialize()
    {
        OnInitializeRan = true;
    }

    protected override void OnRender()
    {

    }
}
