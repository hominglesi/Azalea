using Azalea.Graphics.UserInterface;

namespace Azalea.Design.UserInterface;
public class BasicFloatTextBox : BasicTextBox
{
	public float DisplayedFloat
	{
		get => Text == "" ? 0 : float.Parse(Text);
		set
		{
			Text = value == 0 ? "" : value.ToString();
		}
	}

	protected override bool CanAddCharacter(char character)
	{
		try
		{
			var newFloat = float.Parse(Text + character);
			return true;
		}
		catch { return false; }
	}
}
