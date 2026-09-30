using Azalea.Extentions.MatrixExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Loading.OpenGL;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.Design.Shapes;
public class Line : GameObject
{
	public Vector2 StartPoint { get; set; }

	public Vector2 EndPoint { get; set; }

	public float Thickness { get; set; } = 3;

	public override void Draw(RenderCoordinator coordinator)
	{
		var distance = MathUtils.DistanceBetween(StartPoint, EndPoint);
		var rectangle = new Rectangle(Vector2.Zero, new(distance, Thickness));
		rectangle.Y -= Thickness / 2;
		var rotation = MathUtils.GetAngleTowards(StartPoint, EndPoint);

		var matrix = Matrix3.Identity;//Info.Matrix;
		MatrixExtentions.TranslateFromLeft(ref matrix, StartPoint);
		MatrixExtentions.RotateFromLeft(ref matrix, rotation);

		coordinator.BindTexture(GameHost.Instance.Loader.WhitePixel);
		coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(rectangle) * matrix, DrawColorInfo.Color, Rectangle.One);
	}
}
