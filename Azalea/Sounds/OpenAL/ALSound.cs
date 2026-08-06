using Azalea.Native.OpenAL;
using System;

namespace Azalea.Sounds.OpenAL;
internal class ALSound : SoundByte
{
	private readonly ALAudioManager _audioManager;

	public ALBuffer Buffer { get; }
	public float Duration { get; }

	public ALSound(ALAudioManager audioManager, byte[] data, int dataLength, int format, int frequency)
	{
		_audioManager = audioManager;
		Buffer = new ALBuffer(audioManager);
		Buffer.BufferData(data, dataLength, format, frequency);

		Duration = getDuration(data.Length, format, frequency);
	}

	private float getDuration(float bufferSize, int format, int frequency)
	{
		var channels = format switch
		{
			AL.FORMAT_MONO8 => 1,
			AL.FORMAT_MONO16 => 1,
			AL.FORMAT_STEREO8 => 2,
			AL.FORMAT_STEREO16 => 2,
			_ => throw new NotImplementedException()
		};

		var bits = format switch
		{
			AL.FORMAT_MONO8 => 8,
			AL.FORMAT_MONO16 => 16,
			AL.FORMAT_STEREO8 => 8,
			AL.FORMAT_STEREO16 => 16,
			_ => throw new NotImplementedException()
		};

		return bufferSize / (channels * (bits / 8) * frequency);
	}

	protected override void OnDispose()
		=> Buffer.Dispose();
}
