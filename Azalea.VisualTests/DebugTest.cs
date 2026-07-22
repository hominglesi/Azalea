using Azalea.Editor.Design.Gui;

namespace Azalea.VisualTests;

public class DebugTest : TestScene
{
	private readonly GUIWindow _debugWindow;

	public DebugTest()
	{
		_debugWindow = GUIWindow.Create("Debug Window", new(100), new(400));
		_debugWindow.AddLabel("Example debug text");
		_debugWindow.AddCheckbox("Example checkbox");
		_debugWindow.AddLabel("Example debug text 2");
		_debugWindow.AddCheckbox("Example checkbox 2");
		_debugWindow.AddCheckbox("Example checkbox 3");
		_debugWindow.AddSliderFloat("Example slider float", 0, 100, 50);
		_debugWindow.AddGroup("Example group");
		_debugWindow.AddLabel("Label in group");
		_debugWindow.AddSliderFloat("Slider in group", 0, 10, 5);
		_debugWindow.AddLabel("Label in group 2");
		_debugWindow.AddCheckbox("Checkbox in group");
		_debugWindow.FinishGroup();
		_debugWindow.AddGroup("Example group 2");
		_debugWindow.AddGroup("Nested group");
		_debugWindow.AddLabel("Label in nested group");
		_debugWindow.FinishGroup();
		_debugWindow.AddGroup("Nested group 2");
		_debugWindow.AddCheckbox("Checkbox in nested group");
		_debugWindow.AddGroup("Double nested group");
		_debugWindow.AddLabel("Double nested label");
		_debugWindow.AddSliderFloat("Double nested slider");
		_debugWindow.FinishGroup();
		_debugWindow.FinishGroup();
		_debugWindow.FinishGroup();
	}
}
