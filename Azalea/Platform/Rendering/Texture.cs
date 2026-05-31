using System;

namespace Azalea.Platform.Rendering;
public class Texture
{
	internal uint? Handle { get; private set; }

	internal Texture() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("Texture cannot be initialized multiple times!");

		Handle = handle;
	}
}
