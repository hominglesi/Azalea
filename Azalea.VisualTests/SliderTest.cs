using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;

namespace Azalea.VisualTests;
internal class SliderTest : TestScene
{
	public SliderTest()
	{
		AddRange(new GameObject[]
		{
			new BasicSlider()
			{
				Position = new(200, 200),
				Size = new(300, 40)
			},
			new BasicSlider()
			{
				Position = new(500, 300),
				Size = new(40, 300),
				Direction = SliderDirection.Vertical
			}
		});
	}
}
