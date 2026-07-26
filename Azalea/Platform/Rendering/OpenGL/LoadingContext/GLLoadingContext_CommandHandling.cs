namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;
internal partial class GLLoadingContext
{
	internal partial class GLLoadingThread
	{
		private void handleCommand(LoadingCommand command)
		{
			switch (command)
			{
				case ReleaseContextCommand():
					Context!.Release();
					break;
				case RebindContextCommand():
					Context!.MakeCurrent();
					break;
			}

			command.Return();
		}
	}
}
