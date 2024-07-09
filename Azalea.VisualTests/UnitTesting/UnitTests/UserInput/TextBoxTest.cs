using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;

#pragma warning disable CS8602, CS8618

namespace Azalea.VisualTests.UnitTesting.UnitTests.UserInput;
public class TextBoxTest : UnitTest
{
	private TextBox _textBox;

	public TextBoxTest()
	{
		AddOperation("Input 'Lorem Ipsum'", () =>
		{
			Input.ChangeFocus(_textBox);
			Input.HandleTextInput('L'); Input.HandleTextInput('o'); Input.HandleTextInput('r');
			Input.HandleTextInput('e'); Input.HandleTextInput('m'); Input.HandleTextInput(' ');
			Input.HandleTextInput('I'); Input.HandleTextInput('p'); Input.HandleTextInput('s');
			Input.HandleTextInput('u'); Input.HandleTextInput('m');
		});
		AddResult("Check if Text is 'Lorem Ipsum'", () => _textBox.Text == "Lorem Ipsum");

		AddOperation("Press Backspace twice", () =>
		{
			Input.ChangeFocus(_textBox);
			for (int i = 0; i < 2; i++)
			{
				Input.HandleKeyboardKeyStateChange(Keys.Backspace, true);
				Input.HandleKeyboardKeyStateChange(Keys.Backspace, false);
			}
		});
		AddResult("Check if Text is 'Lorem Ips'", () => _textBox.Text == "Lorem Ips");

		AddOperation("Press Left arrow 3 times", () =>
		{
			Input.ChangeFocus(_textBox);
			for (int i = 0; i < 3; i++)
			{
				Input.HandleKeyboardKeyStateChange(Keys.Left, true);
				Input.HandleKeyboardKeyStateChange(Keys.Left, false);
			}
		});
		AddOperation("Input 'ch'", () =>
		{
			Input.ChangeFocus(_textBox);
			Input.HandleTextInput('c');
			Input.HandleTextInput('h');
		});
		AddResult("Check if Text is 'Lorem chIps'", () => _textBox.Text == "Lorem chIps");

		AddOperation("Clear Textbox", () => _textBox.Text = "");
		AddResult("Check if Textbox is empty", () => _textBox.Text == "");
	}

	public override void Setup(Composition scene)
	{
		_textBox = new BasicTextBox(t => t.Color = Palette.Black)
		{
			Origin = Anchor.Center,
			Anchor = Anchor.Center,
			CaratColor = Palette.Black
		};

		scene.Add(_textBox);
	}

	public override void TearDown(Composition scene)
	{
		scene.Remove(_textBox);
	}
}

#pragma warning restore CS8602, CS8618
