using Azalea.Caching;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Azalea.Graphics.UserInterface;

public abstract class TextBox : Container
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

	private readonly Caret _caret;

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
					_caret = CreateCaret(),
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
	}

	private int _selectionStart;
	private int _selectionEnd;

	private int _selectionLength => Math.Abs(_selectionEnd - _selectionStart);
	private int _selectionLeft => Math.Min(_selectionStart, _selectionEnd);
	private int _selectionRight => Math.Max(_selectionStart, _selectionEnd);

	public string SelectedText => _selectionLength > 0 ? Text.Substring(_selectionLeft, _selectionLength) : string.Empty;

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

	private bool _caretVisible;

	private void updateCaretVisibility()
	{
		bool newVisibility = HasFocus;

		if (_caretVisible != newVisibility)
		{
			_caretVisible = newVisibility;

			if (_caretVisible)
				_caret.Alpha = 1;
			else
				_caret.Alpha = 0;

			_cursorAndLayout.Invalidate();
		}
	}

	protected abstract Caret CreateCaret();

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

			_cursorAndLayout.Invalidate();
		}
	}

	private void setText(string value)
	{
		_selectionStart = _selectionEnd = 0;

		TextFlow?.Clear();
		_text = string.Empty;

		insertString(value);

		_cursorAndLayout.Invalidate();
	}

	protected void MoveCursorBy(int amount)
	{
		_selectionStart = _selectionEnd;
		_cursorAndLayout.Invalidate();
		moveSelection(amount, false);
	}

	protected void ExpandSelectionBy(int amount)
	{
		moveSelection(amount, true);
	}

	private void moveSelection(int offset, bool expand)
	{
		int oldStart = _selectionStart;
		int oldEnd = _selectionEnd;

		if (expand)
			_selectionEnd = Math.Clamp(_selectionEnd + offset, 0, _text.Length);
		else
		{
			if (_selectionLength > 0 && Math.Abs(offset) <= 1)
			{
				if (offset > 0)
					_selectionEnd = _selectionStart = _selectionRight;
				else
					_selectionEnd = _selectionStart = _selectionLeft;
			}
			else
				_selectionEnd = _selectionStart = Math.Clamp((offset > 0 ? _selectionRight : _selectionLeft) + offset, 0, _text.Length);
		}

		if (oldStart != _selectionStart || oldEnd != _selectionEnd)
		{
			_cursorAndLayout.Invalidate();
		}
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

		_cursorAndLayout.Invalidate();

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

	private readonly Cached _cursorAndLayout = new();

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (_cursorAndLayout.IsValid == false)
		{
			updateCaretVisibility();
			updateCursorAndLayout();
			_cursorAndLayout.Validate();
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

		if (_caretVisible)
			_caret.DisplayAt(new Vector2(cursorPos, 0), selectionWidth);
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

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		var controlPressed = Input.GetKey(Keys.ControlLeft).Pressed || Input.GetKey(Keys.ControlRight).Pressed;
		var shiftPressed = Input.GetKey(Keys.ShiftLeft).Pressed || Input.GetKey(Keys.ShiftRight).Pressed;

		switch (e.Key)
		{
			case Keys.A:
				if (controlPressed) return onAction(PlatformAction.SelectAll);
				return false;
			case Keys.V:
				if (controlPressed) return onAction(PlatformAction.Paste);
				return false;
			case Keys.C:
				if (controlPressed) return onAction(PlatformAction.Copy);
				return false;
			case Keys.X:
				if (controlPressed) return onAction(PlatformAction.Cut);
				return false;
			case Keys.Backspace: return onAction(PlatformAction.DeleteBackwardChar);
			case Keys.Left:
				if (shiftPressed) return onAction(PlatformAction.SelectBackwardChar);
				return onAction(PlatformAction.MoveBackwardChar);
			case Keys.Right:
				if (shiftPressed) return onAction(PlatformAction.SelectForwardChar);
				return onAction(PlatformAction.MoveForwardChar);
		}

		return false;
	}

	private bool onAction(PlatformAction action)
	{
		if (HasFocus == false) return false;

		switch (action)
		{
			case PlatformAction.Cut:
			case PlatformAction.Copy:
				if (string.IsNullOrEmpty(SelectedText)) return true;

				AzaleaGame.Main.Host.Clipboard.SetText(SelectedText);

				if (action == PlatformAction.Cut)
					DeleteBy(0);

				return true;

			case PlatformAction.Paste:
				var clipboardText = AzaleaGame.Main.Host.Clipboard.GetText();
				if (clipboardText is null) return false;
				InsertString(clipboardText);
				return true;

			case PlatformAction.SelectAll:
				_selectionStart = 0;
				_selectionEnd = _text.Length;
				_cursorAndLayout.Invalidate();
				return true;

			case PlatformAction.MoveForwardChar:
				MoveCursorBy(1);
				return true;

			case PlatformAction.MoveBackwardChar:
				MoveCursorBy(-1);
				return true;

			case PlatformAction.DeleteBackwardChar:
				DeleteBy(-1);
				return true;

			case PlatformAction.SelectBackwardChar:
				ExpandSelectionBy(-1);
				return true;

			case PlatformAction.SelectForwardChar:
				ExpandSelectionBy(1);
				return true;
		}

		return false;
	}

	public override bool AcceptsFocus => true;

	protected override void OnFocus(FocusEvent e)
	{
		Input.OnTextInput += handleTextInput;
		updateCaretVisibility();

		base.OnFocus(e);
	}

	protected override void OnFocusLost(FocusLostEvent e)
	{
		Input.OnTextInput -= handleTextInput;
		updateCaretVisibility();

		base.OnFocusLost(e);
	}

	protected virtual void KillFocus() => killFocus();

	private void killFocus()
	{
		Input.ChangeFocus(null);
	}

	private void handleTextInput(char text)
	{
		InsertString(text.ToString());
	}
}
