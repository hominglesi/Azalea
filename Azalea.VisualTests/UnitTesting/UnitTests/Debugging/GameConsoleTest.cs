﻿using Azalea.Debugging;
using Azalea.Design.Containers;
using Azalea.Inputs;
using Azalea.Platform;
using Azalea.Utils;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Debugging;
public class GameConsoleTests : UnitTestSuite
{
	public class FunctionalityTest : UnitTest
	{
		public FunctionalityTest()
		{
			AddOperation("Press F9 key", () => InputUtils.SimulateKeyInput(Keys.F9));
			AddOperation("Input 'fullscreen' command", () =>
			{
				Input.ChangeFocus(Editor.Overlay.DebugConsole);
				InputUtils.SimulateCharInput("fullscreen");
			});
			AddOperation("Input enter", () => InputUtils.SimulateKeyInput(Keys.Enter));
			AddResult("Check if window is Fullscreen", () => AzaleaGame.Main.Host.Window.State == WindowState.Fullscreen);

			AddOperation("Press F9 key", () => InputUtils.SimulateKeyInput(Keys.F9));
			AddOperation("Execute 'restorewindow' command", () =>
			{
				Input.ChangeFocus(Editor.Overlay.DebugConsole);
				InputUtils.SimulateCharInput("restorewindow");
			});
			AddOperation("Input enter", () => InputUtils.SimulateKeyInput(Keys.Enter));
			AddResult("Check if window is Restored", () => AzaleaGame.Main.Host.Window.State == WindowState.Normal);

			var lastTitle = AzaleaGame.Main.Host.Window.Title;
			AddOperation("Press F9 key", () => InputUtils.SimulateKeyInput(Keys.F9));
			AddOperation("Execute 'windowtitle Lorem Ipsum' command", () =>
			{
				Input.ChangeFocus(Editor.Overlay.DebugConsole);
				InputUtils.SimulateCharInput("windowtitle Lorem Ipsum");
			});
			AddOperation("Input enter", () => InputUtils.SimulateKeyInput(Keys.Enter));
			AddResult("Check if WindowTitle is 'Lorem Ipsum'", () => AzaleaGame.Main.Host.Window.Title == "Lorem Ipsum");
			AddOperation("Restore title", () => AzaleaGame.Main.Host.Window.Title = lastTitle);
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
				() => Editor.Overlay.DebugConsole.AddCommand(_customCommand, args => _commandRan = true));
			AddOperation("Run custom command",
				() => Editor.Overlay.DebugConsole.ExecuteQuery(_customCommand));
			AddResult("Check if command was ran", () => _commandRan);
		}

		public override void TearDown(Composition scene)
		{
			Editor.Overlay.DebugConsole.RemoveCommand(_customCommand);
		}
	}
}