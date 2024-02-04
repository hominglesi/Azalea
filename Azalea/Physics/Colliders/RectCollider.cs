using System.Numerics;

namespace Azalea.Physics.Colliders;
public class RectCollider : Collider
{
	//TODO: Rework with Quad

	public float HalfA { get; private set; }
	public float HalfB { get; private set; }

	//Upper and Bottom side
	private float _sideA;
	public float SideA
	{
		get => _sideA;
		set
		{
			_sideA = value;
			HalfA = _sideA / 2;
		}
	}

	//Left And Right side
	private float _sideB;
	public float SideB
	{
		get => _sideB;
		set
		{
			_sideB = value;
			HalfB = _sideB / 2;
		}
	}

	public override float ShortestDistance => SideA < SideB ? SideA : SideB;
	public override Vector2[] GetVertices()
	{
		// Vertices in local space
		float halfWidth = SideA / 2;
		float halfHeight = SideB / 2;

		return new Vector2[]
		{
			new Vector2(-halfWidth, -halfHeight),
			new Vector2(halfWidth, -halfHeight),
			new Vector2(halfWidth, halfHeight),
			new Vector2(-halfWidth, halfHeight)
		};
	}

	public override bool ProcessCollision(Collider other, bool resolveCollision)
		=> other.ProcessCollision(this, resolveCollision);

	public override bool ProcessCollision(CircleCollider other, bool resolveCollision)
		=> CollisionLogic.CircleRectCollision(other, this, resolveCollision);

	public override bool ProcessCollision(RectCollider other, bool resolveCollision)
		=> CollisionLogic.RectRectCollision(this, other, resolveCollision);
}
