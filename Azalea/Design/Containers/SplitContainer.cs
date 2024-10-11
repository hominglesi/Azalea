using Azalea.Graphics;
using System.Numerics;

namespace Azalea.Design.Containers;
public class SplitContainer : Composition
{
	public SplitDirection Direction { get; set; } = SplitDirection.Horizontal;
	public bool ReversedPriority { get; set; } = false;

	private GameObject _firstObject;
	private GameObject _secondObject;
	public SplitContainerLine SplitLine { get; init; }

	private float _minSize = 150;

	public SplitContainer(GameObject firstObject, GameObject secondObject)
	{
		_firstObject = firstObject;
		_secondObject = secondObject;

		SplitLine = CreateSplitLine();

		AddRange(new GameObject[] { _firstObject, _secondObject, SplitLine });
	}

	private void updateLayout()
	{
		if (Direction == SplitDirection.Horizontal)
		{
			var firstWidth = SplitLine.X - (SplitLine.DrawWidth / 2);
			var secondWidth = DrawWidth - firstWidth - SplitLine.DrawWidth;
			var secondOffset = firstWidth + SplitLine.DrawWidth;

			_firstObject.Position = Vector2.Zero;
			_firstObject.RelativeSizeAxes = Axes.Y;
			_firstObject.Size = new(firstWidth, 1);

			_secondObject.Position = new(secondOffset, 0);
			_secondObject.RelativeSizeAxes = Axes.Y;
			_secondObject.Size = new(secondWidth, 1);
		}
		else
		{
			var firstHeight = SplitLine.Y - (SplitLine.DrawHeight / 2);
			var secondHeight = DrawHeight - firstHeight - SplitLine.DrawHeight;
			var secondOffset = firstHeight + SplitLine.DrawHeight;

			_firstObject.Position = Vector2.Zero;
			_firstObject.RelativeSizeAxes = Axes.X;
			_firstObject.Size = new(1, firstHeight);

			_secondObject.Position = new(0, secondOffset);
			_secondObject.RelativeSizeAxes = Axes.X;
			_secondObject.Size = new(1, secondHeight);
		}
	}

	private Vector2 _lastSplitPosition;
	private Vector2 _lastDrawSize;
	private SplitDirection _lastDirection = SplitDirection.Horizontal;
	private bool _lastReversedPriority = false;
	protected override void Update()
	{
		if (DrawSize == _lastDrawSize && _lastDirection == Direction && _lastReversedPriority == ReversedPriority && SplitLine.Position == _lastSplitPosition)
			return;

		var maxScroll = Direction == SplitDirection.Horizontal ? DrawWidth : DrawHeight;
		var scrollRange = new Vector2(0, maxScroll);

		if (maxScroll - (_minSize * 2) > 0)
		{
			scrollRange.X += _minSize;
			scrollRange.Y -= _minSize;
		}

		float lastValue;
		if (_lastDirection == SplitDirection.Horizontal)
		{
			lastValue = SplitLine.X;
			if (_lastReversedPriority)
				lastValue = _lastDrawSize.X - lastValue;
		}
		else
		{
			lastValue = SplitLine.Y;
			if (_lastReversedPriority)
				lastValue = _lastDrawSize.Y - lastValue;
		}

		var newValue = lastValue;
		if (ReversedPriority)
		{
			var newValueMax = Direction == SplitDirection.Horizontal ? DrawWidth : DrawHeight;
			newValue = newValueMax - newValue;
		}

		SplitLine.UpdateLayout(Direction, scrollRange, newValue);
		updateLayout();

		_lastSplitPosition = SplitLine.Position;
		_lastDirection = Direction;
		_lastDrawSize = DrawSize;
		_lastReversedPriority = ReversedPriority;
	}

	public abstract class SplitContainerLine : DraggableContainer
	{
		public abstract void UpdateLayout(SplitDirection direction, Vector2 scrollRange, float newValue);
	}

	protected virtual SplitContainerLine CreateSplitLine()
		=> new DefaultSplitContainerLine();

	public class DefaultSplitContainerLine : SplitContainerLine
	{
		public DefaultSplitContainerLine()
		{
			X = 300;
		}

		public override void UpdateLayout(SplitDirection direction, Vector2 scrollRange, float newValue)
		{
			if (direction == SplitDirection.Horizontal)
			{
				Origin = Anchor.TopCenter;
				X = newValue;
				Y = 0;
				RelativeSizeAxes = Axes.Y;
				Size = new(0, 1);
				DragBoundary = new(0, scrollRange.Y, 0, scrollRange.X);

				DragAreaObject.Origin = Anchor.TopCenter;
				DragAreaObject.RelativeSizeAxes = Axes.Y;
				DragAreaObject.Size = new(10, 1);
			}
			else
			{
				Origin = Anchor.CenterLeft;
				X = 0;
				Y = newValue;
				RelativeSizeAxes = Axes.X;
				Size = new(1, 0);
				DragBoundary = new(scrollRange.X, 0, scrollRange.Y, 0);

				DragAreaObject.Origin = Anchor.CenterLeft;
				DragAreaObject.RelativeSizeAxes = Axes.X;
				DragAreaObject.Size = new(1, 10);
			}
		}
	}
}

public enum SplitDirection
{
	Horizontal,
	Vertical
}
