using Azalea.Graphics.Colors;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Threading;

namespace Azalea.Graphics.Shaders;

public class Shader
{
	internal Program? Program;

	public Shader(string vertexCode, string fragmentCode, bool autoInitialize = true)
	{
		VertexCode = vertexCode;
		FragmentCode = fragmentCode;

		if (autoInitialize)
			GameHost.Instance.Loader.InitializeShader(this);
	}

	public readonly string VertexCode;
	public readonly string FragmentCode;

	public virtual void InitializationLogic() { }

	public void Uniform(ICommandHandler<RenderCommand> target, string uniformName, float v)
		=> target.Enqueue(Uniform1fCommand.Borrow(this, uniformName, v));

	public void Uniform(ICommandHandler<RenderCommand> target, string uniformName, float v1, float v2)
		=> target.Enqueue(Uniform2fCommand.Borrow(this, uniformName, v1, v2));

	public void Uniform(ICommandHandler<RenderCommand> target, string uniformName, Color color)
		=> target.Enqueue(Uniform4fCommand.Borrow(this, uniformName,
			color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized));
}
