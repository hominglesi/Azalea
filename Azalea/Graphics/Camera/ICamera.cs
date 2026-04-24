using Azalea.Numerics;
using System.Numerics;

namespace Azalea.Graphics.Camera;
public interface ICamera
{
	public Vector2 Position { get; set; }
	public float Zoom { get; set; }

	public Matrix4x4 CreateProjectionMatrix(Vector2 screenSize);
	public Vector2 ToWorldSpace(Vector2 screenPoint);
	public RectangleInt ToWorldSpace(RectangleInt screenRectangle);
}
