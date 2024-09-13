using Azalea.Debugging.Design;
using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.IO;

namespace Azalea.Debugging;
public class ResourceExplorer : ScrollableContainer
{
	public ResourceExplorer()
	{
		RelativeSizeAxes = Axes.Both;
		Add(new DirectoryContainer(Assets.ReflectedStore)
		{
			RelativeSizeAxes = Axes.X,
			AutoSizeAxes = Axes.Y,
		});
	}

	protected override Slider CreateSlider() => new DebugScrollbar();

	private class DirectoryContainer : FlexContainer
	{
		private Storage _storage;
		private string _subPath = "";
		public DirectoryContainer(Storage storage)
		{
			_storage = storage;

			Wrapping = FlexWrapping.Wrap;
			Spacing = new(12, 6);

			displayDirectory(_subPath);
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
				Size = new(105, 120),
				Children = new GameObject[]
				{
					new Sprite()
					{
						Size = new(90),
						Origin = Anchor.TopCenter,
						Anchor = Anchor.TopCenter,
						Y = 3,
						Texture = icon
					},
					new SpriteText()
					{
						Origin = Anchor.BottomCenter,
						Anchor = Anchor.BottomCenter,
						Y = -5,
						Text = name,
						Font = FontUsage.Default.With(size: 16)
					}
				}
			};
		}
	}
}
