using Azalea.Threading;

namespace Azalea.Platform.Rendering;
public interface IRenderCommandConsumer
{
	internal void Enqueue(RenderCommand command, ICommandGroup? commandGroup);
}
