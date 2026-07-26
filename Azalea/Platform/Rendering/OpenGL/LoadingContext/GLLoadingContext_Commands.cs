using Azalea.Threading;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;
internal partial class GLLoadingContext
{
	[ThreadCommand(awaitable: true)]
	internal partial class ReleaseContextCommand : LoadingCommand { }
	public void ReleaseContext()
		=> Thread.Enqueue(ReleaseContextCommand.Borrow())!.Await();

	[ThreadCommand(awaitable: true)]
	internal partial class RebindContextCommand : LoadingCommand { }
	public void RebindContext()
		=> Thread.Enqueue(RebindContextCommand.Borrow())!.Await();
}

internal abstract class LoadingCommand : ThreadCommand
{
	internal static new int TotalCreated = 0;

	internal LoadingCommand()
	{
		TotalCreated++;
	}
}
