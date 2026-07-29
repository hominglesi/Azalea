using Azalea.Lists;
using Azalea.Native.OpenGL;
using System;

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
				case TexImage2DCommand(var texture, var width, var height, var pixels, var generateMipmap):
					if (texture.NativeTexture is not GLTexture glTexture)
						throw new Exception();

					GL.BindTexture(glTexture.Target, glTexture.Handle);

					if (pixels is null)
						GL.TexImage2D(glTexture.Target, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, IntPtr.Zero);
					else
						GL.TexImage2D(glTexture.Target, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, in pixels[0]);

					if (generateMipmap)
						GL.GenerateMipmap(glTexture.Target);

					GL.Flush();
					texture.FinishLoadingOperation();
					break;
			}

			command.Return();
		}
	}
}
