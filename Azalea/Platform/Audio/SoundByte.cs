using System;

namespace Azalea.Platform.Audio;
public class SoundByte
{
	internal uint? Handle { get; private set; }

	public float Duration { get; private set; }

	internal SoundByte() { }

	internal void Initialize(uint handle, float duration)
	{
		if (Handle is not null)
			throw new Exception("SoundByte cannot be initialized multiple times!");

		Handle = handle;
		Duration = duration;
	}
}
