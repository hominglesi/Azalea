using Azalea.Sounds;
using Azalea.Sounds.OpenAL;
using Azalea.Threading;

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
	public ALFormat Format;
	public int Frequency;
}

internal static class CreateSoundByteCommand_Handler
{
	internal static SoundByte CreateSoundByte(this ICommandHandler<AudioCommand> handler, byte[] data, int dataLength, ALFormat format, int frequency)
	{
		var soundByte = new SoundByte();
		handler.Enqueue(CreateSoundByteCommand.Borrow(soundByte, data, dataLength, format, frequency));
		return soundByte;
	}
}

[ThreadCommand]
internal partial class PlayCommand : AudioCommand
{
	public Sound Sound;
	public float Gain;
	public float Looping;
}

[ThreadCommand]
internal partial class PlayByteCommand : AudioCommand
{
	public SoundByte SoundByte;
	public float Gain;
	public float Looping;
}

[ThreadCommand]
internal partial class PlayByteInternalCommand : AudioCommand
{
	public SoundByte SoundByte;
	public float Gain;
	public float Looping;
}
