using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Numerics;
using System.Numerics;

namespace Azalea.VisualTests.BoundingBoxTree;
public class BoundingBoxTreeTest : TestScene
{
	private BoundingBoxBranch Root;

	public BoundingBoxTreeTest()
	{
		var window = AzaleaGame.Main.Host.Window;
		window.Resizable = false;

		Root = new BoundingBoxBranch(new Rectangle(Vector2.Zero, window.ClientSize), 9);

		Add(_displayBox = new Box()
		{
			Color = Palette.Aqua
		});
		Add(_checkCountDisplay = new SpriteText());
	}

	private Box _displayBox;
	private SpriteText _checkCountDisplay;
	protected override void Update()
	{
		var contained = Root.ContainsPoint(Input.MousePosition);

		if (contained is null)
		{
			_checkCountDisplay.Text = "0";
			_displayBox.Alpha = 0;
		}
		else
		{
			_displayBox.Alpha = 1;
			_displayBox.Position = contained.Value.BoundingBox.TopLeft;
			_displayBox.Size = contained.Value.BoundingBox.Size;

			_checkCountDisplay.Text = contained.Value.CheckCount.ToString();
		}
	}
}
