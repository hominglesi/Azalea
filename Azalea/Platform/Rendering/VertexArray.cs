using System;

namespace Azalea.Platform.Rendering;
public class VertexArray
{
	internal uint? Handle { get; private set; }

	internal VertexArray() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("VertexArray cannot be initialized multiple times!");

		Handle = handle;
	}
}
