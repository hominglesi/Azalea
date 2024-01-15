using Azalea.Physics.Colliders;
using System.Numerics;

namespace Azalea.Physics;
public class Ray
{
	public Ray(PhysicsGenerator pgen, Vector2 startPosition, float angle, int range)
	{
		PGen = pgen;
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
	public Collider HitCollider { get; set; }
	public PhysicsGenerator PGen { get; }


}
