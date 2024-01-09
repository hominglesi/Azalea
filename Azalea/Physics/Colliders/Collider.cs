using Azalea.Design.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics.Colliders;
public abstract class Collider : Component
{
	public Vector2 Position { get { return Parent.Position; } set { Parent.Position = value; } }
	public Vector2 Scale { get { return Parent.Scale; } set { Parent.Scale = value; } }
	public float Rotation { get { return Parent.Rotation; } set { Parent.Rotation = value; } }

	public abstract float ShortestDistance { get; }

	public abstract Vector2[] GetVertices();
}
