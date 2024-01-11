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

	public override void Draw(IRenderer renderer)
	{
		var halfLine = new Vector2(0, Thickness / 2);

		var quad = new Quad(
			StartPoint + halfLine,
			StartPoint - halfLine,
			EndPoint - halfLine,
			EndPoint + halfLine);

		renderer.DrawQuad(
		AzaleaGame.Main.Host.Renderer.WhitePixel,
		quad * DrawInfo.Matrix,
		DrawColorInfo);
	}
}
