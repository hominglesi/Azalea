using System;

namespace Azalea.Platform.Rendering;
public class Framebuffer
{
	internal uint? Handle { get; private set; }

	internal Framebuffer() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("Framebuffer cannot be initialized multiple times!");

		Handle = handle;
	}
}
