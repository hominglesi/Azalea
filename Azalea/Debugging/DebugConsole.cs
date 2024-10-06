using Azalea.Amends;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Debugging;
public class DebugConsole : TextBox
{
	private Dictionary<string, ConsoleCommandDelegate> _commands = new();
	private Box _carat;

	public DebugConsole()
	{
		RelativeSizeAxes = Axes.X;
		Size = new(1, 20);
		BackgroundColor = Palette.Gray;
		AddInternal(_carat = new Box()
		{
			Size = new(1, 20),
			Alpha = 0,
			Color = Palette.White
		});

		//Windowing Commands
		AddCommand("fullscreen", args => GameHost.Main.Window.State = WindowState.Fullscreen);
		AddCommand("minimize", args => GameHost.Main.Window.State = WindowState.Minimized);
		AddCommand("maximize", args => GameHost.Main.Window.State = WindowState.Maximized);
		AddCommand("restorewindow", args => GameHost.Main.Window.State = WindowState.Normal);
		AddCommand("windowtitle", args => GameHost.Main.Window.Title = args.ArgumentQuery);
		AddCommand("vsync", args =>
		{
			if (args.ArgumentQuery.ToLower() == "on") GameHost.Main.Window.VSync = true;
			else if (args.ArgumentQuery.ToLower() == "off") GameHost.Main.Window.VSync = false;
		});
	}

	public event Action? Activated;
	public event Action? Deactivated;

	internal void Activate()
	{
		Input.ChangeFocus(this);
		Activated?.Invoke();
	}

	internal void Deactivate()
	{
		if (HasFocus)
			Input.ChangeFocus(this);

		Deactivated?.Invoke();
	}

	#region Style
	protected override void OnFocus(FocusEvent e)
	{
		_carat.Loop(
			x => x.Execute(x => x.Alpha = 1)
			.Then().Wait(0.5f)
			.Then().Execute(x => x.Alpha = 0), 1f);
	}

	protected override void OnFocusLost(FocusLostEvent e)
	{
		_carat.RemoveAmends();
		_carat.Alpha = 0;
	}

	protected override void OnCaratPositionChanged(Vector2 position)
	{
		_carat.Position = position;
	}
	#endregion

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.Enter)
		{
			ExecuteQuery(Text);
			Text = "";
			InputUtils.SimulateKeyInput(Keys.F9);
			return true;
		}

		return base.OnKeyDown(e);
	}

	public void AddCommand(string keyword, ConsoleCommandDelegate command)
	{
		if (_commands.ContainsKey(keyword))
			throw new InvalidOperationException($"Command with keyword {keyword} already exists");

		_commands.Add(keyword, command);
	}

	public void RemoveCommand(string keyword)
	{
		if (_commands.ContainsKey(keyword))
			_commands.Remove(keyword);
	}

	public void ExecuteQuery(string query)
	{
		var commandParameters = new CommandParameters(query);

		_commands.TryGetValue(commandParameters.Keyword, out var command);
		if (command is null) return;

		command(commandParameters);
	}

	public delegate void ConsoleCommandDelegate(CommandParameters parameters);
	public readonly struct CommandParameters
	{
		public readonly string Query;
		public readonly string Keyword;
		public readonly string[] Arguments;
		public readonly string ArgumentQuery;

		public CommandParameters(string query)
		{
			Query = query;

			var args = query.Split(' ');
			Keyword = args[0];

			if (args.Length == 1)
				Arguments = Array.Empty<string>();
			else
				Arguments = args.AsSpan(1, args.Length - 1).ToArray();

			ArgumentQuery = string.Join(' ', Arguments);
		}
	}
}
