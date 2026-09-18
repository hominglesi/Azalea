using Azalea.Caching;
using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Utils;

namespace Azalea.Editor.Design.Gui;
public class GUIObservingLabel<T> : TextContainer
{
	private readonly string _label;
	private readonly ObservableProxy<T> _observable;
	private readonly string? _displayString;

	internal GUIObservingLabel(string label, ObservableProxy<T> observable, string? displayString = null)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		_label = label;
		_observable = observable;
		_displayString = displayString;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;
	}

	protected override void Update()
	{
		if (_observable.TryGetInvalid(out T newValue))
			Text = $"{_label}: {format(newValue)}";
	}

	private string format(object? obj)
	{
		if (obj is null)
			return "null";

		if (_displayString is not null)
			return string.Format(_displayString, obj);

		return obj.ToString()!;
	}
}
