using Azalea.Graphics.Primitives;
using System;
using System.Numerics;

namespace Azalea.Simulations.Colliders;
public class RectCollider : Collider
{
	private Quad _quad;
	private Vector2 _size;
	private Vector2 _halfSize;
	private float _shortestDistance;
	private Vector2 _position;

	protected override void OnAttached()
	{
		Parent.OnDrawInfoInvalidated += recalculateCollider;
		recalculateCollider();
	}

	private void recalculateCollider()
	{
		_quad = Parent.ScreenSpaceDrawQuad;

		_size = _quad.Size;
		_halfSize = _quad.Size / 2;
		_shortestDistance = Math.Min(_size.X, _size.Y);

		_position = _quad.TopLeft + ((_quad.BottomRight - _quad.TopLeft) / 2);
	}

	internal Quad Quad => _quad;

	public Vector2 Size => _size;
	public Vector2 HalfSize => _halfSize;

	public float HalfA => _halfSize.X;
	public float HalfB => _halfSize.Y;
	public float SideA => _size.X;
	public float SideB => _size.Y;

	public override float ShortestDistance => _shortestDistance;

	internal override Vector2 GetPosition()
	{
		var x = Parent.DrawInfo;
		return _position;
	}

	internal override void SetPosition(Vector2 pos)
	{
		var newPos = pos - ((_quad.BottomRight - _quad.TopLeft) / 2);
		newPos += Parent.OriginPosition;

		var parent = Parent.Parent;
		while (parent != null)
		{
			newPos -= parent.Position;
			parent = parent.Parent;
		}

		base.SetPosition(newPos);
	}

	public override Vector2[] GetVertices()
	{
		return new Vector2[]
		{
			_quad.TopLeft,
			_quad.TopRight,
			_quad.BottomLeft,
			_quad.BottomRight
		};
	}

	public override bool ProcessCollision(Collider other, bool resolveCollision)
		=> other.ProcessCollision(this, resolveCollision);

	public override bool ProcessCollision(CircleCollider other, bool resolveCollision)
		=> CollisionLogic.CircleRectCollision(other, this, resolveCollision);

	public override bool ProcessCollision(RectCollider other, bool resolveCollision)
		=> CollisionLogic.RectRectCollision(this, other, resolveCollision);
}
