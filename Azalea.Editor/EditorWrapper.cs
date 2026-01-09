using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Design.Scenes;
using Azalea.Editor.Views.MsdfGen;
using Azalea.Editor.Views.ResourceExploring;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.Editor;

public class EditorWrapper : AzaleaGame
{
	private readonly AzaleaGame? _wrappedGame;

	private readonly BasicDockingContainer _mainContainer;

	public EditorWrapper(AzaleaGame? wrappedGame)
	{
		_wrappedGame = wrappedGame;
		SceneManager.ChangeScene(null);

		Add(_mainContainer = new BasicDockingContainer()
		{
			RelativeSizeAxes = Axes.Both
		});
		_mainContainer.ContentBackground.Color = Palette.White;

		_mainContainer.AddDockable("Game", _wrappedGame is not null ? _wrappedGame : new MissingGameDisplay()
		{
			RelativeSizeAxes = Axes.Both,
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

	private class MissingGameDisplay : Composition
	{
		public MissingGameDisplay()
		{
			Add(new SpriteText()
			{
				Text = "No game provided for editor.",
				Anchor = Anchor.Center,
				Origin = Anchor.Center
			});
		}
	}
}
