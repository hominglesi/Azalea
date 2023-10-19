using Azalea.Graphics;

namespace Azalea.Design.Shapes;
public partial class HollowBox : GameObject
{
	private Boundary _thickness = new(3);
	public Boundary Thickness
	{
		get => _thickness;
		set
		{
			if (_thickness == value) return;

			_thickness = value;
		}
	}

	protected override DrawNode CreateDrawNode() => new HollowBoxDrawNode(this);
}
