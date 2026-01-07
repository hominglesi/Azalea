using Azalea.Design.Containers;
using Azalea.Editing;
using Azalea.Editor.Views.ResourceExploring.Views;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;

namespace Azalea.Editor.Views.ResourceExploring;
public class EditorResourceExplorer : Composition
{
	private const float __headerHeight = 44;
	private const float __headerMiniIconSize = 22;

	private readonly IResourceStore _store;

	private readonly HeaderButton _previousButton;
	private readonly HeaderButton _nextButton;

	private readonly HeaderButton _detailsButton;
	private readonly HeaderButton _largeIconsButton;
	private HeaderButton _pressedHeader;

	private readonly Composition _viewContainer;
	private readonly SpriteText _pathText;
	private readonly DetailsExplorerView _detailsExplorer;
	private LargeIconsExplorerView? _largeIconsExplorer;

	public EditorResourceExplorer(IResourceStore store)
	{
		_store = store;

		RelativeSizeAxes = Axes.Both;
		AddRange([
			new Composition
			{
				RelativeSizeAxes = Axes.X,
				Height = __headerHeight,
				BackgroundColor = Palette.Gray,
				Children = [
					new FlexContainer(){
						Width = __headerHeight * 2,
						Height = __headerHeight,
						Children = [
							_previousButton = new HeaderButton("left", __headerHeight,
								_ => getViewFromButton(_pressedHeader!).MoveBackward()),
							_nextButton = new HeaderButton("right", __headerHeight,
								_ => getViewFromButton(_pressedHeader!).MoveForward())
						]
					},
					_pathText = new SpriteText()
					{
						X = (__headerHeight * 2) + 8,
						Origin = Anchor.CenterLeft,
						Anchor = Anchor.CenterLeft
					},
					new FlexContainer()
					{
						Width = __headerMiniIconSize * 2,
						Height = __headerMiniIconSize,
						Origin = Anchor.CenterRight,
						Anchor = Anchor.CenterRight,
						Children = [
							_largeIconsButton = new HeaderButton("icons", __headerMiniIconSize,
								headerPressed),
							_detailsButton = new HeaderButton("list", __headerMiniIconSize,
								headerPressed),
						]
					}
				]
			},
			_viewContainer = new Composition()
			{
				RelativeSizeAxes = Axes.Both,
				NegativeSize = new(0, __headerHeight + 8),
				Y = __headerHeight + 8,
				Child = _detailsExplorer = new DetailsExplorerView(_store){
					RelativeSizeAxes = Axes.Both
				}
			}
		]);

		_pressedHeader = _detailsButton;
		_pressedHeader.SetPressed(true);
		_pathText.Text = _detailsExplorer.DisplayedDirectory;
		_detailsExplorer.DisplayDirectoryChanged += updatePath;
	}

	private void headerPressed(HeaderButton sender)
	{
		if (_pressedHeader == sender)
			return;

		var currentView = getViewFromButton(_pressedHeader);
		var nextView = getViewFromButton(sender);

		_pressedHeader.SetPressed(false);
		sender.SetPressed(true);

		_viewContainer.Remove(currentView);
		_viewContainer.Add(nextView);

		nextView.SyncData(currentView);
		_pressedHeader = sender;
	}

	private void updatePath(string path)
		=> _pathText.Text = path;

	private ResourceExplorer getViewFromButton(HeaderButton button)
	{
		if (button == _largeIconsButton)
		{
			if (_largeIconsExplorer is not null)
				return _largeIconsExplorer;

			_largeIconsExplorer = new LargeIconsExplorerView(_store)
			{
				RelativeSizeAxes = Axes.Both
			};
			_largeIconsExplorer.DisplayDirectoryChanged += updatePath;
			return _largeIconsExplorer;
		}

		return _detailsExplorer;
	}

	private class HeaderButton : Composition
	{
		private readonly Action<HeaderButton> _clickAction;

		public HeaderButton(string iconName, float size, Action<HeaderButton> clickAction)
		{
			_clickAction = clickAction;
			Size = new(size);

			Add(new Sprite()
			{
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				Texture = Assets.GetTexture($"Textures/{iconName}-icon-small.png")
			});
		}

		public void SetPressed(bool pressed)
		{
			if (pressed)
			{
				BackgroundColor = EditorPallete.HoverBackground;
				BackgroundObject!.Alpha = 1;
				BorderColor = EditorPallete.HoverBorderBackground;
				BorderThickness = 1;
				BorderObject!.Alpha = 1;
			}
			else
			{
				if (BackgroundObject is not null)
					BackgroundObject.Alpha = 0;

				if (BorderObject is not null)
					BorderObject.Alpha = 0;
			}
		}

		protected override bool OnClick(ClickEvent e)
		{
			_clickAction?.Invoke(this);
			return true;
		}
	}
}
