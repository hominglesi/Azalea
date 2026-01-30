using System;

namespace Azalea.Sounds.OpenAL;
internal class ALSound : SoundByte
{
	internal ALBuffer Buffer;

	public float Duration { get; private init; }

	public ALSound(ReadOnlySpan<byte> data, ALFormat format, int frequency)
	{
		Buffer = new ALBuffer();
		Buffer.SetData(data, format, frequency);

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
