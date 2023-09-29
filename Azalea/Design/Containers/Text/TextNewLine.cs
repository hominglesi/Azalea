using Azalea.Extentions.IEnumerableExtentions;
using Azalea.Graphics;
using System.Collections.Generic;

namespace Azalea.Design.Containers.Text;

public class TextNewLine : TextPart
{
	private readonly bool _indicatesNewParagraph;

	public TextNewLine(bool indicatesNewParagraph)
	{
		_indicatesNewParagraph = indicatesNewParagraph;
	}

	public override IEnumerable<GameObject> CreateGameObjectsFor(TextContainer textComposition)
	{
		var newLineComposition = new TextContainer.NewLineComposition();
		return newLineComposition.Yield();
	}
}
