using Azalea.Native.OpenAL;

namespace Azalea.Platform.Audio.OpenAL;
internal class ALAudioByteSource
{
	internal readonly uint Handle;

	public ALAudioByteSource()
	{
		AL.GenSources(1, ref Handle);
	}
}
