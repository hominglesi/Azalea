using Azalea.Threading;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;
public abstract class LoadingCommand : ThreadCommand
{
	internal static new int TotalCreated = 0;

	internal LoadingCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand]
internal partial class GenerateProgramCommand : LoadingCommand
{
	public Program Program;
	public string VertexShaderCode;
	public string FragmentShaderCode;
}

[ThreadCommand]
internal partial class GenerateTextureCommand : LoadingCommand
{
	public Texture Texture;
}

[ThreadCommand(awaitable: true)]
internal partial class RebindContextCommand : LoadingCommand { }

[ThreadCommand(awaitable: true)]
internal partial class ReleaseContextCommand : LoadingCommand { }

[ThreadCommand(generateHandler: false)]
internal partial class TexImage2DCommand : LoadingCommand
{
	public Texture Texture;
	public int Width;
	public int Height;
	public byte[]? Pixels;
	public bool GenerateMipmap;
}

internal static class TexImage2DCommand_Handler
{
	internal static void TexImage2D(this ICommandHandler<LoadingCommand> handler, Texture texture, int width, int height, byte[]? pixels, bool generateMipmap)
	{
		texture.BeginLoadingOperation();
		handler.Enqueue(TexImage2DCommand.Borrow(texture, width, height, pixels, generateMipmap));
	}
}
