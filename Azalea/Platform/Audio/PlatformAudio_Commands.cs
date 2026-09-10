using Azalea.Threading;
using System.Diagnostics;

namespace Azalea.Platform.Audio;
public abstract class AudioCommand : ThreadCommand
{
	internal static volatile new int TotalCreated = 0;

	internal AudioCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class CreateSoundByteCommand : AudioCommand
{
	public SoundByte SoundByte;
	public byte[] Data;
	public int DataLength;
	public int Format;
	public int Frequency;
}

internal static class CreateSoundByteCommand_Handler
{
	internal static SoundByte CreateSoundByte(this ICommandHandler<AudioCommand> handler, byte[] data, int dataLength, int format, int frequency)
	{
		var duration = AudioUtils.CalculateDuration(data, dataLength, format, frequency);

		var soundByte = new SoundByte(duration);
		handler.Enqueue(CreateSoundByteCommand.Borrow(soundByte, data, dataLength, format, frequency));
		return soundByte;
	}
}

[ThreadCommand(awaitable: true)]
internal partial class InitializeCommand : AudioCommand, ICommandAwaitable { }

[ThreadCommand(generateHandler: false)]
internal partial class PlayByteCommand : AudioCommand
{
	public AudioByteInstance Instance;
	public SoundByte SoundByte;
	public float Gain;
	public bool Looping;
}

internal static class PlayByteCommand_Handler
{
	internal static IAudioInstance PlayByte(this ICommandHandler<AudioCommand> handler, SoundByte soundByte, float gain, bool looping)
	{
		Debug.Assert(handler is PlatformAudio);

		var instance = new AudioByteInstance((PlatformAudio)handler, soundByte.Duration);
		handler.Enqueue(PlayByteCommand.Borrow(instance, soundByte, gain, looping));
		return instance;
	}
}
