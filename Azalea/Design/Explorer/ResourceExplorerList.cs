using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;

namespace Azalea.Design.Explorer;
internal class ResourceExplorerList : ResourceExplorer
{
	public ResourceExplorerList()
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;
		Direction = FlexDirection.Vertical;
		Spacing = new(0, 4);
	}

	protected override ResourceItem AddItem(string path, bool isDirectory)
	{
		var item = new ListItem(path, isDirectory)
		{
			RelativeSizeAxes = Axes.X,
			NegativeSize = new(24, 0),
		};

		Add(item);
		return item;
	}

	protected override ResourceItem AddReturn()
	{
		var item = new ListItem("..", true)
		{
			RelativeSizeAxes = Axes.X,
			NegativeSize = new(16, 0)
		};

		Add(item);
		return item;
	}

	protected override void ClearItems()
		=> Clear();

	private class ListItem : ResourceItem
	{
		private const float __itemHeight = 24;

		public ListItem(string path, bool isDirectory)
			: base(path, isDirectory)
		{
			BackgroundColor = EditorPallete.HoverBackground;
			BackgroundObject!.Alpha = 0;

			BorderColor = EditorPallete.HoverBorderBackground;
			BorderThickness = 1;
			BorderObject!.OutsideContent = false;
			BorderObject!.Alpha = 0;

			Height = __itemHeight;
			Children = new GameObject[]
			{
				new Sprite()
				{
					Anchor = Anchor.CenterLeft,
					Origin = Anchor.CenterLeft,
					X = 4,
					Texture = Assets.GetTexture($"Textures/{(isDirectory ? "directory" : "file")}-icon-small.png")
				},
				new SpriteText()
				{
					Anchor = Anchor.CenterLeft,
					Origin = Anchor.CenterLeft,
					X = 28,
					Text = path
				}
			};
		}

		protected override bool OnClick(ClickEvent e)
		{
			InvokeClicked();
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
