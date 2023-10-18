using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System;
using System.Collections.Generic;

namespace Azalea.Debugging.BindableDisplays;
public class DebugBindableDisplay<T> : FlexContainer
{
	private readonly object _observedObject;
	private readonly string _observedProperty;
	private T? _currentValue;

	private SpriteText _propertyNameText;

	protected event Action<T>? ValueChanged;

	public DebugBindableDisplay(object obj, string propertyName)
	{
		Direction = FlexDirection.Vertical;
		Wrapping = FlexWrapping.NoWrapping;

		_observedObject = obj;
		_observedProperty = propertyName;

		RelativeSizeAxes = Axes.X;
		Size = new(1, 0);

		AddElement(_propertyNameText = new SpriteText()
		{
			Text = propertyName
		});
	}

	protected override void Update()
	{
		var propertyValue = GetValue();
		if (EqualityComparer<T>.Default.Equals(propertyValue, _currentValue) == false)
		{
			_currentValue = propertyValue;
			ValueChanged?.Invoke(_currentValue);
		}
	}

	protected void AddElement(GameObject obj)
	{
		Add(obj);
		Height += obj.Height;
	}

	protected T GetValue() => (T)_observedObject.GetType().GetProperty(_observedProperty).GetValue(_observedObject, null);
	protected void SetValue(T value) => _observedObject.GetType().GetProperty(_observedProperty).SetValue(_observedObject, value);
}
