using Azalea.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azalea.Graphics.Containers;

public class TextChunk<TSpriteText> : TextPart
	where TSpriteText : SpriteText, new()
{
	private readonly string _text;
	private readonly Func<TSpriteText> _creationFunc;
	private readonly Action<TSpriteText>? _creationParameters;
	public TextChunk(string text, Func<TSpriteText> creationFunc, Action<TSpriteText>? creationParameters = null)
	{
		_text = text;
		_creationFunc = creationFunc;
		_creationParameters = creationParameters;
	}

	public override IEnumerable<GameObject> CreateGameObjectsFor(TextContainer textContainer)
	{
		var gameObjects = new List<GameObject>();

		gameObjects.AddRange(CreateGameObjectsFor(_text, textContainer));
		return gameObjects;
	}

	protected virtual IEnumerable<GameObject> CreateGameObjectsFor(string text, TextContainer textContainer)
	{
		bool first = true;
		var sprites = new List<GameObject>();

		foreach (var l in text.Split('\n'))
		{
			/*
			if (!first)
			{
				GameObject? lastChild = sprites.LastOrDefault() ?? textContainer.Children.LastOrDefault();

				if (lastChild is not null)
				{
					var newLine = new TextContainer.NewLineContainer();
					sprites.Add(newLine);
				}
			}*/

			foreach (var word in SplitWords(l))
			{
				if (string.IsNullOrEmpty(word)) continue;

				var textSprite = CreateSpriteText(textContainer);
				textSprite.Text = word;
				sprites.Add(textSprite);
			}

			first = false;
		}

		return sprites;
	}

	protected string[] SplitWords(string text)
	{
		var words = new List<string>();
		var builder = new StringBuilder();

		for (int i = 0; i < text.Length; i++)
		{
			if (i == 0 || char.IsSeparator(text[i - 1]) || char.IsControl(text[i - 1]))
			{
				words.Add(builder.ToString());
				builder.Clear();
			}

			builder.Append(text[i]);
		}

		if (builder.Length > 0)
			words.Add(builder.ToString());

		return words.ToArray();
	}

	protected virtual TSpriteText CreateSpriteText(TextContainer textContainer)
	{
		var spriteText = _creationFunc.Invoke();
		textContainer.ApplyDefaultCreationParameters(spriteText);
		_creationParameters?.Invoke(spriteText);
		return spriteText;
	}
}
