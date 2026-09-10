using System;

namespace Azalea.Platform.Audio;
public class SoundByte(double duration)
{
	internal uint? Handle { get; private set; }

	public readonly double Duration = duration;

	internal void Initialize(uint handle)
	{
		if (Handle is not null)
			throw new Exception("SoundByte cannot be initialized multiple times!");

		Handle = handle;
	}
}
