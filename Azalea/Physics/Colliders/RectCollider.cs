using System.Numerics;

namespace Azalea.Physics.Colliders;
public class RectCollider : Collider
{
	//TODO: Rework with Quad

	//Upper and Bottom side
	public float SideA { get; set; }
	//Left And Right side
	public float SideB { get; set; }

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

}
