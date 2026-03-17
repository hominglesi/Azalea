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
		_box.BorderAlignment = BorderAlignment.Inner;
	}

	protected override bool OnClick(ClickEvent e)
	{
		SetChecked(!Checked);
		return true;
	}

	public void SetChecked(bool isChecked)
	{
		if (Checked == isChecked) return;

		Checked = isChecked;
		_box.BackgroundColor = isChecked ? Palette.Black : new Color(0, 0, 0, 0);
		Toggled?.Invoke(Checked);
	}
}
