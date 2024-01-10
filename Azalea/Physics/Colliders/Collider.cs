using Azalea.Design.Components;
using System.Numerics;

namespace Azalea.Physics.Colliders;
public abstract class Collider : Component
{
	public Vector2 Position { get => Parent.Position; set => Parent.Position = value; }
	public Vector2 Scale { get => Parent.Scale; set => Parent.Scale = value; }
	public float Rotation { get => Parent.Rotation; set => Parent.Rotation = value; }

	public abstract float ShortestDistance { get; }

	public abstract Vector2[] GetVertices();
}
