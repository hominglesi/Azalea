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

[ThreadCommand()]
internal partial class GenerateTextureCommand : LoadingCommand
{
	internal Texture Texture;
}

[ThreadCommand(awaitable: true)]
internal partial class RebindContextCommand : LoadingCommand { }

[ThreadCommand(awaitable: true)]
internal partial class ReleaseContextCommand : LoadingCommand { }

[ThreadCommand()]
internal partial class TexImage2DCommand : LoadingCommand
{
	public Texture Texture;
	public int Width;
	public int Height;
	public byte[]? Pixels;
	public bool GenerateMipmap;
}
