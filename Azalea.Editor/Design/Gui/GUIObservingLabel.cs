using Azalea.Caching;
using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Utils;

namespace Azalea.Editor.Design.Gui;
public class GUIObservingLabel<T> : TextContainer
	where T : unmanaged
{
	private readonly string _label;
	private readonly IObservable<T> _observable;
	private readonly Cached _valueValid = new();

	internal GUIObservingLabel(string label, IObservable<T> observable)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		_label = label;
		_observable = observable;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		observable.OnValueChanged += _ => _valueValid.Invalidate();
	}

	protected override void Update()
	{
		if (_valueValid.IsValid == false)
		{
			Text = $"{_label}: {_observable.Value}";
			_valueValid.Validate();
		}
	}
}
