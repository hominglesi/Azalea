using Azalea.Graphics;
using System;
using System.Collections.Generic;

namespace Azalea.Design.Containers.Text;

public interface ITextPart
{
	IEnumerable<GameObject> GameObjects { get; }

	event Action<IEnumerable<GameObject>> GameObjectPartsRecreated;

	void RecreateGameObjectsFor(TextContainer textComposition);
}
