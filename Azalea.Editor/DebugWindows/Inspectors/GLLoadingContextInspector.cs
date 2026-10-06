using Azalea.Editor.Design.Gui;
using Azalea.Platform.Loading;
using Azalea.Threading;

namespace Azalea.Editor.DebugWindows.Inspectors;

internal class GLLoadingContextInspector
{
	public static void Inject(GUIWindow window, PlatformLoader loader)
	{
		window.AddGroup("GL Loading Context");

		window.AddGroup("GL Loading Thread");
		var thread = loader.Thread;

		var textureCount = window.AddCounter("LoadedTextures: ", loader.LoadedTextures.Count);
		loader.LoadedTextures.OnChanged +=
			() => window.App.Scheduler.InvokeAction(() => textureCount.Value = loader.LoadedTextures.Count);

		var programCount = window.AddCounter("LoadedPrograms: ", loader.LoadedShaders.Count);
		loader.OnShaderLoaded +=
			_ => window.App.Scheduler.InvokeAction(() => programCount.Value = loader.LoadedShaders.Count);

		GameThreadInspector.Inject(window, thread);

		window.FinishGroup();
		window.FinishGroup();
	}
}
