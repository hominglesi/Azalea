using System.Collections;
using System.Collections.Generic;

namespace Azalea.Inputs.States;

public class ButtonStates<TButton> : IEnumerable<TButton>
    where TButton : struct
{
    private HashSet<TButton> pressedButtons = new();

    public bool IsPressed(TButton button) => pressedButtons.Contains(button);

    public IEnumerator<TButton> GetEnumerator() => ((IEnumerable<TButton>)pressedButtons).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
