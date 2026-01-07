using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;

namespace Azalea.Design.Explorer;
internal class ResourceExplorerIcons : ResourceExplorer
{
	public ResourceExplorerIcons()
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;
		Direction = FlexDirection.Horizontal;
		Wrapping = FlexWrapping.Wrap;
		Spacing = new(12, 4);
	}

	protected override ResourceItem AddItem(string path, bool isDirectory)
	{
		var item = new IconItem(path, isDirectory);

		Add(item);
		return item;
	}

	protected override ResourceItem AddReturn()
	{
		var item = new IconItem("..", true);

		Add(item);
		return item;
	}

	protected override void ClearItems()
		=> Clear();

	private class IconItem : ResourceItem
	{
		private const float __itemWidth = 104;
		private const float __itemHeight = 116;

		public IconItem(string path, bool isDirectory)
			: base(path, isDirectory)
		{
			BackgroundColor = EditorPallete.HoverBackground;
			BackgroundObject!.Alpha = 0;

			BorderColor = EditorPallete.HoverBorderBackground;
			BorderThickness = 1;
			BorderObject!.OutsideContent = false;
			BorderObject!.Alpha = 0;

			Width = __itemWidth;
			Height = __itemHeight;
			Children = [
				new Sprite()
				{
					Anchor = Anchor.TopCenter,
					Origin = Anchor.TopCenter,
					Y = 4,
					Size = new(92),
					Texture = Assets.MainStore.GetTexture(
						$"Textures/{(isDirectory ? "directory" : "file")}-icon.png",
						Graphics.Textures.TextureFiltering.Linear)
				},
				new SpriteTextTruncating()
				{
					Y = -4,
					Width = __itemWidth - 4,
					Anchor = Anchor.BottomCenter,
					Origin = Anchor.BottomCenter,
					Text = path
				}
			];
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
