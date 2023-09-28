This static class is a simple way to access some useful inputs.

`MousePosition` is the current pixel position within a [[Window]].
`GetKey()` returns the  [[ButtonState]] of the [[Key]] passed to it.
`GetMouseButton()` returns the [[ButtonState]] of the [[MouseButton]] passed to it.
`OnTextInput` is called when ever a char is pressed on the keyboard. It respects language specific keys like ö and ć.
`GetHoveredObjects()` returns a list of all hovered objects sorted front to back.