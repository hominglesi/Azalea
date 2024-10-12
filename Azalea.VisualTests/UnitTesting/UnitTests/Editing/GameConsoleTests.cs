using Azalea.Editing;
using Azalea.Inputs;
using Azalea.Platform;
using Azalea.Utils;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Editing;
public class GameConsoleTests : UnitTestSuite
{
	public class FunctionalityTest : UnitTest
	{
		public FunctionalityTest()
		{
			AddOperation("Press F9 key", () => InputUtils.SimulateKeyInput(Keys.F9));
			AddOperation("Input 'fullscreen' command", () =>
			{
				Editor.FocusConsole();
				InputUtils.SimulateCharInput("fullscreen");
			});
			AddOperation("Input enter", () => InputUtils.SimulateKeyInput(Keys.Enter));
			AddResult("Check if window is Fullscreen", () => Window.State == WindowState.Fullscreen);

			AddOperation("Press F9 key", () => InputUtils.SimulateKeyInput(Keys.F9));
			AddOperation("Execute 'restorewindow' command", () =>
			{
				Editor.FocusConsole();
				InputUtils.SimulateCharInput("restorewindow");
			});
			AddOperation("Input enter", () => InputUtils.SimulateKeyInput(Keys.Enter));
			AddResult("Check if window is Restored", () => Window.State == WindowState.Normal);

			var lastTitle = Window.Title;
			AddOperation("Press F9 key", () => InputUtils.SimulateKeyInput(Keys.F9));
			AddOperation("Execute 'windowtitle Lorem Ipsum' command", () =>
			{
				Editor.FocusConsole();
				InputUtils.SimulateCharInput("windowtitle Lorem Ipsum");
			});
			AddOperation("Input enter", () => InputUtils.SimulateKeyInput(Keys.Enter));
			AddResult("Check if WindowTitle is 'Lorem Ipsum'", () => Window.Title == "Lorem Ipsum");
			AddOperation("Restore title", () => Window.Title = lastTitle);
		}
	}

	public class NewCommandTest : UnitTest
	{
		private const string _customCommand = "GameConsoleTestCustomCommand";
		private bool _commandRan;
		public NewCommandTest()
		{
			_commandRan = false;
			AddOperation("Add custom command",
				() => Editor.AddConsoleCommand(_customCommand, args => _commandRan = true));
			AddOperation("Run custom command",
				() => Editor.ExecuteConsoleQuery(_customCommand));
			AddResult("Check if command was ran", () => _commandRan);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			base.TearDown(scene);

			Editor.RemoveConsoleCommand(_customCommand);
		}
	}
}
