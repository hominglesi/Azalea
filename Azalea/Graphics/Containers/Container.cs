using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Containers;

public class Container : Container<GameObject>
{

}

public class Container<T> : CompositeGameObject
    where T : GameObject
{
    private int enumeratorVersion;

    protected virtual Container<T> Content => this;

    public virtual void Add(T gameObject)
    {
        if (gameObject == Content)
            throw new InvalidOperationException("Content may not be added to itself.");

        ArgumentNullException.ThrowIfNull(gameObject);

        if (Content == this)
            AddInternal(gameObject);
        else
            Content.Add(gameObject);
    }

    public virtual void AddRange(IEnumerable<T> range)
    {
        foreach (T gameObject in range)
            Add(gameObject);
    }

    public virtual bool Remove(T gameObject)
    {
        if (Content != this)
            Content.Remove(gameObject);

        return RemoveInternal(gameObject);
    }

    public void RemoveRange(IEnumerable<T> range)
    {
        if (range == null)
            return;

        foreach (T obj in range)
            Remove(obj);
    }

    protected override void AddInternal(GameObject gameObject)
    {
        if (Content == this && gameObject != null && (gameObject is T) == false)
        {
            throw new Exception("Cannot add Game Object");
        }

        enumeratorVersion++;

        base.AddInternal(gameObject);
    }
}

