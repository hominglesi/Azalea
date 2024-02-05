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

		AddToScene();
	}

	internal void DetachParent()
	{
		_parent = null;
		OnDetached();

		RemoveFromScene();
	}

	private bool _inScene;

	internal void AddToScene()
	{
		if (_inScene) return;

		ComponentStorage.Add(this);
		_inScene = true;
	}

	internal void RemoveFromScene()
	{
		if (_inScene == false) return;

		ComponentStorage.Remove(this);
		_inScene = false;
	}

	public virtual void Update() { }
}
