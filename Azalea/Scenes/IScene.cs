using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Scenes;

public interface IScene
{
	internal void RunUpdate();
	internal void RunFixedUpdate();
	internal void RunDraw(RenderCoordinator coordinator);
	internal void UpdateSize(Vector2 newSize);
	internal void SetApplication(Application? app);

	internal IEnumerable<ISceneObject> BuildPositionalInputQueue(Vector2 position);
	internal IEnumerable<ISceneObject> BuildNonPositionalInputQueue();
}
