using System;

namespace Azalea.Platform.Rendering;
public class Shader
{
	internal uint? Handle { get; private set; }

	internal Shader() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("Shader cannot be initialized multiple times!");

		Handle = handle;
	}
}
