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

    public virtual void Add(T drawable)
    {
        if (drawable == Content)
            throw new InvalidOperationException("Content may not be added to itself.");

        ArgumentNullException.ThrowIfNull(drawable);

        if (Content == this)
            AddInternal(drawable);
        else
            Content.Add(drawable);
    }

    public virtual void AddRange(IEnumerable<T> range)
    {
        foreach (T drawable in range)
            Add(drawable);
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

