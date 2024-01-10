using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs.Events;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.Containers;
public class ScrollableContainer : Composition
{
	private CompositeGameObject _internalComposition;

	private Slider _slider;
	public Slider ScrollBar => _slider;

	public ScrollableContainer()
	{
		Masking = true;
		AddInternal(_internalComposition = new CompositeGameObject()
		{
			AutoSizeAxes = Axes.Y
		});
		AddInternal(_slider = CreateSlider());

		_slider.OnValueChanged += onSliderMoved;
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
		ScrollPosition = MathUtils.Map(newPosition, 0, 1, 0, -(_internalComposition.DrawHeight - DrawHeight));
	}

	protected override IReadOnlyList<GameObject> PublicChildren => _internalComposition.InternalChildren;
	public override void Add(GameObject gameObject) => _internalComposition.AddInternal(gameObject);
	public override bool Remove(GameObject gameObject) => _internalComposition.RemoveInternal(gameObject);
	public override void Clear() => _internalComposition.ClearInternal();

	public float ScrollSpeed { get; set; } = 30;

	private float _scrollPosition;
	public float ScrollPosition
	{
		get => _scrollPosition;
		set
		{
			if (_scrollPosition == value) return;

			_scrollPosition = clampWithinBoundaries(value);
			_internalComposition.Position = new Vector2(0, _scrollPosition);
		}
	}

	protected override void OnScroll(ScrollEvent e)
	{
		if (Hovered == false) return;

		ScrollPosition += e.ScrollDelta * ScrollSpeed;

		_slider.Value = MathUtils.Map(ScrollPosition, _scrollRange.X, _scrollRange.Y, 0, 1);
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

	private Vector2 _lastDrawSize;
	private Vector2 _lastInternalDrawSize;

	protected override void Update()
	{
		if (_lastDrawSize != DrawSize || _lastInternalDrawSize != _internalComposition.DrawSize)
		{
			updateChildLayout();
		}
	}

	protected override void UpdateAfterChildren()
	{
		if (_lastDrawSize != DrawSize || _lastInternalDrawSize != _internalComposition.DrawSize)
		{
			_scrollRange = getScrollRange(DrawHeight, _internalComposition.DrawHeight);
			updateSliderHeight();
			onSliderMoved(_slider.Value);

			_lastDrawSize = DrawSize;
			_lastInternalDrawSize = _internalComposition.DrawSize;
		}
	}

	private void updateSliderHeight()
	{
		if (_scrollRange.X == 0 && _scrollRange.Y == 0)
		{
			_slider.Alpha = 0;
		}
		else
		{
			_slider.Alpha = 1;
			_slider.Head.Height = DrawSize.Y / _internalComposition.DrawSize.Y;
		}
	}

	private void updateChildLayout()
	{
		_internalComposition.Width = DrawSize.X;
		if (_slider.Alpha > 0)
			_internalComposition.Width -= _slider.DrawWidth;
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
