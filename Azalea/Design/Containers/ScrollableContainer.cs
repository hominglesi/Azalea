using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs.Events;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.Design.Containers;
public class ScrollableContainer : ContentContainer
{
	public Slider ScrollBar { get; init; }

	public ScrollableContainer()
	{
		Masking = true;

		ContentComposition.RelativeSizeAxes = Axes.None;
		ContentComposition.AutoSizeAxes = Axes.Y;
		AddInternal(ScrollBar = CreateSlider());

		ScrollBar.OnValueChanged += onSliderMoved;
	}

	protected Slider CreateSlider()
		=> new DefaultScrollableSlider()
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			Direction = SliderDirection.Vertical,
			RelativeSizeAxes = Axes.Y,
			Height = 1f,
			Width = 20,
		};

	private void onSliderMoved(float newPosition)
	{
		ScrollPosition = MathUtils.Map(newPosition, 0, 1, 0, -(ContentComposition.DrawHeight - DrawHeight));
	}

	public float ScrollSpeed { get; set; } = 30;

	private float _scrollPosition;
	public float ScrollPosition
	{
		get => _scrollPosition;
		set
		{
			if (_scrollPosition == value) return;

			_scrollPosition = clampWithinBoundaries(value);
			ContentComposition.Position = new Vector2(0, _scrollPosition);
		}
	}

	protected override void OnScroll(ScrollEvent e)
	{
		if (Hovered == false) return;

		ScrollPosition += e.ScrollDelta * ScrollSpeed;

		ScrollBar.Value = MathUtils.Map(ScrollPosition, _scrollRange.X, _scrollRange.Y, 0, 1);
	}

	private float clampWithinBoundaries(float position)
	{
		var x = Math.Clamp(position, _scrollRange.Y, _scrollRange.X);

		return x;
	}

	private Vector2 _scrollRange;
	private Vector2 getScrollRange(float scrollHeight, float contentHeight)
	{
		var range = new Vector2(0, -(contentHeight - scrollHeight));


		if (range.Y > 0) range.Y = 0;
		return range;
	}

	protected override void UpdateContentLayout()
	{
		updateChildLayout();

		_scrollRange = getScrollRange(DrawHeight, ContentComposition.DrawHeight);
		updateSliderHeight();
		onSliderMoved(ScrollBar.Value);
	}

	private void updateSliderHeight()
	{
		if (_scrollRange.X == 0 && _scrollRange.Y == 0)
		{
			ScrollBar.Alpha = 0;
		}
		else
		{
			ScrollBar.Alpha = 1;
			ScrollBar.Head.Height = DrawSize.Y / ContentComposition.DrawSize.Y;
		}
	}

	private void updateChildLayout()
	{
		ContentComposition.Width = DrawSize.X;
		if (ScrollBar.Alpha > 0)
			ContentComposition.Width -= ScrollBar.DrawWidth;
	}

	private class DefaultScrollableSlider : Slider
	{
		protected override GameObject CreateBody()
			=> new Box()
			{
				Color = Palette.Gray,
				RelativeSizeAxes = Axes.Both
			};

		protected override GameObject CreateHead()
			=> new Box()
			{
				Origin = Anchor.Center,
				Color = Palette.Blue,
				RelativeSizeAxes = Axes.Both,
				Height = 0.2f,
			};
	}
}
