using Azalea.Extentions.IEnumerableExtentions;
using System.Collections.Generic;

namespace Azalea.Graphics.Containers;

public class TextNewLine : TextPart
{
	private readonly bool _indicatesNewParagraph;

	public TextNewLine(bool indicatesNewParagraph)
	{
		_indicatesNewParagraph = indicatesNewParagraph;
	}

	public override IEnumerable<GameObject> CreateGameObjectsFor(TextContainer textContainer)
	{
		var newLineContainer = new TextContainer.NewLineContainer();
		return newLineContainer.Yield();
	}
}
