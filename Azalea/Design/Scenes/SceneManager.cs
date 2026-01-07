using Azalea.Platform;

namespace Azalea.Design.Scenes;
public static class SceneManager
{
	private static SceneContainer? _instance;
	public static SceneContainer Instance => _instance ??= GameHost.Main.SceneManager;

	public static Scene? CurrentScene => Instance.CurrentScene;
	public static void ChangeScene(Scene? newScene) => Instance.ChangeScene(newScene);
}
