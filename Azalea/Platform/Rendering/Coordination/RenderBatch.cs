namespace Azalea.Platform.Rendering.Coordination;
public abstract class RenderBatch<T> : IRenderBatch
	where T : struct
{
	private readonly RenderCoordinator _coordinator;

	public RenderBatch(RenderCoordinator coordinator)
	{
		_coordinator = coordinator;
	}

	public void Add(T vertex)
	{
		_coordinator.SelectRenderBatch(this);

		AddImplementation(vertex);
	}

	internal abstract void AddImplementation(T vertex);

	internal abstract void Draw(RenderCommandQueue commandQueue);
	void IRenderBatch.Draw(RenderCommandQueue commandQueue) => Draw(commandQueue);
}

internal interface IRenderBatch
{
	void Draw(RenderCommandQueue commandQueue);
}
