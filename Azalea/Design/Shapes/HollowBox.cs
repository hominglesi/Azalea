using Azalea.Graphics;

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

	protected override DrawNode CreateDrawNode() => new HollowBoxDrawNode(this);
}
