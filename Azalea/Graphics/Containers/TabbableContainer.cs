using System.Collections.Generic;
using System.Linq;

namespace Azalea.Graphics.Containers;

public class TabbableContainer : TabbableContainer<GameObject>
{ }


/// <summary>
/// This interface is used for recognizing <see cref="TabbableContainer{T}"/> of any type without reflection.
/// </summary>
public interface ITabbableContainer
{
    /// <summary>
    /// Whether this <see cref="ITabbableContainer"/> can be tabbed to.
    /// </summary>
    public bool CanBeTabbedTo { get; }
}

public class TabbableContainer<T> : Container<T>, ITabbableContainer
    where T : GameObject
{
    public virtual bool CanBeTabbedTo => true;

    /*
    //TODO: Enable tabbing
    public CompositeGameObject TabbableContentContainer { private get; set; }

    public override bool OnKeyDown(KeyDownEvent e)
    {
        if (TabbableContentContainer == null || e.Key != Keys.Tab)
            return false;

        var nextTab = nextTabStop(TabbableContentContainer, e.ShiftPressed);
        if (nextTab != null) GetContainingInputManager().ChangeFocus(nextTab);
        return true;
    }*/

    private GameObject? nextTabStop(CompositeGameObject target, bool reverse)
    {
        Stack<GameObject> stack = new();
        stack.Push(target); //Extra push for circular tabbing
        stack.Push(target);

        bool started = false;

        while (stack.Count > 0)
        {
            var drawable = stack.Pop();

            if (started == false)
                started = ReferenceEquals(drawable, this);
            else if (drawable is ITabbableContainer tabbable && tabbable.CanBeTabbedTo)
                return drawable;

            if (drawable is CompositeGameObject composite)
            {
                var newChildren = composite.InternalChildren.ToList();
                int bound = reverse ? newChildren.Count : 0;

                if (!started)
                {
                    int index = newChildren.IndexOf(this);
                    if (index != -1)
                        bound = reverse ? index + 1 : index;
                }

                if (reverse)
                {
                    for (int i = 0; i < bound; i++)
                        stack.Push(newChildren[i]);
                }
                else
                {
                    for (int i = newChildren.Count - 1; i >= bound; i--)
                        stack.Push(newChildren[i]);
                }
            }
        }

        return null;
    }
}


