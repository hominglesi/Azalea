using System;

namespace Azalea.Sounds.OpenAL;
internal class ALSound : SoundByte
{
	private readonly ALAudioManager _audioManager;

	public ALBuffer Buffer { get; }
	public float Duration { get; }

	public ALSound(ALAudioManager audioManager, byte[] data, int dataLength, ALFormat format, int frequency)
	{
		_audioManager = audioManager;
		Buffer = new ALBuffer(audioManager);
		Buffer.BufferData(data, dataLength, format, frequency);

		Duration = getDuration(data.Length, format, frequency);
	}

	private float getDuration(float bufferSize, ALFormat format, int frequency)
	{
		var channels = format switch
		{
			ALFormat.Mono8 => 1,
			ALFormat.Mono16 => 1,
			ALFormat.Stereo8 => 2,
			ALFormat.Stereo16 => 2,
			_ => throw new NotImplementedException()
		};

		var bits = format switch
		{
			ALFormat.Mono8 => 8,
			ALFormat.Mono16 => 16,
			ALFormat.Stereo8 => 8,
			ALFormat.Stereo16 => 16,
			_ => throw new NotImplementedException()
		};

		return bufferSize / (channels * (bits / 8) * frequency);
	}

	protected override void OnDispose()
		=> Buffer.Dispose();
}
