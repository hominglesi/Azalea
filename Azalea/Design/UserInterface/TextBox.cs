using Azalea.Design.Containers;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System;
using System.Numerics;

namespace Azalea.Design.UserInterface;
public abstract class TextBox : TextContainer
{
	private int _caratPosition = 0;
	public TextBox(Action<SpriteText>? defaultCreationParameters = null)
		: base(defaultCreationParameters)
	{
		Input.OnTextInput += onTextInput;
	}

	private string _text = "";

	public new string Text
	{
		get => _text;
		set
		{
			if (value == _text) return;

			_text = value;

			updateText(_text);
		}
	}

	public override bool AcceptsFocus => true;

	#region Input

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (HasFocus == false) return false;

		if (e.Key == Keys.Backspace && _text.Length > 0) removeCharacterAtCarat();
		if (e.Key == Keys.Enter) addCharacterAtCarat('\n');
		if (e.Key == Keys.Left) moveCarat(-1);
		if (e.Key == Keys.Right) moveCarat(1);

		return base.OnKeyDown(e);
	}

	private void onTextInput(char chr)
	{
		if (HasFocus == false) return;

		if (char.IsControl(chr))
			return;

		addCharacterAtCarat(chr);
	}

	#endregion

	#region Character Manipulation

	private void addCharacterAtCarat(char chr)
	{
		if (IsCharacterAllowed(chr) == false) return;

		var newText = _text.Insert(_caratPosition, chr.ToString());
		_caratPosition++;

		updateText(newText);
	}

	protected virtual bool IsCharacterAllowed(char chr) => true;

	private void removeCharacterAtCarat()
	{
		if (_caratPosition == 0) return;

		var newText = _text.Substring(0, _caratPosition - 1);

		if (_caratPosition < _text.Length)
			newText += _text.Substring(_caratPosition, _text.Length - _caratPosition);

		_caratPosition--;

		updateText(newText);
	}

	private void moveCarat(int change)
	{
		var newPosition = _caratPosition + change;
		if (newPosition < 0) newPosition = 0;
		if (newPosition > _text.Length) newPosition = _text.Length;

		_caratPosition = newPosition;

		updateCaratPosition();
	}

	#endregion

	#region Display

	protected abstract void OnCaratPositionChanged(Vector2 position);

	private void updateText(string newText)
	{
		_text = newText;
		base.Text = _text;

		if (_caratPosition > _text.Length)
			_caratPosition = _text.Length;

		PerformLayout();

		updateCaratPosition();
	}

	private void updateCaratPosition()
	{
		int caratIndex = 0;
		int childIndex = 0;
		float childPosition = 1;

		while (caratIndex < _caratPosition)
		{
			var child = Children[childIndex];
			if (child is SpriteText text)
			{
				if (_caratPosition - caratIndex < text.Text.Length)
				{
					childPosition = (float)(_caratPosition - caratIndex) / text.Text.Length;
				}


				caratIndex += text.Text.Length;
			}
			else if (child is FlowNewLine line)
			{
				caratIndex++;
			}

			childIndex++;
		}

		if (childIndex > 0)
			childIndex--;

		var caratPosition = Vector2.Zero;

		if (Children.Count > 0 && _caratPosition > 0)
		{
			var child = Children[childIndex];
			caratPosition = child.Position;
			caratPosition.X += child.Width * childPosition;

			if (child is FlowNewLine line)
			{
				caratPosition.Y += line.Length;
			}
		}

		OnCaratPositionChanged(caratPosition);
	}

	#endregion
}

