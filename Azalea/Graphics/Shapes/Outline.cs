using Azalea.Graphics.Colors;
using Azalea.Graphics.Containers;
using Azalea.Layout;
using System;
using System.Numerics;

namespace Azalea.Graphics.Shapes;

public partial class Outline : CompositeGameObject
{
	private float _thickness = 3;
	public float Thickness
	{
		get => _thickness;
		set
		{
			if (_thickness == value) return;

			_thickness = value;
			_sizeValue.Invalidate();
		}
	}

	private readonly GameObject _child;
	private readonly OutlineWrapper _wrapper;

	public Outline(GameObject child)
	{
		AddLayout(_sizeValue);

		Color = Palette.Black;

		AddInternal(_child = child);
		AddInternal(_wrapper = new OutlineWrapper(_child));
	}

	private LayoutValue _sizeValue = new(Invalidation.DrawSize, InvalidationSource.Child);

	protected override void Update()
	{
		base.Update();

		if (_sizeValue.IsValid) return;

		base.Size = _child.Size + new Vector2(Thickness * 2, Thickness * 2);
		_child.Position = new Vector2(Thickness, Thickness);
		_wrapper.Thickness = Thickness;
	}

	public override Vector2 Size
	{
		get => base.Size;
		set => throw new InvalidOperationException($"The size of {nameof(Outline)} cannot be directly edited," +
			$"use thickness insead or directly change the size of the wrapped child.");
	}

	internal class OutlineWrapper : GameObject
	{
		public float Thickness { get; set; }
		public GameObject WrappedChild { get; set; }

		public OutlineWrapper(GameObject child)
		{
			WrappedChild = child;
		}

		protected override DrawNode CreateDrawNode() => new OutlineWrapperDrawNode(this);
	}
}
