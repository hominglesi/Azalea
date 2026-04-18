using Azalea.Design.Containers;
using Azalea.Design.Controls;
using Azalea.Design.Docking;
using Azalea.Design.Scenes;
using Azalea.Editor.Views.MsdfGen;
using Azalea.Editor.Views.ResourceExploring;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System;
using System.Diagnostics;

namespace Azalea.Editor;

public class EditorWrapper : AzaleaGame
{
	private static EditorWrapper? _instance;
	public static EditorWrapper Instance
	{
		get
		{
			return _instance ??= new EditorWrapper();
		}
	}

	private readonly BasicDockingContainer _mainContainer;
	private readonly Composition _gameDockable;

	internal EditorWrapper()
	{
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
			Child = new MissingGameDisplay()
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
	}

	private AzaleaGame? _wrappedGame;
	public static AzaleaGame Wrap(AzaleaGame game)
	{
		if (Instance._wrappedGame is not null)
			throw new Exception("Only one game can be wrapped by the editor");

		Instance._wrappedGame = game;

		if (game is not null)
			Instance._gameDockable.Child = game;

		return Instance;
	}

	bool _sceneContainerHandedOver = false;

	protected override void Update()
	{
		if (_sceneContainerHandedOver == false && _wrappedGame is not null)
		{
			GameObject? sceneContainer = null;

			foreach (var child in InternalChildren)
				if (child is SceneContainer)
				{
					sceneContainer = child;
					break;
				}

			Debug.Assert(sceneContainer is not null);

			RemoveInternal(sceneContainer);
			_wrappedGame.AddInternal(sceneContainer);
			_sceneContainerHandedOver = true;
		}
	}

	private class MissingGameDisplay : Composition
	{
		public MissingGameDisplay()
		{
			RelativeSizeAxes = Axes.Both;
			Add(new SpriteText()
			{
				Text = "No game provided for editor.",
				Color = ControlConstants.DarkTextColor,
				Anchor = Anchor.Center,
				Origin = Anchor.Center
			});
		}
	}
}
