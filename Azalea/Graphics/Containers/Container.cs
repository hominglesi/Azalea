using System;

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

    protected override void AddInternal(GameObject gameObject)
    {
        if (Content == this && gameObject != null && (gameObject is T) == false)
        {

        }

        enumeratorVersion++;

        base.AddInternal(gameObject);
    }
}

