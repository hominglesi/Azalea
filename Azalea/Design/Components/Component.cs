using Azalea.Graphics;
using System;

namespace Azalea.Design.Components;
public abstract class Component
{
	private GameObject? _parent;
	public GameObject Parent => _parent ?? throw new Exception("This components Parent has not been set");
	protected virtual void OnAttached() { }
	protected virtual void OnDetached() { }
	internal void AttachParent(GameObject parent)
	{
		_parent = parent;
		OnAttached();

		ComponentStorage.Add(this);
	}

	internal void DetachParent()
	{
		_parent = null;
		OnDetached();

		ComponentStorage.Remove(this);
	}

	public virtual void Update() { }
}
