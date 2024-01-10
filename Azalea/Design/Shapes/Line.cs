using Azalea.Graphics;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
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

			var halfLine = new Vector2(0, Thickness / 2);

			var quad = new Quad(
				StartPoint + halfLine,
				StartPoint - halfLine,
				EndPoint - halfLine,
				EndPoint + halfLine);

			renderer.DrawQuad(
			AzaleaGame.Main.Host.Renderer.WhitePixel,
			quad * Info.Matrix,
			DrawColorInfo);
		}
	}
}
