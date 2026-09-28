using Azalea.Native.OpenGL;
using System;

namespace Azalea.Platform.Rendering.OpenGL;
internal class GLTexture : NativeTexture
{
	public uint Handle { get; private set; } = 0;

	internal void Initialize(uint handle)
	{
		if (Handle != 0)
			throw new Exception("GLTexture cannot be initialized multiple times!");

		Handle = handle;
		FinishLoadingOperation();
	}

	internal override bool IsReady() => Handle != 0;
}
