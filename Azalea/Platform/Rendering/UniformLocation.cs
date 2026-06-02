using System;

namespace Azalea.Platform.Rendering;
public class UniformLocation
{
	internal int? Handle { get; private set; }

	internal UniformLocation() { }

	internal void Initialize(int handle)
	{
		if (Handle is not null)
			throw new Exception("Uniform Location cannot be initialized multiple times!");

		Handle = handle;
	}
}
