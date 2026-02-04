using Azalea.Amends;
using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Design.Shapes;
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

		private readonly Box _backgroundObject;
		private readonly SpriteText _titleText;

		public EditorDockingContainerTab(string title, bool focused, float heigth)
		{
			_focused = focused;

			AddRange([
				_backgroundObject = new Box(){
					RelativeSizeAxes = Axes.Both,
					Color = focused ? __focusedColor : __navigationColor
				},
				_titleText = new SpriteText()
				{
					Font = FontUsage.Default.With(size: 18),
					Text = title,
					Anchor = Anchor.Center,
					Origin = Anchor.Center,
				}
			]);

			Size = new(_titleText.Width + 30, heigth);
		}

		protected override bool OnHover(HoverEvent e)
		{
			if (_focused) return false;

			_backgroundObject.FinishAmends();
			_backgroundObject.RecolorTo(__hoverColor, __animationSpeed);

			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			if (_focused) return;

			_backgroundObject.FinishAmends();
			_backgroundObject.RecolorTo(__navigationColor, __animationSpeed);
		}
	}
}
