using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Containers;

public abstract class TextPart : ITextPart
{
	public IEnumerable<GameObject> GameObjects { get; }
	public event Action<IEnumerable<GameObject>>? GameObjectPartsRecreated;

	private readonly List<GameObject> _gameObjects = new List<GameObject>();

	public TextPart()
	{
		GameObjects = _gameObjects.AsReadOnly();
	}

	public void RecreateGameObjectsFor(TextContainer textContainer)
	{
		_gameObjects.Clear();
		_gameObjects.AddRange(CreateGameObjectsFor(textContainer));
		GameObjectPartsRecreated?.Invoke(_gameObjects);
	}

	public abstract IEnumerable<GameObject> CreateGameObjectsFor(TextContainer textContainer);
}
