using Azalea.Graphics.Textures;
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

public abstract partial class PlatformLoader
{
	[ThreadCommand]
	internal partial class CreateProgramCommand : LoadingCommand
	{
		public Program Program;
		public string VertexShaderCode;
		public string FragmentShaderCode;
	}

	[ThreadCommand]
	internal partial class CreateTextureCommand : LoadingCommand
	{
		public NativeTexture Texture;
		public int Width;
		public int Height;
		public byte[]? Pixels;
		public bool GenerateMipmap;
		public TextureFiltering Filtering;
	}

	internal NativeTexture CreateTexture(int width, int height, byte[]? pixels, bool generateMipmap, TextureFiltering filtering, NativeTexture? texture = null)
	{
		texture ??= CreateEmptyTexture();
		texture.Width = width;
		texture.Height = height;
		Enqueue(CreateTextureCommand.Borrow(texture, width, height, pixels, generateMipmap, filtering));
		return texture;
	}

	[ThreadCommand(awaitable: true)]
	internal partial class RebindContextCommand : LoadingCommand;
	internal ICommandAwaitable RebindContext() => Enqueue(RebindContextCommand.Borrow())!;

	[ThreadCommand(awaitable: true)]
	internal partial class ReleaseContextCommand : LoadingCommand;
	internal ICommandAwaitable ReleaseContext() => Enqueue(ReleaseContextCommand.Borrow())!;
}
