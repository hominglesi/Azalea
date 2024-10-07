using Azalea.Simulations.Colliders;
using System.Numerics;

namespace Azalea.Simulations;
public class Ray
{
	public Ray(Vector2 startPosition, float angle, int range)
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
	public Collider HitCollider { get; set; }
	public PhysicsGenerator PGen { get; }


}
