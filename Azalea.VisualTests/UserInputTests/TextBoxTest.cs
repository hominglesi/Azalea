using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Inputs;

namespace Azalea.VisualTests.UserInputTests;
public class TextBoxTest : UnitTestScene
{
	private TextBox _textBox;

	public TextBoxTest()
	{
		Add(_textBox = new BasicTextBox()
		{
			Origin = Anchor.Center,
			Anchor = Anchor.Center,
		});

		AddStep("Clear Textbox", () => _textBox.Text = "");

		AddStep("Input 'Ide Gas'", () =>
		{
			Input.ChangeFocus(_textBox);
			Input.HandleTextInput('I');
			Input.HandleTextInput('d');
			Input.HandleTextInput('e');
			Input.HandleTextInput(' ');
			Input.HandleTextInput('G');
			Input.HandleTextInput('a');
			Input.HandleTextInput('s');
		});

		AddTestStep("Check if Text is 'Ide Gas'", () => _textBox.DisplayedText == "Ide Gas");
		AddStep("Clear Textbox", () => _textBox.Text = "");
		AddTestStep("Check if Textbox is empty", () => _textBox.DisplayedText == "");
	}
}
