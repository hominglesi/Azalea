using System.Numerics;

namespace Azalea.Scenes.Physics;

public readonly struct Manifold2D(Vector2 normal, float depth)
{
	public readonly Vector2 Normal = normal;
	public readonly float Depth = depth;

	public Manifold2D() : this(Vector2.Zero, 0) { }
}
