using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System.Collections.Generic;

namespace Azalea.Debugging.BindableDisplays;
public abstract class DebugBindableDisplay<T> : FlexContainer
{
	private readonly object _observedObject;
	private readonly string _observedProperty;
	protected T CurrentValue;

	private SpriteText _propertyNameText;

	public DebugBindableDisplay(object obj, string propertyName)
	{
		Direction = FlexDirection.Vertical;
		Wrapping = FlexWrapping.NoWrapping;

		_observedObject = obj;
		_observedProperty = propertyName;
		CurrentValue = GetValue();

		RelativeSizeAxes = Axes.X;
		Size = new(1, 0);

		AddElement(_propertyNameText = new SpriteText()
		{
			Text = propertyName
		});
	}

	protected abstract void OnValueChanged(T newValue);
	protected override void Update()
	{
		var propertyValue = GetValue();
		if (EqualityComparer<T>.Default.Equals(propertyValue, CurrentValue) == false)
		{
			CurrentValue = propertyValue;
			OnValueChanged(CurrentValue);
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
