using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Layout;
using System.Collections.Generic;

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

	public CompositeGameObject InternalComposition;

	private LayoutValue _sizeLayout = new(Invalidation.DrawSize);

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (_sizeLayout.IsValid == false)
		{
			if (BorderObject is not null && AutoSizeAxes == Axes.None)
			{
				var thickness = BorderObject.Thickness;
				InternalComposition.Position = new(thickness.Left, thickness.Top);
				InternalComposition.RelativeSizeAxes = Axes.None;
				InternalComposition.Size = DrawSize - thickness.Total;
			}
			_sizeLayout.Validate();
		}
	}

	public override Axes AutoSizeAxes
	{
		get => base.AutoSizeAxes;
		set
		{
			InternalComposition.RelativeSizeAxes = Axes.None;
			InternalComposition.AutoSizeAxes = value;
			base.AutoSizeAxes = value;
		}
	}

	public Axes InternalRelativeSizeAxes
	{
		get => InternalComposition.RelativeSizeAxes;
		set
		{
			InternalComposition.RelativeSizeAxes = value;
		}
	}

	protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
	{
		InternalComposition.Invalidate(invalidation, source);
		return base.OnInvalidate(invalidation, source);
	}

	#region Background

	public GameObject? BackgroundObject { get; set; }

	public ColorQuad BackgroundColor
	{
		get
		{
			if (BackgroundObject == null)
				return new Color();

			return BackgroundObject.Color;
		}
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
		IgnoredForAutoSizeAxes = Axes.Both,
		Depth = 1000
	};

	#endregion

	#region Border
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

	public Boundary BorderThickness
	{
		get
		{
			if (BorderObject is null) return Boundary.Zero;

			return BorderObject.Thickness;
		}
		set
		{
			if (BorderObject is null)
				AddInternal(BorderObject = createBorder());

			BorderObject.Thickness = value;
			_sizeLayout.Invalidate();
		}
	}

	private HollowBox createBorder() => new()
	{
		RelativeSizeAxes = Axes.Both,
		IgnoredForAutoSizeAxes = Axes.Both,
		Depth = -1000,
		Color = ColorQuad.SolidColor(Palette.Black)
	};
	#endregion

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

