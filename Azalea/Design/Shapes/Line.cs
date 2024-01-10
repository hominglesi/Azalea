using Azalea.Extentions.MatrixExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Numerics;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.Design.Shapes;
public class Line : GameObject
{
	public Vector2 StartPoint { get; set; }

	public Vector2 EndPoint { get; set; }

	public float Thickness { get; set; } = 3;

	protected override DrawNode CreateDrawNode() => new LineDrawNode(this);

	private class LineDrawNode : DrawNode
	{
		protected new Line Source => (Line)base.Source;

		private Vector2 StartPoint { get; set; }
		private Vector2 EndPoint { get; set; }
		private DrawInfo Info { get; set; }
		private float Thickness { get; set; }

		public override void ApplyState()
		{
			base.ApplyState();

			StartPoint = Source.StartPoint;
			EndPoint = Source.EndPoint;
			Thickness = Source.Thickness;
			Info = Source.DrawInfo;
		}

		public LineDrawNode(Line source)
			: base(source) { }

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			var distance = MathUtils.DistanceBetween(StartPoint, EndPoint);
			var rectangle = new Rectangle(Vector2.Zero, new(distance, Thickness));
			rectangle.Y -= Thickness / 2;
			var rotation = MathUtils.GetAngleTowards(StartPoint, EndPoint);

			var matrix = Matrix3.Identity;//Info.Matrix;
			MatrixExtentions.TranslateFromLeft(ref matrix, StartPoint);
			MatrixExtentions.RotateFromLeft(ref matrix, rotation);

			renderer.DrawQuad(
			AzaleaGame.Main.Host.Renderer.WhitePixel,
			Quad.FromRectangle(rectangle) * matrix, //  quad * Info.Matrix,
			DrawColorInfo);
		}
	}
}
