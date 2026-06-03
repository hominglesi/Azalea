using System.Numerics;

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

	public static void Uniform1i(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, int int0)
		=> consumer.Enqueue(Uniform1iCommand.Borrow(uniformLocation, int0));

	public static void Uniform4f(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, float float0, float float1, float float2, float float3)
		=> consumer.Enqueue(Uniform4fCommand.Borrow(uniformLocation, float0, float1, float2, float3));

	public static void UniformMatrix4fv(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, int count, bool transpose, Matrix4x4 matrix)
		=> consumer.Enqueue(UniformMatrix4fvCommand.Borrow(uniformLocation, count, transpose, matrix));

	public static void UseProgram(this IRenderCommandConsumer consumer, Program program)
		=> consumer.Enqueue(UseProgramCommand.Borrow(program));
}
