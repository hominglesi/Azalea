using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs.Events;
using System;

namespace Azalea.Design.UserInterface;
public class Checkbox : Composition
{
	public bool Checked { get; private set; }

	protected Composition _box;

	public event Action<bool>? Toggled;

	public Checkbox()
	{
		Size = new(40, 40);
		Add(_box = new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Color = Palette.White,
			BorderColor = Palette.Black
		});
		_box.BorderObject.OutsideContent = false;
	}

	protected override bool OnClick(ClickEvent e)
	{
		Toggle();
		return true;
	}

	public void Toggle()
	{
		if (Checked)
		{
			_box.BackgroundColor = new Color(0, 0, 0, 0);
			Checked = false;
		}
		else
		{
			_box.BackgroundColor = Palette.Black;
			Checked = true;
		}
		Toggled?.Invoke(Checked);
	}
}
