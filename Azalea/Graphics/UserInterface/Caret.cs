using Azalea.Design.Compositions;
using System.Numerics;

namespace Azalea.Graphics.UserInterface;

public abstract class Caret : CompositeGameObject
{
	public abstract void DisplayAt(Vector2 position, float? selectionWidth);
}
