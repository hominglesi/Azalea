using Azalea.Amends;
using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;

namespace Azalea.Debugging.Design;
public class DebugDockingContainer : BasicDockingContainer
{
	private static Color __navigationColor = new(30);
	private static Color __focusedColor = Palette.Gray;
	private static Color __hoverColor = new(90);

	public DebugDockingContainer()
	{
		NavigationBackground.Color = __navigationColor;
		ContentPadding = new(0);
		NavigationHeight = 22;
	}

	protected override GameObject CreateNavigationTab(string name, bool focused)
		=> new DebugDockingContainerTab(name, focused, NavigationHeight);

	protected class DebugDockingContainerTab : Composition
	{
		private const float __animationSpeed = 0.1f;
		private readonly bool _focused;
		public DebugDockingContainerTab(string title, bool focused, float heigth)
		{
			_focused = focused;

			var titleText = new SpriteText()
			{
				Font = FontUsage.Default.With(size: 18),
				Text = title,
				Anchor = Anchor.Center,
				Origin = Anchor.Center,
			};

			Size = new(titleText.Width + 30, heigth);
			BackgroundColor = focused ? __focusedColor : __navigationColor;
			Add(titleText);
		}

		protected override bool OnHover(HoverEvent e)
		{
			if (_focused) return false;

			BackgroundObject!.FinishAmends();
			BackgroundObject!.RecolorTo(__hoverColor, __animationSpeed);

			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			if (_focused) return;

			BackgroundObject!.FinishAmends();
			BackgroundObject!.RecolorTo(__navigationColor, __animationSpeed);
		}
	}
}
