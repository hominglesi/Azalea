using Azalea.Graphics;
using Azalea.Graphics.Rendering;

namespace Azalea.Design.Shapes;
public partial class HollowBox : GameObject
{
	public Boundary Thickness { get; set; } = new(3);
	public BorderAlignment Alignment { get; set; }

	public override void Draw(IRenderer renderer)
	{
		renderer.DrawRectangle(DrawRectangle, DrawInfo.Matrix, Thickness, DrawColorInfo, Alignment);
	}
}
