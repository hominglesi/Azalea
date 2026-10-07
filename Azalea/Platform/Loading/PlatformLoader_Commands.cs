using Azalea.Platform.Rendering;
using Azalea.Threading;

namespace Azalea.Platform.Loading;

public abstract class LoadingCommand : ThreadCommand
{
	internal static new int TotalCreated = 0;

	internal LoadingCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class CreateProgramCommand : LoadingCommand
{
	public Program Program;
	public string VertexShaderCode;
	public string FragmentShaderCode;
}

[ThreadCommand(generateHandler: false)]
internal partial class CreateTextureCommand : LoadingCommand
{
	public NativeTexture Texture;
	public int Width;
	public int Height;
	public byte[]? Pixels;
	public bool GenerateMipmap;
}

internal static class CreateTextureCommand_Handler
{
	internal static NativeTexture CreateTexture(this ICommandHandler<LoadingCommand> handler, int width, int height, byte[]? pixels, bool generateMipmap = false, NativeTexture? texture = null)
	{
		texture ??= ((PlatformLoader)handler).CreateEmptyTexture();
		texture.SetSize(width, height);
		texture.BeginLoadingOperation();
		handler.Enqueue(CreateTextureCommand.Borrow(texture, width, height, pixels, generateMipmap));
		return texture;
	}
}

[ThreadCommand]
internal partial class GenerateTextureCommand : LoadingCommand
{
	public NativeTexture Texture;
}

[ThreadCommand(awaitable: true)]
internal partial class RebindContextCommand : LoadingCommand { }

[ThreadCommand(awaitable: true)]
internal partial class ReleaseContextCommand : LoadingCommand { }
