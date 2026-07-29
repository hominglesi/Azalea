using Azalea.Threading;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;
public abstract class LoadingCommand : ThreadCommand
{
	internal static new int TotalCreated = 0;

	internal LoadingCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand(awaitable: true)]
internal partial class ReleaseContextCommand : LoadingCommand { }

[ThreadCommand(awaitable: true)]
internal partial class RebindContextCommand : LoadingCommand { }
