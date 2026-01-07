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
internal class DetailsExplorerView : ResourceExplorer
{
	private readonly Composition _contentContainer = new FlexContainer()
	{
		RelativeSizeAxes = Axes.X,
		AutoSizeAxes = Axes.Y,
		Direction = FlexDirection.Vertical,
		Spacing = new(0, 4)
	};

	public DetailsExplorerView(IResourceStore store) : base(store)
	{
		Add(new EditorScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Child = _contentContainer
		});
	}

	protected override void AddResourceItem(string path)
	{
		ListItem item;
		bool isDirectory = path[^1] == '\\';

		if (_itemCache.Count > 0)
		{
			item = _itemCache[^1];
			_itemCache.RemoveAt(_itemCache.Count - 1);
			item.SetPath(path, isDirectory);
		}
		else
		{
			item = new ListItem(this, path, isDirectory)
			{
				RelativeSizeAxes = Axes.X,
				NegativeSize = new(24, 0),
			};
		}

		_contentContainer.Add(item);
	}

	private readonly List<ListItem> _itemCache = [];
	protected override void ClearResourceItems()
	{
		foreach (var item in _contentContainer.Children)
			if (item is ListItem lItem)
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

	private class ListItem : Composition
	{
		private const float __itemHeight = 24;
		private readonly ResourceExplorer _explorer;
		private readonly Sprite _iconDisplay;
		private readonly SpriteText _pathDisplay;

		private string _path;
		private bool _isDirectory;

		public ListItem(ResourceExplorer explorer, string path, bool isDirectory)
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

			Height = __itemHeight;
			Children = [
				_iconDisplay = new Sprite()
				{
					Anchor = Anchor.CenterLeft,
					Origin = Anchor.CenterLeft,
					X = 4,
					Texture = getIcon(isDirectory)
				},
				_pathDisplay = new SpriteText()
				{
					Anchor = Anchor.CenterLeft,
					Origin = Anchor.CenterLeft,
					X = 28,
					Text = path
				}
			];
		}

		private static Texture getIcon(bool isDirectory)
			=> Assets.GetTexture($"Textures/{(isDirectory ? "directory" : "file")}-icon-small.png");

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
