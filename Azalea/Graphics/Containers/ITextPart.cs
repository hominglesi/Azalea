using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Containers;

public interface ITextPart
{
	IEnumerable<GameObject> GameObjects { get; }

	event Action<IEnumerable<GameObject>> GameObjectPartsRecreated;

	void RecreateGameObjectsFor(TextContainer textContainer);
}
