namespace Azalea.Platform.Rendering;
public static class IRenderCommandConsumer_Extentions
{
	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, byte[]? data, int hint, bool freeData)
		=> consumer.Enqueue(BufferDataCommand.Borrow(type, size, data, hint, freeData));

	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, float[]? data, int hint, bool freeData)
		=> consumer.Enqueue(BufferDataFloatCommand.Borrow(type, size, data, hint, freeData));

	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, uint[]? data, int hint, bool freeData)
		=> consumer.Enqueue(BufferDataUIntCommand.Borrow(type, size, data, hint, freeData));

	public static void BindBuffer(this IRenderCommandConsumer consumer, int type, Buffer? buffer)
		=> consumer.Enqueue(BindBufferCommand.Borrow(type, buffer));

	public static void BindVertexArray(this IRenderCommandConsumer consumer, VertexArray? vertexArray)
		=> consumer.Enqueue(BindVertexArrayCommand.Borrow(vertexArray));

	public static void PrintErrors(this IRenderCommandConsumer consumer)
		=> consumer.Enqueue(PrintErrorsCommand.Borrow());

	public static void Uniform4f(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, float value0, float value1, float value2, float value3)
		=> consumer.Enqueue(Uniform4fCommand.Borrow(uniformLocation, value0, value1, value2, value3));

	public static void UseProgram(this IRenderCommandConsumer consumer, Program program)
		=> consumer.Enqueue(UseProgramCommand.Borrow(program));
}
