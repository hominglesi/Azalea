using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.IO;

namespace Azalea.VisualTests;
public class ReflectedDataTest : TestScene
{
	public ReflectedDataTest()
	{
		AzaleaGame.Main.Host.Renderer.ClearColor = new Color(30, 30, 30);
		Add(new ScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Child = new DirectoryContainer(Assets.ReflectedStore)
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
			}
		});
	}

	private class DirectoryContainer : FlexContainer
	{
		private Storage _storage;
		private string _subPath = "";
		public DirectoryContainer(Storage storage)
		{
			_storage = storage;

			Wrapping = FlexWrapping.Wrap;
			Spacing = new(20);

			displayDirectory(_subPath);
		}

		private void addFile(string path)
		{
			var iconName = Path.GetFileName(path);
			var iconExtention = Path.GetExtension(path);
			var icon = createIcon(iconName, Assets.GetTexture("Textures/fileIcon.png"));

			switch (iconExtention)
			{
				case ".txt":
				case ".cfg":
				case ".cs":
					icon.Click += _ =>
					{
						using var reader = new StreamReader(path);
						Console.WriteLine(reader.ReadToEnd());
					};
					break;
			}

			Add(icon);
		}

		private void addDirectory(string path)
		{
			var directoryInfo = new DirectoryInfo(path);
			var directory = createIcon("/" + directoryInfo.Name, Assets.GetTexture("Textures/directoryIcon.png"));
			directory.Click += _ =>
			{
				_subPath = Path.Combine(_subPath, directoryInfo.Name);
				displayDirectory(_subPath);
			};
			Add(directory);
		}

		private void addReturn()
		{
			var returnIcon = createIcon("..", Assets.GetTexture("Textures/directoryIcon.png"));
			returnIcon.Click += _ =>
			{
				_subPath = FileSystemUtils.CombinePaths(_subPath, "../");
				displayDirectory(_subPath);
			};
			Add(returnIcon);
		}

		private Composition createIcon(string name, Texture icon)
		{
			return new Composition()
			{
				Size = new(150, 130),
				Children = new GameObject[]
				{
					new Sprite()
					{
						Size = new(100),
						Origin = Anchor.Center,
						Anchor = Anchor.Center,
						Y = -10,
						Texture = icon
					},
					new SpriteText()
					{
						Origin = Anchor.BottomCenter,
						Anchor = Anchor.BottomCenter,
						Y = -5,
						Text = name
					}
				}
			};
		}

		private void displayDirectory(string path)
		{
			Clear();

			if (path != "")
				addReturn();

			var resources = _storage.GetContainedResources(path);
			foreach (var resource in resources)
			{
				var resourceAttributes = File.GetAttributes(resource);

				if (resourceAttributes.HasFlag(FileAttributes.Directory))
					addDirectory(resource);
				else
					addFile(resource);
			}
		}
	}

}
