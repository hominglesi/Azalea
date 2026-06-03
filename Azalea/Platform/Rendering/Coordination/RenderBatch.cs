namespace Azalea.Platform.Rendering.Coordination;
public abstract class RenderBatch<T>
	where T : struct
{
	private readonly RenderCoordinator _coordinator;

	public RenderBatch(RenderCoordinator coordinator)
	{
		_coordinator = coordinator;
	}

	public abstract void Add(T vertex);

	internal abstract void Draw(RenderCommandQueue commandQueue);
}
