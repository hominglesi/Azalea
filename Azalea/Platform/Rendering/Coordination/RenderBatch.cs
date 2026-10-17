namespace Azalea.Platform.Rendering.Coordination;

public abstract class RenderBatch<T>(RenderCoordinator coordinator) : IRenderBatch
	where T : struct
{
	private readonly RenderCoordinator _coordinator = coordinator;

	public void Add(T vertex)
	{
		_coordinator.SelectRenderBatch(this);

		AddImplementation(vertex);
	}

	internal abstract void AddImplementation(T vertex);

	internal abstract void Draw(RenderCommandGroup commandQueue);
	void IRenderBatch.Draw(RenderCommandGroup commandQueue) => Draw(commandQueue);
}

internal interface IRenderBatch
{
	void Draw(RenderCommandGroup commandQueue);
}
