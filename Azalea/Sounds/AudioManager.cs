using Azalea.Sounds.OpenAL;
using Azalea.Utils;
using System;

namespace Azalea.Sounds;
internal abstract class AudioManager : Disposable, IAudioManager
{
	private float _masterVolume = 1.0f;
	protected abstract void SetMasterVolumeImplementation(float volume);
	public float MasterVolume
	{
		get => _masterVolume;
		set
		{
			if (_masterVolume == value) return;

			SetMasterVolumeImplementation(value);

			_masterVolume = value;
		}
	}

	public abstract IAudioSource[] AudioChannels { get; }
	public abstract IAudioSource[] AudioByteChannels { get; }
	public abstract IAudioSource[] AudioByteInternalChannels { get; }

	public abstract IAudioInstance PlayAudio(Sound sound, float gain = 1, bool looping = false);
	public abstract IAudioInstance PlayAudioByte(SoundByte soundByte, float gain = 1, bool looping = false);
	public abstract IAudioInstance PlayAudioByteInternal(SoundByte soundByte, float gain = 1, bool looping = false);
	public abstract SoundByte CreateSoundByte(ReadOnlySpan<byte> data, ALFormat format, int frequency);

	public virtual void Update() { }
}
