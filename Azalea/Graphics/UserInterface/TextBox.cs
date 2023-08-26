using Azalea.Caching;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Azalea.Graphics.UserInterface;

public abstract class TextBox : TabbableContainer
{
	protected FlexContainer TextFlow { get; private set; }
	protected Container TextContainer { get; private set; }

	private bool canAddCharacter(char character) => char.IsControl(character) == false;

	public int? MaxLength;

	private bool _readOnly;
	public bool ReadOnly
	{
		get => _readOnly;
		set
		{
			_readOnly = value;

			if (_readOnly)
				KillFocus();
		}
	}

	public TextBox()
	{
		Children = new GameObject[]
		{
			TextContainer = new Container()
			{
				Size = Vector2.One,
				RelativeSizeAxes = Axes.Both,
				Anchor = Anchor.CenterLeft,
				Origin = Anchor.CenterLeft,
				Position = new Vector2(0, 0),
				Children = new GameObject[]
				{
					TextFlow = new FlexContainer()
					{
						Anchor = Anchor.CenterLeft,
						Origin = Anchor.CenterLeft,
						Direction = FlexDirection.Horizontal,
						Wrapping = FlexWrapping.NoWrapping,
						Size = Vector2.One,
						RelativeSizeAxes = Axes.Both
					}
				}
			}
		};
		HasFocus = true;

		Input.GetTextInput().OnTextInput += handleTextInput;
	}

	private int _selectionStart;
	private int _selectionEnd;

	private int _selectionLength => Math.Abs(_selectionEnd - _selectionStart);
	private int _selectionLeft => Math.Min(_selectionStart, _selectionEnd);
	private int _selectionRight => Math.Max(_selectionStart, _selectionEnd);

	private string _text = string.Empty;

	public virtual string Text
	{
		get => _text;
		set
		{
			if (value == _text) return;

			setText(value);
		}
	}

	protected void InsertString(string value) => insertString(value);

	private void insertString(string value)
	{
		if (string.IsNullOrEmpty(value)) return;

		foreach (char c in value)
		{
			if (canAddCharacter(c) == false)
			{
				NotifyInputError();
				continue;
			}

			if (_selectionLength > 0)
				removeSelection();

			if (_text.Length + 1 > MaxLength)
			{
				NotifyInputError();
				break;
			}

			AddCharacterToFlow(c);

			_text = _text.Insert(_selectionLeft, c.ToString());
			_selectionStart = _selectionEnd = _selectionLeft + 1;

			cursorAndLayout.Invalidate();
		}
	}

	private void setText(string value)
	{
		_selectionStart = _selectionEnd = 0;

		TextFlow?.Clear();
		_text = string.Empty;

		insertString(value);

		cursorAndLayout.Invalidate();
	}

	protected virtual GameObject GetGameObjectCharacter(char c) =>
		new SpriteText { Text = c.ToString(), Font = new FontUsage(size: CalculatedTextSize) };

	protected virtual GameObject AddCharacterToFlow(char c)
	{
		List<GameObject> charsRight = new();
		foreach (var go in TextFlow.Children.Skip(_selectionLeft))
			charsRight.Add(go);
		TextFlow.RemoveRange(charsRight);

		int i = _selectionLeft;
		foreach (var go in charsRight)
			go.Depth = getDepthForCharacterIndex(i++);

		GameObject ch = GetGameObjectCharacter(c);
		ch.Depth = getDepthForCharacterIndex(_selectionLeft);

		TextFlow.Add(ch);
		TextFlow.AddRange(charsRight);

		return ch;
	}

	private float getDepthForCharacterIndex(int index) => -index;

	protected float CalculatedTextSize => TextFlow.DrawSize.Y - (TextFlow.Padding.Top + TextFlow.Padding.Bottom);

	#region Removing Text

	private string removeSelection() => removeCharacters(_selectionLength);

	private string removeCharacters(int number = 1)
	{
		if (_text.Length == 0) return string.Empty;

		int removeStart = Math.Clamp(_selectionRight - number, 0, _selectionRight);
		int removeCount = _selectionRight - removeStart;

		if (removeCount == 0) return string.Empty;

		Debug.Assert(_selectionLength == 0 || removeCount == _selectionLength);

		foreach (var c in TextFlow.Children.Skip(removeStart).Take(removeCount).ToArray())
		{
			TextFlow.Remove(c);
		}

		string removedText = _text.Substring(removeStart, removeCount);
		_text = _text.Remove(removeStart, removeCount);

		for (int i = removeStart; i < TextFlow.Count; i++)
			TextFlow.ChangeChildDepth(TextFlow[i], getDepthForCharacterIndex(i));

		_selectionStart = _selectionEnd = removeStart;

		cursorAndLayout.Invalidate();

		return removedText;
	}

	protected void DeleteBy(int amount)
	{
		if (_selectionLength == 0)
			_selectionEnd = Math.Clamp(_selectionStart + amount, 0, _text.Length);

		if (_selectionLength > 0)
		{
			removeSelection();
		}
	}

	#endregion

	private readonly Cached cursorAndLayout = new();

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (cursorAndLayout.IsValid == false)
		{
			updateCursorAndLayout();
			cursorAndLayout.Validate();
		}
	}

	#region Virtual Methods

	protected virtual void NotifyInputError()
	{

	}

	#endregion

	private void updateCursorAndLayout()
	{
		float cursorPos = 0;
		if (_text.Length > 0)
			cursorPos = getPositionAt(_selectionLeft);

		float cursorPosEnd = getPositionAt(_selectionEnd);

		float? selectionWidth = null;
		if (_selectionLength > 0)
			selectionWidth = getPositionAt(_selectionRight) - cursorPos;
	}

	private float getPositionAt(int index)
	{
		if (index > 0)
		{
			if (index < _text.Length)
				return TextFlow.Children[index].DrawPosition.X + TextFlow.DrawPosition.X;

			var d = TextFlow.Children[index - 1];
			return d.DrawPosition.X + d.DrawSize.X + /*TextFlow.Spacing.X + */ TextFlow.DrawPosition.X;
		}

		return 0;
	}

	protected virtual void KillFocus() => killFocus();

	private void killFocus()
	{
		var manager = GetContainingInputManager();
		if (manager?.FocusedObject == this)
			manager.ChangeFocus(null);
	}

	private void handleTextInput(string text)
	{
		InsertString(text);
	}
}
