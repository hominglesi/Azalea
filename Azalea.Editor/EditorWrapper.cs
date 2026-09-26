using Azalea.Design.Containers;
using Azalea.Design.Controls;
using Azalea.Design.Docking;
using Azalea.Design.Scenes;
using Azalea.Editing;
using Azalea.Editor.DebugWindows;
using Azalea.Editor.Views.MsdfGen;
using Azalea.Editor.Views.ResourceExploring;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System;
using System.Diagnostics;

namespace Azalea.Editor;

public class EditorWrapper : AzaleaGame
{
	private readonly AzaleaGame _wrappedGame;
	private readonly BasicDockingContainer _mainContainer;
	private readonly Composition _gameDockable;

	internal EditorWrapper(AzaleaGame game)
	{
		_wrappedGame = game;

		Assets.AddToMainStore(new NamespacedResourceStore(new EmbeddedResourceStore(typeof(EditorWrapper).Assembly), "Resources"));

		Add(_mainContainer = new BasicDockingContainer()
		{
			RelativeSizeAxes = Axes.Both,
			ContentPadding = 0
		});
		_mainContainer.ContentBackground.Color = Palette.White;

		_mainContainer.AddDockable("Game", _gameDockable = new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Child = new EditorContainer(game)
		});
		_mainContainer.AddDockable("Resource Explorer", new EditorResourceExplorer(Assets.FileSystemStore)
		{
			RelativeSizeAxes = Axes.Both,
			BackgroundColor = Palette.Gray
		});
		_mainContainer.AddDockable("Msdf Generator", new MsdfGenView()
		{
			RelativeSizeAxes = Axes.Both
		});

		_cameraWindow = new(_wrappedGame);
		_globalWindow = new(_wrappedGame);
		_renderWindow = new(_wrappedGame);
		_classWindow = new(_wrappedGame);
	}

	public static AzaleaGame Wrap(AzaleaGame game) => new EditorWrapper(game);

	private readonly CameraWindow _cameraWindow;
	private readonly GlobalWindow _globalWindow;
	private readonly RenderWindow _renderWindow;
	private readonly ClassWindow _classWindow;

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.F1 && e.State.ShiftPressed)
			_cameraWindow.Toggle();
		else if (e.Key == Keys.F2 && e.State.ShiftPressed)
			_globalWindow.Toggle();
		else if (e.Key == Keys.F3 && e.State.ShiftPressed)
			_renderWindow.Toggle();
		else if (e.Key == Keys.F4 && e.State.ShiftPressed)
			_classWindow.Toggle();
		else
			return false;

		return true;
	}
}
