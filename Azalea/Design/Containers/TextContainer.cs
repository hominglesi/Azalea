using Azalea.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azalea.Design.Containers;

public class TextContainer : FlexContainer
{
	private readonly Action<SpriteText>? _defaultCreationParameters;

	public TextContainer(Action<SpriteText>? defaultCreationParameters = null)
	{
		Wrapping = FlexWrapping.Wrap;
		_defaultCreationParameters = defaultCreationParameters;
	}

	public string Text
	{
		set
		{
			Clear();

			AddText(value);
		}
	}

	public void AddText(string text, Action<SpriteText>? creationParameters = null)
	{
		var words = splitText(text);

		for (int i = 0; i < words.Length; i++)
		{
			if (words[i] == "\n")
			{
				if (Children.Count == 0 || Children[Children.Count - 1] is FlowNewLine)
					AddNewLine(20);
				else
					AddNewLine();

				continue;
			}

			var chunkText = new SpriteText()
			{
				Text = words[i]
			};
			_defaultCreationParameters?.Invoke(chunkText);
			creationParameters?.Invoke(chunkText);
			Add(chunkText);
		}
	}

	private string[] splitText(string text)
	{
		var words = new List<string>();
		var builder = new StringBuilder();

		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '\n')
			{
				if (builder.Length > 0)
				{
					words.Add(builder.ToString());
					builder.Clear();
				}

				words.Add("\n");
				continue;
			}

			builder.Append(text[i]);

			if (char.IsSeparator(text[i]))
			{
				words.Add(builder.ToString());
				builder.Clear();
			}
		}

		if (builder.Length > 0)
			words.Add(builder.ToString());

		return words.ToArray();
	}
}
