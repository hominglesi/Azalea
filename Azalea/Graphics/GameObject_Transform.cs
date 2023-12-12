using Azalea.Debugging;
using Azalea.Design.Components;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Azalea.Graphics;
public abstract partial class GameObject
{
	private Transform _transform;
	public Transform Transform => _transform;

	[MemberNotNull(nameof(_transform))]
	private void addTransform()
	{
		_transform = new Transform();
		_transform.OnGeomentryChanged += () => Invalidate(Invalidation.MiscGeometry);
		_transform.OnSizeChanged += (axes) => invalidateParentSizeDependencies(Invalidation.DrawSize, axes);
		AddComponent(_transform);
	}

	public Vector2 Position
	{
		get => Transform.Position;
		set => Transform.Position = value;
	}

	[HideInInspector]
	public float X
	{
		get => Transform.X;
		set => Transform.X = value;
	}

	[HideInInspector]
	public float Y
	{
		get => Transform.Y;
		set => Transform.Y = value;
	}

	public virtual Vector2 Size
	{
		get => Transform.Size;
		set => Transform.Size = value;
	}

	[HideInInspector]
	public virtual float Width
	{
		get => Transform.Width;
		set => Transform.Width = value;
	}

	[HideInInspector]
	public virtual float Height
	{
		get => Transform.Height;
		set => Transform.Height = value;
	}

	public float Rotation
	{
		get => Transform.Rotation;
		set => Transform.Rotation = value;
	}
}
