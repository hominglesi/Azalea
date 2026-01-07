using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Editor.Design;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;

namespace Azalea.Editor.Views.ResourceExploring.Views;
internal class LargeIconsExplorerView : ResourceExplorer
{
	private readonly Composition _contentContainer = new FlexContainer()
	{
		RelativeSizeAxes = Axes.X,
		AutoSizeAxes = Axes.Y,
		Direction = FlexDirection.Horizontal,
		Wrapping = FlexWrapping.Wrap,
		Spacing = new(12, 4)
	};

	public LargeIconsExplorerView(IResourceStore store) : base(store)
	{
		Add(new EditorScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Child = _contentContainer
		});
	}

	protected override void AddResourceItem(string path)
	{
		IconItem item;
		bool isDirectory = path[^1] == '\\';

		if (_itemCache.Count > 0)
		{
			item = _itemCache[^1];
			_itemCache.RemoveAt(_itemCache.Count - 1);
			item.SetPath(path, isDirectory);
		}
		else
		{
			item = new IconItem(this, path, isDirectory);
		}

		_contentContainer.Add(item);
	}

	private readonly List<IconItem> _itemCache = [];
	protected override void ClearResourceItems()
	{
		foreach (var item in _contentContainer.Children)
			if (item is IconItem lItem)
				_itemCache.Add(lItem);

		_contentContainer.Clear();
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Left).Down)
			MoveBackward();

		if (Input.GetKey(Keys.Right).Down)
			MoveForward();
	}

	private class IconItem : Composition
	{
		private const float __itemWidth = 104;
		private const float __itemHeight = 116;
		private readonly ResourceExplorer _explorer;
		private readonly Sprite _iconDisplay;
		private readonly SpriteTextTruncating _pathDisplay;

		private string _path;
		private bool _isDirectory;

		public IconItem(ResourceExplorer explorer, string path, bool isDirectory)
		{
			_explorer = explorer;
			_path = path;
			_isDirectory = isDirectory;

			BackgroundColor = EditorPallete.HoverBackground;
			BackgroundObject!.Alpha = 0;

			BorderColor = EditorPallete.HoverBorderBackground;
			BorderThickness = 1;
			BorderObject!.OutsideContent = false;
			BorderObject!.Alpha = 0;

			Width = __itemWidth;
			Height = __itemHeight;
			Children = [
				_iconDisplay = new Sprite()
				{
					Anchor = Anchor.TopCenter,
					Origin = Anchor.TopCenter,
					Y = 4,
					Size = new(92),
					Texture = getIcon(isDirectory)
				},
				_pathDisplay = new SpriteTextTruncating()
				{
					Y = -4,
					Width = __itemWidth - 4,
					Anchor = Anchor.BottomCenter,
					Origin = Anchor.BottomCenter,
					Text = path
				}
			];
		}

		private static Texture getIcon(bool isDirectory)
			=> Assets.GetTexture($"Textures/{(isDirectory ? "directory" : "file")}-icon.png");

		public void SetPath(string path, bool isDirectory)
		{
			_path = path;
			_isDirectory = isDirectory;

			_iconDisplay.Texture = getIcon(isDirectory);
			_pathDisplay.Text = path;
		}

		protected override bool OnClick(ClickEvent e)
		{
			if (_isDirectory)
			{
				_explorer.WriteToHistory(_path);
				_explorer.SetDisplayedDirectory(_path);
			}

			return true;
		}

		protected override bool OnHover(HoverEvent e)
		{
			BackgroundObject!.Alpha = 1;
			BorderObject!.Alpha = 1;

			return base.OnHover(e);
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			BackgroundObject!.Alpha = 0;
			BorderObject!.Alpha = 0;

			base.OnHoverLost(e);
		}
	}
}
