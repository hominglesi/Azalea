using Azalea.Graphics;

namespace Azalea.Design.Shapes;
public partial class HollowBox : GameObject
{
	private float _thickness = 3;
	public float Thickness
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
