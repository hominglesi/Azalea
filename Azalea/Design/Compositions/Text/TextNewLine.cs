using Azalea.Extentions.IEnumerableExtentions;
using Azalea.Graphics;
using System.Collections.Generic;

namespace Azalea.Design.Compositions.Text;

public class TextNewLine : TextPart
{
	private readonly bool _indicatesNewParagraph;

	public TextNewLine(bool indicatesNewParagraph)
	{
		_indicatesNewParagraph = indicatesNewParagraph;
	}

	public override IEnumerable<GameObject> CreateGameObjectsFor(TextComposition textComposition)
	{
		var newLineComposition = new TextComposition.NewLineComposition();
		return newLineComposition.Yield();
	}
}
