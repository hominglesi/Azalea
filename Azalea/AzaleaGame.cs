using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Scenes;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea;

public abstract class AzaleaGame : Composition, IScene
{
	public AzaleaGame()
	{
		RelativeSizeAxes = Axes.Both;
	}

	void IScene.Update() => UpdateSubTree();
	void IScene.FixedUpdate() => FixedUpdateSubTree();
	void IScene.Draw(RenderCoordinator coordinator) => Draw(coordinator);
	void IScene.UpdateSize(Vector2 newSize) => Size = newSize;
	void IScene.SetApplication(Application? app) => App = app;

	IEnumerable<ISceneObject> IScene.BuildNonPositionalInputQueue()
	{
		var inputQueue = new List<GameObject>();
		BuildNonPositionalInputQueue(inputQueue);
		inputQueue.Reverse();

		return inputQueue;
	}

	IEnumerable<ISceneObject> IScene.BuildPositionalInputQueue(Vector2 position)
	{
		var inputQueue = new List<GameObject>();
		BuildPositionalInputQueue(position, inputQueue);
		inputQueue.Reverse();

		return inputQueue;
	}
}
