using System;

namespace Azalea.Platform.Audio;
public class SoundByte
{
	internal uint? Handle { get; private set; }

	internal SoundByte() { }

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("SoundByte cannot be initialized multiple times!");

		Handle = handle;
	}
}
