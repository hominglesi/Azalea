Every kind of user input goes through the Input class. We can use it to get the current state of all kinds of inputs such as mouse position, key state or scroll delta. It is platform independent because every platform needs to report user input to it directly and we can use this to execute fake inputs if needed.

## Getting Input

#### `MousePosition`

Current mouse coordinates in screen space, meaning that (0, 0) is the top-left of the window client area.

#### `MouseWheelDelta`

Scroll value change since last frame.

#### `GetMouseButton(MouseButton button)`

Returns the state of the specified mouse button.

#### `GetHoveredObjects()`

Returns a read-only list of all the currently hovered objects.

#### `GetKey(Keys key)`

Returns the state of the specified key.

#### `OnTextInput`

Event that get raised whenever a character is inputted on the keyboard.

#### `GetDirectionalMovement()`

Returns the movement vector by getting the state of the WASD and Arrow keys.

## Other

#### `ChangeFocus(GameObject? newFocus)`

Changes the focused object. Can be set to null to remove current focus.

#### `GetPositionalInputQueue(Vector2 position)`

Gets a list of all the objects in the scene graph that contain the specified position starting with the deepest ones and going out.

#### `GetNonPositionalInputQueue()`

Gets a list of all the objects in the scene graph starting with the deepest ones and going out.

## Executing Actions

Using the `Input` class we can also simulate actions as if the user has performed them. This is not advised as the action is executed out of normal update loop order and can cause unexpected behavior, but it is useful for testing purposes.

#### `ExecuteMousePositionChange(Vector2 newPosition)`

Executes a mouse move action.

#### `ExecuteScroll(float delta)`

Executes a scroll action.

#### `ExecuteMouseButtonStateChange(MouseButton button, bool pressed)`

Executes a pressed state change action on the specified button.

#### `ExecuteKeyboardKeyStateChange(Keys key, bool pressed)`

Executes a pressed state change action on the specified key.

#### `ExecuteKeyboardKeyRepeat(Keys key)`

Sets the repeat state of the specified key to true.

#### `ExecuteTextInput(char input)`

Executes a text input action with the specified character.