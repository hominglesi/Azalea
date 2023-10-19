using Azalea.Debugging;
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
}
