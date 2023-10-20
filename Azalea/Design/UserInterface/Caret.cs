using Azalea.Design.Containers;
using System.Numerics;

namespace Azalea.Design.UserInterface;

public abstract class Caret : CompositeGameObject
{
	public abstract void DisplayAt(Vector2 position, float? selectionWidth);
}
