using System;

namespace Azalea.Platform.Rendering;
public class Buffer
{
	internal uint? Handle { get; private set; }

	internal Buffer() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("Buffer cannot be initialized multiple times!");

		Handle = handle;
	}
}
