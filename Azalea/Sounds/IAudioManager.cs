using Azalea.Sounds.OpenAL;
using System;

namespace Azalea.Sounds;
public interface IAudioManager
{
	public float MasterVolume { get; set; }
	public IAudioSource[] AudioChannels { get; }
	public IAudioSource[] AudioByteChannels { get; }
	public IAudioSource[] AudioByteInternalChannels { get; }

	public IAudioInstance Play(Sound sound, float gain = 1, bool looping = false);
	public IAudioInstance PlayByte(SoundByte soundByte, float gain = 1, bool looping = false);
	internal IAudioInstance PlayByteInternal(SoundByte soundByte, float gain = 1, bool looping = false);
	internal SoundByte CreateSoundByte(ReadOnlySpan<byte> data, ALFormat format, int frequency);

	internal void Update();
}
