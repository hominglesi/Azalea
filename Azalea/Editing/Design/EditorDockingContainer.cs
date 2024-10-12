using Azalea.Amends;
using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;

namespace Azalea.Editing.Design;
internal class EditorDockingContainer : BasicDockingContainer
{
	private static Color __navigationColor = new(30);
	private static Color __focusedColor = Palette.Gray;
	private static Color __hoverColor = new(90);

	public EditorDockingContainer(Boundary padding, bool backgroundHidden = false)
	{
		ContentBackground.Color = new Color(100);
		ContentBackground.Alpha = backgroundHidden ? 0 : 1;
		NavigationBackground.Color = __navigationColor;
		ContentPadding = padding;
		NavigationHeight = 22;
	}

	protected override GameObject CreateNavigationTab(string name, bool focused)
		=> new EditorDockingContainerTab(name, focused, NavigationHeight);

	protected class EditorDockingContainerTab : Composition
	{
		private const float __animationSpeed = 0.1f;
		private readonly bool _focused;
		public EditorDockingContainerTab(string title, bool focused, float heigth)
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
