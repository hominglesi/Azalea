using Azalea.Graphics;
using System;
using System.Collections.Generic;

namespace Azalea.Design.Compositions.Text;

public interface ITextPart
{
	IEnumerable<GameObject> GameObjects { get; }

	event Action<IEnumerable<GameObject>> GameObjectPartsRecreated;

	void RecreateGameObjectsFor(TextComposition textComposition);
}
