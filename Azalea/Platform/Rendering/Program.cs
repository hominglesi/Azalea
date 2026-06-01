using System;

namespace Azalea.Platform.Rendering;
public class Program
{
	internal uint? Handle { get; private set; }

	internal Program() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("Program cannot be initialized multiple times!");

		Handle = handle;
	}
}
