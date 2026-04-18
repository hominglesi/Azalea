using Azalea.Editor.Design.Gui;

namespace Azalea.VisualTests;

public class DebugTest : TestScene
{
	private readonly GUIWindow _debugWindow;

	public DebugTest()
	{
		_debugWindow = GUIWindow.Create("Debug Window", new(400));
		_debugWindow.AddLabel("Example debug text");
		_debugWindow.AddCheckbox("Example checkbox");
		_debugWindow.AddLabel("Example debug text 2");
		_debugWindow.AddCheckbox("Example checkbox 2");
		_debugWindow.AddCheckbox("Example checkbox 3");
		_debugWindow.AddSliderFloat("Example slider float", 0, 100, 50);
	}
}
