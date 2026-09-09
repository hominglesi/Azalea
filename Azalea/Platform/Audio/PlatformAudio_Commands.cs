using Azalea.Sounds;
using Azalea.Threading;
using System.Buffers;

namespace Azalea.Platform.Audio;
public abstract class AudioCommand : ThreadCommand
{
	internal static volatile new int TotalCreated = 0;

	internal AudioCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand]
internal partial class BindSourceBufferCommand : AudioCommand
{
	public uint Source;
	public uint Buffer;
}

[ThreadCommand]
internal partial class BufferDataCommand : AudioCommand
{
	public uint Buffer;
	public byte[] Data;
	public int DataLength;
	public int Format;
	public int Frequency;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData)
			ArrayPool<byte>.Shared.Return(Data);
	}
}

[ThreadCommand]
internal partial class CloseDeviceCommand : AudioCommand
{
	public nint Device;
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
		var soundByte = new SoundByte();
		handler.Enqueue(CreateSoundByteCommand.Borrow(soundByte, data, dataLength, format, frequency));
		return soundByte;
	}
}

[ThreadCommand(awaitable: true)]
internal partial class InitializeCommand : AudioCommand, ICommandAwaitable { }

[ThreadCommand]
internal partial class PlayCommand : AudioCommand
{
	public Sound Sound;
	public float Gain;
	public bool Looping;
}

[ThreadCommand]
internal partial class PlayByteCommand : AudioCommand
{
	public SoundByte SoundByte;
	public float Gain;
	public bool Looping;
}

[ThreadCommand]
internal partial class PlayByteInternalCommand : AudioCommand
{
	public SoundByte SoundByte;
	public float Gain;
	public bool Looping;
}

[ThreadCommand]
internal partial class SetMasterVolumeCommand : AudioCommand
{
	public float Volume;
}
