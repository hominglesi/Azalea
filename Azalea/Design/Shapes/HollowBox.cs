using Azalea.Graphics;
using Azalea.Graphics.Rendering;

namespace Azalea.Design.Shapes;
public partial class HollowBox : GameObject
{
	private Boundary _thickness = new(3);
	public Boundary Thickness
	{
		get => _thickness;
		set => _thickness = value;
	}

	private bool _outsideContent = false;
	public bool OutsideContent
	{
		get => _outsideContent;
		set => _outsideContent = value;
	}

	public override void Draw(IRenderer renderer)
	{
		renderer.BindShader(renderer.QuadShader);

		renderer.DrawRectangle(DrawRectangle, DrawInfo.Matrix, Thickness, DrawColorInfo, OutsideContent);
	}
}
