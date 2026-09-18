using Azalea.Caching;
using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Utils;
using Azalea.Utils.Proxies;

namespace Azalea.Editor.Design.Gui;
public class GUIPropertyLabel<T> : TextContainer
{
	private readonly string _label;
	private readonly IProxy<T> _proxy;
	private readonly string? _displayString;

	internal GUIPropertyLabel(string label, IProxy<T> proxy, string? displayString = null)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		_label = label;
		_proxy = proxy;
		_displayString = displayString;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;
	}

	protected override void Update()
	{
		if (_proxy.HasNewValue(out T newValue))
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
