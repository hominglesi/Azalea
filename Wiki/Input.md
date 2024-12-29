Every kind of user input goes through the Input class. We can use it to get the current state of all kinds of inputs such as mouse position, key state or scroll delta. It is platform independent because every platform needs to report user input to it directly and we can use this to execute fake inputs if needed.

# Getting Input

## MousePosition
Current mouse coordinates in screen space, meaning that (0, 0) is the top-left of the window client area.

`public static Vector2 MousePosition { get; }`

## MouseWheelDelta
Scroll value change since last frame.

`public static float MouseWheelDelta { get; }`

## GetMouseButton
Returns the state of the specified mouse button.

`public static ButtonState GetMouseButton(MouseButton button)`

## GetHoveredObjects
Returns a read-only list of all the currently hovered objects. `recalculate` specifies if the scene graph needs to be reevaluated for moved objects.

`public static IReadOnlyList<GameObject> GetHoveredObjects(bool recalculate)`

## GetKey
Returns the state of the specified key.

`public static ButtonState GetKey(Keys key)`

## OnTextInput
Event that get raised whenever a character is inputted on the keyboard.

`public static event Action<char>? OnTextInput`

## GetDirectionalMovement
Returns the movement vector by getting the state of the WASD and Arrow keys.

`public static Vector2 GetDirectionalMovement()`

# Other

## ChangeFocus
Changes the focused object. Can be set to null to remove current focus.

`public static bool ChangeFocus(GameObject? newFocus)`

## GetPositionalInputQueue
Gets a list of all the objects in the scene graph that contain the specified position starting with the deepest ones and going out.

`public static IReadOnlyList<GameObject> GetPositionalInputQueue(Vector2 position)`

## GetNonPositionalInputQueue
Gets a list of all the objects in the scene graph starting with the deepest ones and going out.

`public static IReadOnlyList<GameObject> GetNonPositionalInputQueue()`

# Executing Actions

Using the `Input` class we can also simulate actions as if the user has performed them. This is not advised as the action is executed out of normal update loop order and can cause unexpected behavior, but it is useful for testing purposes.

## ExecuteMousePositionChange
Executes a mouse move action.

`public static void ExecuteMousePositionChange(Vector2 newPosition)`

## ExecuteScroll
Executes a scroll action.

`public static void ExecuteScroll(float delta)`

## ExecuteMouseButtonStateChange
Executes a pressed state change action on the specified button.

`public static void ExecuteMouseButtonStateChange(MouseButton button, bool pressed)`

## ExecuteKeyboardKeyStateChange
Executes a pressed state change action on the specified key.

`public static void ExecuteKeyboardKeyStateChange(Keys key, bool pressed)`

## ExecuteKeyboardKeyRepeat
Sets the repeat state of the specified key to true.

`public static void ExecuteKeyboardKeyRepeat(Keys key)`

## ExecuteTextInput
Executes a text input action with the specified character.

`public static void ExecuteTextInput(char input)`
