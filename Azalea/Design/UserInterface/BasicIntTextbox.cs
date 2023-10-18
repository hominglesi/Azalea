using Azalea.Graphics.UserInterface;

namespace Azalea.Design.UserInterface;
public class BasicIntTextBox : BasicTextBox
{
	public int DisplayedInt
	{
		get => Text == "" ? 0 : int.Parse(Text);
		set { Text = value == 0 ? "" : value.ToString(); }
	}

	protected override bool CanAddCharacter(char character)
	{
		try
		{
			var newInt = int.Parse(Text + character);
			return true;
		}
		catch { return false; }
	}
}
