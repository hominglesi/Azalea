using Azalea.Debugging;
using Azalea.Graphics;
using System;
using System.Numerics;

namespace Azalea.Design.Containers;
public class PannableContainer : Composition
{
	public Vector2 CameraPosition
	{
		get => base.Position;
		set
		{
			base.Position = value;
		}
	}

	public void MoveCameraBy(Vector2 change) { CameraPosition += change; }

	[HideInInspector]
	public new Vector2 Position
	{
		get => base.Position;
		set => throw new InvalidOperationException($"Do not change {nameof(Position)} directly, insted use {nameof(CameraPosition)}");
	}

	public Vector2 CameraZoom
	{
		get => Scale;
		set
		{
			base.Scale = value;
		}
	}

	public void ZoomCameraBy(Vector2 zoom) { CameraZoom += zoom; }

	[HideInInspector]
	public new Vector2 Scale
	{
		get => base.Scale;
		set => throw new InvalidOperationException($"Do not change {nameof(Scale)} directly, insted use {nameof(CameraZoom)}");
	}

	private Vector2 _windowSize => AzaleaGame.Main.Host.Window.ClientSize;

	public void CenterOnObject(GameObject obj)
	{
		CameraPosition = -obj.Position * CameraZoom;
		CameraPosition += _windowSize / 2;
		CameraPosition -= obj.Size / 2 * CameraZoom;
	}

	private GameObject? _followedObject;
	private Vector2 _lastFollowedObjectPosition;
	private Vector2 _lastWindowSize;
	public void SetFollowedObject(GameObject obj)
	{
		_followedObject = obj;
		_lastFollowedObjectPosition = obj.Position;
	}

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (_lastWindowSize == Vector2.Zero)
			_lastWindowSize = _windowSize;

		if (_lastWindowSize != _windowSize)
		{
			var movement = (_windowSize - _lastWindowSize) / 2;
			CameraPosition += movement;
			_lastWindowSize = _windowSize;
		}

		if (_followedObject is not null)
		{
			var cameraMovement = _lastFollowedObjectPosition - _followedObject.Position;

			CameraPosition += cameraMovement * CameraZoom;
			_lastFollowedObjectPosition = _followedObject.Position;
		}
	}
}
