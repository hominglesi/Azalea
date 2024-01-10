using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.Design.Containers;

public class Composition : CompositeGameObject
{
	private List<GameObject> _publicChildren = new();

	protected virtual IReadOnlyList<GameObject> PublicChildren => _publicChildren;

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
				AddInternal(BorderObject = CreateBorderObject());

			BorderObject.Color = value;
			MaskingPadding = BorderObject.Thickness;
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
				AddInternal(BorderObject = CreateBorderObject());

			BorderObject.Thickness = value;
			MaskingPadding = value;
		}
	}

	protected virtual HollowBox CreateBorderObject() => new()
	{
		RelativeSizeAxes = Axes.Both,
		IgnoredForAutoSizeAxes = Axes.Both,
		Depth = -1000,
		Color = ColorQuad.SolidColor(Palette.Black),
		OutsideContent = true
	};
	#endregion

	#region Children

	public IReadOnlyList<GameObject> Children
	{
		get => PublicChildren;
		set
		{
			Clear();
			AddRange(value);
		}
	}

	public GameObject Child
	{
		get
		{
			if (PublicChildren.Count != 1)
				throw new Exception("The 'Child' property can only be used when this Composition has exactly one child");

			return PublicChildren[0];
		}
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
		_publicChildren.Add(gameObject);

		AddInternal(gameObject);
	}

	public virtual void AddRange(IEnumerable<GameObject> range)
	{
		foreach (GameObject gameObject in range)
			Add(gameObject);
	}

	public virtual bool Remove(GameObject gameObject)
	{
		_publicChildren.Remove(gameObject);

		return RemoveInternal(gameObject);
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
		var children = PublicChildren.ToArray();

		foreach (var child in children)
		{
			Remove(child);
		}
	}

	public void ChangeChildDepth(GameObject child, float newDepth)
	{
		ChangeInternalChildDepth(child, newDepth);
	}

	#endregion
}

