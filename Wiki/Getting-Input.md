Every window input is handled through the static [Input](Input) class. This ensures that getting user input is simple from anywhere in the code. Additionally, every `GameObject` has their own virtual functions for responding to certain inputs, such as an `OnClick` function that is called only when the `MouseDownEvent` and `MouseUpEvent` happen while hovering that object. It is recommended to use the latter option as it supports more features such as "consuming" input and won't break if we ever introduce support for multiple windows.

# Example

### Input class examples

```
public override void Update()
{
	Cursor.Position = Input.MousePosition;
	
	Player.Position += Input.GetDirectionalMovement();
	if (Input.GetKey(Keys.Shift).Down)
		Player.Dash();
	
	Input.OnCharInput += chr => Text += chr;
}
```