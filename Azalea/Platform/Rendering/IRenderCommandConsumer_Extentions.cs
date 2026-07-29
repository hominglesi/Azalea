using Azalea.Threading;
using System.Numerics;

namespace Azalea.Platform.Rendering;
public static class IRenderCommandConsumer_Extentions
{
	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, byte[]? data, int hint, bool freeData, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(BufferDataCommand.Borrow(type, size, data, hint, freeData), commandGroup);

	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, float[]? data, int hint, bool freeData, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(BufferDataFloatCommand.Borrow(type, size, data, hint, freeData), commandGroup);

	public static void BufferData(this IRenderCommandConsumer consumer, int type, nint size, uint[]? data, int hint, bool freeData, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(BufferDataUIntCommand.Borrow(type, size, data, hint, freeData), commandGroup);

	public static void BindBuffer(this IRenderCommandConsumer consumer, int type, Buffer? buffer, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(BindBufferCommand.Borrow(type, buffer), commandGroup);

	public static void BindTexture(this IRenderCommandConsumer consumer, int type, Texture texture, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(BindTextureCommand.Borrow(type, texture), commandGroup);

	public static void BindVertexArray(this IRenderCommandConsumer consumer, VertexArray? vertexArray, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(BindVertexArrayCommand.Borrow(vertexArray), commandGroup);

	public static void PrintErrors(this IRenderCommandConsumer consumer, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(PrintErrorsCommand.Borrow(), commandGroup);

	public static void Uniform1i(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, int int0, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(Uniform1iCommand.Borrow(uniformLocation, int0), commandGroup);

	public static void Uniform4f(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, float float0, float float1, float float2, float float3, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(Uniform4fCommand.Borrow(uniformLocation, float0, float1, float2, float3), commandGroup);

	public static void UniformMatrix4fv(this IRenderCommandConsumer consumer, UniformLocation uniformLocation, int count, bool transpose, Matrix4x4 matrix, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(UniformMatrix4fvCommand.Borrow(uniformLocation, count, transpose, matrix), commandGroup);

	public static void UseProgram(this IRenderCommandConsumer consumer, Program program, ICommandGroup? commandGroup = null)
		=> consumer.Enqueue(UseProgramCommand.Borrow(program), commandGroup);
}
