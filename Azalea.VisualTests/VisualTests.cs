using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.IO.Stores;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{


    protected override void OnInitialize()
    {
        Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

        Host.Renderer.ClearColor = Color.Azalea;
        Host.Window.Resizable = true;

        Add(new TestingTestScene());

    }

    protected override void Update()
    {
        if (Input.GetKey(Keys.Escape).Down) Host.Window.Close();
    }
}
