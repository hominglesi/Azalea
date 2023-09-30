using Azalea.Inputs.Events;
using System;
using System.Numerics;

namespace Azalea.Design.Containers;
public class ScrollableContainer : Composition
{
	public float ScrollSpeed { get; set; } = 10;

	private float _scrollPosition;
	public float ScrollPosition
	{
		get => _scrollPosition;
		set
		{
			if (_scrollPosition == value) return;

			_scrollPosition = value;
		}
	}

	public ScrollableContainer()
	{
		Masking = true;
	}


	protected override void OnScroll(ScrollEvent e)
	{
		if (Hovered == false) return;

		scrollBy(e.ScrollDelta);
	}

	private void scrollBy(float scrollValue)
	{
		var scrollChange = scrollValue * ScrollSpeed;

		var newScrollPosition = InternalComposition.Position.Y + scrollChange;

		InternalComposition.Position = new Vector2(0, Math.Max(newScrollPosition, 0));
	}
}
