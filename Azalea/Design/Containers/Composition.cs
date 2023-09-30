using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Layout;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.Containers;

public class Composition : CompositeGameObject
{
	public Composition()
	{
		AddLayout(_sizeLayout);
		AddInternal(InternalComposition = new CompositeGameObject()
		{
			RelativeSizeAxes = Axes.Both
		});
	}

	protected CompositeGameObject InternalComposition;

	private LayoutValue _sizeLayout = new(Invalidation.DrawSize);

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (_sizeLayout.IsValid == false)
		{
			if (BorderObject is not null)
			{
				var thickness = BorderObject.Thickness;
				InternalComposition.Position = new(thickness, thickness);
				InternalComposition.RelativeSizeAxes = Axes.None;
				InternalComposition.Size = DrawSize - new Vector2(thickness * 2, thickness * 2);
			}
			_sizeLayout.Validate();
		}
	}

	#region Background

	public GameObject? BackgroundObject { get; set; }

	public ColorQuad BackgroundColor
	{
		set
		{
			if (BackgroundObject == null)
				AddInternal(BackgroundObject = createSolidBackground());

			BackgroundObject.Color = value;
		}
	}

	private Box createSolidBackground() => new()
	{
		RelativeSizeAxes = Axes.Both,
		Depth = 1000
	};

	#endregion

	public HollowBox? BorderObject { get; set; }
	public ColorQuad BorderColor
	{
		set
		{
			if (BorderObject is null)
				AddInternal(BorderObject = createBorder());

			BorderObject.Color = value;
		}
	}

	public int BorderThickness
	{
		set
		{
			if (BorderObject is null)
				AddInternal(BorderObject = createBorder());

			BorderObject.Thickness = value;
		}
	}

	private HollowBox createBorder() => new()
	{
		RelativeSizeAxes = Axes.Both,
		Depth = -1000,
		Color = ColorQuad.SolidColor(Palette.Black)
	};

	#region Children

	public IReadOnlyList<GameObject> Children
	{
		get
		{
			return InternalComposition.InternalChildren;
		}
		set
		{
			Clear();
			AddRange(value);
		}
	}

	public GameObject Child
	{
		set
		{
			Clear();
			Add(value);
		}
	}

	/// <summary>
	/// Accesses the <paramref name="index"/>-th child.
	/// </summary>
	/// <param name="index">The index of the child to access.</param>
	/// <returns>The <paramref name="index"/>-th child.</returns>
	public GameObject this[int index] => Children[index];

	/// <summary>
	/// The amount of elements in <see cref="Children"/>.
	/// </summary>
	public int Count => Children.Count;

	public virtual void Add(GameObject gameObject)
	{
		InternalComposition.AddInternal(gameObject);
	}

	public virtual void AddRange(IEnumerable<GameObject> range)
	{
		foreach (GameObject gameObject in range)
			Add(gameObject);
	}

	public virtual bool Remove(GameObject gameObject)
	{
		return InternalComposition.RemoveInternal(gameObject);
	}

	public void RemoveRange(IEnumerable<GameObject> range)
	{
		if (range == null)
			return;

		foreach (GameObject obj in range)
			Remove(obj);
	}

	public virtual void Clear()
	{
		InternalComposition.ClearInternal();
	}

	public void ChangeChildDepth(GameObject child, float newDepth)
	{
		InternalComposition.ChangeInternalChildDepth(child, newDepth);
	}

	#endregion
}

