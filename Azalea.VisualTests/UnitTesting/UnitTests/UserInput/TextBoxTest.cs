using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Inputs;

#pragma warning disable CS8602, CS8618

namespace Azalea.VisualTests.UnitTesting.UnitTests.UserInput;
public class TextBoxTest : UnitTest
{
	private TextBox _textBox;

	public TextBoxTest()
	{
		AddOperation("Clear Textbox", () => _textBox.Text = "");
		AddOperation("Input 'Ide Gas'", () =>
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

		AddResult("Check if Text is 'Ide Gas'", () => _textBox.DisplayedText == "Ide Gas");
		AddOperation("Clear Textbox", () => _textBox.Text = "");
		AddResult("Check if Textbox is empty", () => _textBox.DisplayedText == "");
	}

	public override void Setup(Composition scene)
	{
		_textBox = new BasicTextBox()
		{
			Origin = Anchor.Center,
			Anchor = Anchor.Center,
		};

		scene.Add(_textBox);
	}

	public override void TearDown(Composition scene)
	{
		scene.Remove(_textBox);
	}
}

#pragma warning restore CS8602, CS8618
