Why is the `Color` property of an object a `ColorQuad` and not simply `Color`?

Most of the time we want to give an object a color that means that the whole object should be that single color. But what if we want to apply a gradient to the object? By using a `ColorQuad` we can specify a different color for each corner of an object. OpenGL interprets this by linearly transitioning between them. 

By setting every objects `Color` property to be a `ColorInfo` we do make it a bit more complex to work with simple solid colors, because we can not directly read the value, but when setting it `ColorInfo` can be implicitly converted from a simple `Color`.