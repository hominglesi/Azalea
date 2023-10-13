using Azalea.Design.Containers;
using Azalea.Graphics;
using System;

namespace Azalea.Debugging.BindableDisplays;
public class DebugBindableDisplay<T> : Composition where T : class
{
	private readonly object _observedObject;
	private readonly string _observedProperty;
	private T? _currentValue;

	protected event Action<T>? ValueChanged;

	public DebugBindableDisplay(object obj, string propertyName)
	{
		_observedObject = obj;
		_observedProperty = propertyName;

		RelativeSizeAxes = Axes.X;
		Size = new(1, 24);
	}

	protected override void Update()
	{
		var propertyValue = GetValue();
		if (propertyValue != _currentValue)
		{
			_currentValue = propertyValue;
			ValueChanged?.Invoke(_currentValue);
		}
	}

	protected T GetValue() => (T)_observedObject.GetType().GetProperty(_observedProperty).GetValue(_observedObject, null);
	protected void SetValue(T value) => _observedObject.GetType().GetProperty(_observedProperty).SetValue(_observedObject, value);
}
