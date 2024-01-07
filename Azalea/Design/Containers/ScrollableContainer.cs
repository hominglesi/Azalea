using Azalea.Graphics;
using Azalea.Inputs.Events;
using System.Numerics;

namespace Azalea.Design.Containers;
public class ScrollableContainer : Composition
{
	public float ScrollSpeed { get; set; } = 30;

	private float _scrollPosition;
	public float ScrollPosition
	{
		get => _scrollPosition;
		set
		{
			if (_scrollPosition == value) return;

			scrollTo(value);
		}
	}

	private Boundary _contentBoundaries;
	public Boundary ContentBoundaries
	{
		get => _contentBoundaries;
		set
		{
			if (_contentBoundaries == value) return;

			_contentBoundaries = value;

			_scrollPosition = clampWithinBoundaries(new Vector2(0, _scrollPosition)).Y;
		}
	}

	public ScrollableContainer()
	{
		Masking = true;
	}


	protected override void OnScroll(ScrollEvent e)
	{
		if (Hovered == false) return;

		scrollBy(e.ScrollDelta * ScrollSpeed);
	}

	private void scrollBy(float scrollValue)
	{
		_scrollPosition = clampWithinBoundaries(new Vector2(0, _scrollPosition + scrollValue)).Y;

		InternalComposition.Position = new Vector2(0, _scrollPosition);
	}

	private void scrollTo(float scrollPosition)
	{
		_scrollPosition = clampWithinBoundaries(new Vector2(0, scrollPosition)).Y;

		InternalComposition.Position = new Vector2(0, _scrollPosition);
	}

	private Vector2 clampWithinBoundaries(Vector2 position)
	{
		if (position.Y > _contentBoundaries.Top)
			position.Y = _contentBoundaries.Top;

		if (-position.Y > _contentBoundaries.Bottom - DrawHeight)
			position.Y = -(_contentBoundaries.Bottom - DrawHeight);

		return position;
	}
}
