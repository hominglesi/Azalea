using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;

namespace Azalea.Debugging;
public class DebugSelectPointer : Sprite
{
	public DebugSelectPointer()
	{
		Texture = Assets.GetTexture("Textures/bullseye-pointer.png");
		Color = Palette.Black;
	}

	protected override bool OnClick(ClickEvent e)
	{
		var overlay = new SelectOverlay()
		{
			ClickAction = (_) => onSelectClicked()
		};
		AzaleaGame.Main.Host.Root.AddInternal(overlay);
		return base.OnClick(e);
	}

	private void onSelectClicked()
	{
		var hoveredObjects = Input.GetHoveredObjects(recalculate: true);

		for (int i = 0; i < hoveredObjects.Count; i++)
		{
			var hoveredObject = hoveredObjects[i];
			if (hoveredObject.Parent is not null)
			{
				Editor.InspectObject(hoveredObject);
				Editor.HighlightObject(hoveredObject);
				break;
			}
		}
	}

	protected override bool OnHover(HoverEvent e)
	{
		Color = Palette.Flowers.Azalea;
		return true;
	}

	protected override void OnHoverLost(HoverLostEvent e)
	{
		Color = Palette.Black;
	}

	private class SelectOverlay : Composition
	{
		public SelectOverlay()
		{
			Depth = -1000000;
			RelativeSizeAxes = Axes.Both;
			BackgroundColor = new Color(32, 32, 32, 32);
		}

		protected override bool OnHover(HoverEvent e)
		{
			return true;
		}

		protected override bool OnClick(ClickEvent e)
		{
			Parent?.RemoveInternal(this);
			return true;
		}
	}
}
