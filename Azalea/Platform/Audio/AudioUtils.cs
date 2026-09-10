using Azalea.Native.OpenAL;
using System;

namespace Azalea.Platform.Audio;
public static class AudioUtils
{
	public static double CalculateDuration(byte[] pcmData, int dataLength, int format, int frequency)
	{
		var channels = format switch
		{
			AL.FORMAT_MONO8 | AL.FORMAT_MONO16 => 1,
			AL.FORMAT_STEREO8 | AL.FORMAT_STEREO16 => 2,
			_ => throw new NotImplementedException()
		};

		var bits = format switch
		{
			AL.FORMAT_MONO8 | AL.FORMAT_STEREO8 => 8,
			AL.FORMAT_MONO16 | AL.FORMAT_STEREO16 => 16,
			_ => throw new NotImplementedException()
		};

		return (double)dataLength / (channels * (bits / 8) * frequency);
	}
}
