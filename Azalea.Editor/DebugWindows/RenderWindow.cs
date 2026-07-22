using Azalea.Design.Containers;
using Azalea.Editor.Design.Gui;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using System.Collections.Generic;

namespace Azalea.Editor.DebugWindows;
internal static class RenderWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static readonly Dictionary<ITexture, Sprite> _loadedTextures = [];
	private static Composition? _loadedTexturesContainer;

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Rendering", new(100), new(400, 400));
			_window.AddGroup("Loaded Textures");
			_window.Add(_loadedTexturesContainer = new FlexContainer()
			{
				Direction = FlexDirection.Horizontal,
				Wrapping = FlexWrapping.Wrap,
				Spacing = new(10),
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
			});

			foreach (var texture in ITexture.LoadedTextures)
			{
				var textureSprite = new Sprite()
				{
					Size = new(100),
					Texture = texture
				};

				_loadedTexturesContainer.Add(textureSprite);
				_loadedTextures.Add(texture, textureSprite);
			}
			_window.FinishGroup();
		}

		_window.Show();
	}

	private static void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
