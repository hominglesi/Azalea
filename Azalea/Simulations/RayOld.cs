using Azalea.Simulations.Colliders;
using System.Numerics;

namespace Azalea.Simulations;
public class RayOld
{
	public RayOld(Vector2 startPosition, float angle, int range)
	{
		Angle = angle;
		Range = range;
		StartPosition = startPosition;
	}
	public bool Hit { get; set; }
	public int Range { get; set; }
	public int MinimumRange { get; set; }
	public float Distance { get; set; }
	public float Angle { get; set; }
	public Vector2 StartPosition { get; set; }
	public ColliderOld HitCollider { get; set; }
	public PhysicsGeneratorOld PGen { get; }


}
