using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Design.Scenes;
public class SceneContainer : Composition
{
	public Scene? CurrentScene { get; private set; }

	public SceneContainer()
	{
		RelativeSizeAxes = Axes.Both;
	}

	public void ChangeScene(Scene? newScene)
	{
		if (CurrentScene == newScene)
			return;

		if (CurrentScene is not null)
			Remove(CurrentScene);

		if (newScene is not null)
			Add(newScene);

		CurrentScene = newScene;
	}
}
