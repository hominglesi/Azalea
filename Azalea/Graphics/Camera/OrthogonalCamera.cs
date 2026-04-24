using Azalea.Numerics;
using System.Numerics;

namespace Azalea.Graphics.Camera;
internal class OrthogonalCamera : ICamera
{
	public Vector2 Position { get; set; } = Vector2.Zero;
	public float Zoom { get; set; } = 1f;

	public Matrix4x4 CreateProjectionMatrix(Vector2 screenSize)
	{
		var left = Position.X;
		var right = left + screenSize.X / Zoom;
		var top = Position.Y;
		var bottom = top + screenSize.Y / Zoom;

		return Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, 0.1f, 100);
	}

	public Vector2 ToWorldSpace(Vector2 screenPoint) => (screenPoint / Zoom) + Position;

	public RectangleInt ToWorldSpace(RectangleInt screenRectangle)
	{
		return new RectangleInt(
			(int)((screenRectangle.Left - Position.X) * Zoom),
			(int)((screenRectangle.Top - Position.Y) * Zoom),
			(int)(screenRectangle.Width * Zoom),
			(int)(screenRectangle.Height * Zoom));
	}
}
