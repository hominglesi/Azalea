using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System;
using System.Numerics;

namespace Azalea.Design.Containers;
public class DraggableContainer : Composition
{
	private bool _isDragging;
	private Vector2 _lastPosition;

	public Action? PositionChanged;

	private Boundary _dragBoundary = new(float.MinValue, float.MaxValue, float.MaxValue, float.MinValue);
	public Boundary DragBoundary
	{
		get => _dragBoundary;
		set
		{
			if (_dragBoundary == value) return;
			_dragBoundary = value;

			var newPosition = _dragBoundary.ClampWithin(Position);
			changePosition(newPosition);
		}
	}

	private DragArea _dragArea;
	public GameObject DragAreaObject => _dragArea;

	public DraggableContainer()
	{
		AddInternal(_dragArea = new DragArea()
		{
			RelativeSizeAxes = Axes.Both
		});
		_dragArea.DragStarted += onDragStarted;
	}

	protected override void Update()
	{
		if (_isDragging == false)
			return;

		if (Input.GetMouseButton(MouseButton.Left).Released)
		{
			_isDragging = false;
			_dragOverflow = Vector2.Zero;
			return;
		}

		if (_lastPosition != Input.MousePosition)
		{
			var dragOffset = Input.MousePosition - _lastPosition;
			applyDragOffset(dragOffset);

			_lastPosition = Input.MousePosition;
		}
	}

	private Vector2 _dragOverflow;
	private void applyDragOffset(Vector2 dragOffset)
	{
		var newPosition = Position + dragOffset + _dragOverflow;

		if (newPosition.X > _dragBoundary.Right)
			_dragOverflow.X = newPosition.X - _dragBoundary.Right;
		else if (newPosition.X < _dragBoundary.Left)
			_dragOverflow.X = newPosition.X - _dragBoundary.Left;
		else
			_dragOverflow.X = 0;

		if (newPosition.Y > _dragBoundary.Bottom)
			_dragOverflow.Y = newPosition.Y - _dragBoundary.Bottom;
		else if (newPosition.Y < _dragBoundary.Top)
			_dragOverflow.Y = newPosition.Y - _dragBoundary.Top;
		else
			_dragOverflow.Y = 0;

		newPosition.X = Math.Clamp(newPosition.X, _dragBoundary.Left, _dragBoundary.Right);
		newPosition.Y = Math.Clamp(newPosition.Y, _dragBoundary.Top, _dragBoundary.Bottom);

		changePosition(newPosition);
	}

	private void changePosition(Vector2 newPosition)
	{
		Position = newPosition;
		PositionChanged?.Invoke();
	}

	private void onDragStarted()
	{
		_isDragging = true;
		_lastPosition = Input.MousePosition;
	}

	private class DragArea : GameObject
	{
		public Action? DragStarted;

		protected override bool OnMouseDown(MouseDownEvent e)
		{
			DragStarted?.Invoke();
			return true;
		}

		protected override bool OnHover(HoverEvent e)
		{
			return true;
		}
	}
}
