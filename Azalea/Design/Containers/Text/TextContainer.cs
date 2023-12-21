using Azalea.Caching;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.Design.Containers.Text;

public class TextContainer : FlexContainer
{
	private readonly Action<SpriteText> _defaultCreationParameters;

	private readonly List<ITextPart> _parts = new();
	private readonly Cached _partsCache = new();

	public string Text
	{
		set
		{
			Clear();

			AddText(value);
		}
	}

	private float _firstLineIndent;
	public float FirstLineIndent
	{
		get => _firstLineIndent;
		set
		{
			if (value == _firstLineIndent) return;

			_firstLineIndent = value;

			_layout.Invalidate();
		}
	}

	private float _contentIndent;

	public float ContentIndent
	{
		get => _contentIndent;
		set
		{
			if (value == _contentIndent) return;

			_contentIndent = value;

			_layout.Invalidate();
		}
	}

	private float _paragraphSpacing = 0.5f;

	public float ParagraphSpacing
	{
		get => _paragraphSpacing;
		set
		{
			if (value == _paragraphSpacing) return;

			_paragraphSpacing = value;

			_layout.Invalidate();
		}
	}

	private float _lineSpacing;

	public float LineSpacing
	{
		get => _lineSpacing;
		set
		{
			if (value == _lineSpacing) return;

			_lineSpacing = value;

			_layout.Invalidate();
		}
	}

	public TextContainer(Action<SpriteText> defaultCreationParameters = null)
	{
		Wrapping = FlexWrapping.Wrap;
		_defaultCreationParameters = defaultCreationParameters;
	}

	protected override void InvalidateLayout()
	{
		base.InvalidateLayout();
		_layout.Invalidate();
	}

	protected override void Update()
	{
		base.Update();

		if (_partsCache.IsValid == false)
			RecreateAllParts();
	}

	protected override void UpdateAfterChildren()
	{
		if (!_layout.IsValid)
		{
			computeLayout();
			_layout.Validate();
		}

		base.UpdateAfterChildren();
	}

	public ITextPart AddText(string text, Action<SpriteText>? creationParameters = null)
		=> AddPart(CreateChunkFor(text ??= "", CreateSpriteText, creationParameters));

	protected internal virtual TextChunk<TSpriteText> CreateChunkFor<TSpriteText>(string text, Func<TSpriteText> creationFunc,
		Action<TSpriteText>? creationParameters = null)
		where TSpriteText : SpriteText, new()
		=> new(text, creationFunc, creationParameters);

	public void NewLine() => AddPart(new TextNewLine(false));

	protected internal virtual SpriteText CreateSpriteText() => new SpriteText();

	internal void ApplyDefaultCreationParameters(SpriteText spriteText) => _defaultCreationParameters?.Invoke(spriteText);

	protected internal ITextPart AddPart(ITextPart part)
	{
		_parts.Add(part);

		if (_partsCache.IsValid)
			recreatePart(part);

		return part;
	}

	protected virtual void RecreateAllParts()
	{
		foreach (var manualPart in _parts.OfType<TextPartManual>())
			RemoveRange(manualPart.GameObjects);

		base.Clear();

		foreach (var part in _parts)
			recreatePart(part);

		_partsCache.Validate();
	}

	private void recreatePart(ITextPart part)
	{
		part.RecreateGameObjectsFor(this);
		foreach (var go in part.GameObjects)
			base.Add(go);
	}

	public override void Clear()
	{
		base.Clear();
		_parts.Clear();
	}

	private readonly Cached _layout = new();

	private void computeLayout()
	{
		var childrenByLine = new List<List<GameObject>>();
		var currentLine = new List<GameObject>();

		foreach (var c in Children)
		{
			if (c is NewLineComposition nlc)
			{
				currentLine.Add(nlc);
				childrenByLine.Add(currentLine);
				currentLine = new List<GameObject>();
			}
			else
			{
				/*
				if (c.X == 0)
				{
					if (currentLine.Count > 0)
						childrenByLine.Add(currentLine);
					currentLine = new List<GameObject>();
				}*/

				currentLine.Add(c);
			}
		}

		if (currentLine.Count > 0)
			childrenByLine.Add(currentLine);

		bool isFirstLine = true;
		float lastLineHeight = 0f;

		foreach (var line in childrenByLine)
		{
			bool isFirstChild = true;
			IEnumerable<float> lineBaseHeightValues = line.OfType<IHasLineBaseHeight>().Select(l => l.LineBaseHeight);
			float lineBaseHeight = lineBaseHeightValues.Any() ? lineBaseHeightValues.Max() : 0f;
			float currentLineHeight = 0f;
			float lineSpacingValue = lastLineHeight * LineSpacing;

			foreach (GameObject c in line)
			{
				if (c is NewLineComposition nlc)
				{
					nlc.Height = nlc.IndicatesNewParagraph ? (currentLineHeight == 0 ? lastLineHeight : currentLineHeight) * ParagraphSpacing : 0;
					continue;
				}

				float childLineBaseHeight = (c as IHasLineBaseHeight)?.LineBaseHeight ?? 0f;
				Boundary margin = new()
				{
					Top = (childLineBaseHeight != 0f ? lineBaseHeight - childLineBaseHeight : 0f) + lineSpacingValue
				};

				if (isFirstLine)
					margin.Left = FirstLineIndent;
				else if (isFirstChild)
					margin.Left = ContentIndent;

				c.Margin = margin;

				if (c.Height > currentLineHeight)
					currentLineHeight = c.Height;

				isFirstChild = false;
			}

			if (currentLineHeight != 0f)
				lastLineHeight = currentLineHeight;

			isFirstLine = false;
		}
	}

	public class NewLineComposition : Composition
	{
		public readonly bool IndicatesNewParagraph = true;
		public NewLineComposition() { }
	}
}
