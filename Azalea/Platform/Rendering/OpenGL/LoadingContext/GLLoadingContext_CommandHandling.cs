using Azalea.Lists;
using Azalea.Native.OpenGL;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;
internal partial class GLLoadingContext
{
	internal partial class GLLoadingThread
	{
		internal ObservableList<Texture> LoadedTextures { get; } = [];

		protected override void HandleCommand(LoadingCommand command)
		{
			switch (command)
			{
				case GenerateTextureCommand(var texture):
					uint textureHandle = 0;
					GL.GenTextures(1, ref textureHandle);
					texture.Initialize(new GLTexture(textureHandle));
					LoadedTextures.Add(texture);
					break;
				case RebindContextCommand():
					Context!.MakeCurrent();
					break;
				case ReleaseContextCommand():
					Context!.Release();
					break;
			}

			command.Return();
		}
	}
}
