using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Utils;

#pragma warning disable CS8602, CS8618

namespace Azalea.VisualTests.UnitTesting.UnitTests.UserInput;
public class TextBoxTests : UnitTestSuite
{
	public class InputTest : UnitTest
	{
		private TextBox _textBox;

		public InputTest()
		{
			AddOperation("Input 'Lorem Ipsum'", () =>
			{
				Input.ChangeFocus(_textBox);
				InputUtils.SimulateCharInput("Lorem Ipsum");
			});
			AddResult("Check if Text is 'Lorem Ipsum'", () => _textBox.Text == "Lorem Ipsum");

			AddOperation("Press Backspace twice", () =>
			{
				Input.ChangeFocus(_textBox);
				InputUtils.SimulateMultipleKeyInput(Keys.Backspace, 2);
			});
			AddResult("Check if Text is 'Lorem Ips'", () => _textBox.Text == "Lorem Ips");

			AddOperation("Press Left arrow 3 times", () =>
			{
				Input.ChangeFocus(_textBox);
				InputUtils.SimulateMultipleKeyInput(Keys.Left, 3);
			});
			AddOperation("Input 'ch'", () =>
			{
				Input.ChangeFocus(_textBox);
				InputUtils.SimulateCharInput("ch");
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
}

#pragma warning restore CS8602, CS8618
