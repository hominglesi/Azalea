namespace Azalea.Platform.Rendering;
public static class IRenderCommandConsumer_Extentions
{
	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, byte[]? data, int hint)
		=> consumer.Enqueue(BufferDataCommand.Borrow(type, size, data, hint));

	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, float[]? data, int hint)
		=> consumer.Enqueue(BufferDataFloatCommand.Borrow(type, size, data, hint));

	public static void BindBuffer(this IRenderCommandConsumer consumer, int type, Buffer? buffer)
		=> consumer.Enqueue(BindBufferCommand.Borrow(type, buffer));
}
