using Azalea.Editor.Design.Gui;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;
using Azalea.Threading;

namespace Azalea.Editor.DebugWindows.Inspectors;
internal class GLLoadingContextInspector
{
	public static void Inject(GUIWindow window, GLLoadingContext loadingContext)
	{
		window.AddGroup("GL Loading Context");

		window.AddGroup("GL Loading Thread");
		var thread = loadingContext.Thread;

		var textureCount = window.AddCounter("LoadedTextures: ", thread.LoadedTextures.Count);
		thread.LoadedTextures.OnChanged += () => Scheduler.Schedule(
			() => textureCount.Value = thread.LoadedTextures.Count);

		var programCount = window.AddCounter("LoadedPrograms: ", thread.LoadedPrograms.Count);
		thread.LoadedPrograms.OnChanged += () => Scheduler.Schedule(
			() => programCount.Value = thread.LoadedPrograms.Count);

		GameThreadInspector.Inject(window, thread);

		window.FinishGroup();
		window.FinishGroup();
	}
}
