using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.Containers;
public abstract class WindowContainer : Composition
{
	#region Dragging

	private Vector2 _previousDragPosition;
	private bool _isBeingDragged;

	private readonly List<GameObject> _draggableSurfaces = new();

	protected void AddDragableSurface(GameObject surface)
	{
		if (_draggableSurfaces.Contains(surface))
		{
			// LOGGER-WARNING: this window already consideres this object a draggable surface 
			return;
		}

		surface.MouseDown += _ => startDragging();
		_draggableSurfaces.Add(surface);
	}

	private void startDragging()
	{
		_previousDragPosition = Input.MousePosition;
		_isBeingDragged = true;
	}

	private void endDragging()
	{
		_isBeingDragged = false;
	}

	#endregion

	protected override void Update()
	{
		if (_isBeingDragged)
		{
			Position += Input.MousePosition - _previousDragPosition;
			_previousDragPosition = Input.MousePosition;
		}
	}

	protected override void OnMouseUp(MouseUpEvent e)
	{
		endDragging();
		base.OnMouseUp(e);
	}
}
