using Azalea.Graphics.Containers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Azalea.Graphics;

public class TextPartManual : ITextPart
{
	public IEnumerable<GameObject> GameObjects { get; }

	public event Action<IEnumerable<GameObject>> GameObjectPartsRecreated
	{
		add { }
		remove { }
	}

	public TextPartManual(IEnumerable<GameObject> gameObjects)
	{
		GameObjects = gameObjects.ToImmutableArray();
	}

	public void RecreateGameObjectsFor(TextContainer textContainer)
	{

	}
}
