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

public abstract partial class PlatformAudio
{
	[ThreadCommand]
	internal partial class SetMasterVolume : AudioCommand
	{
		public float Volume;
	}

	public float MasterVolume
	{
		get;
		set
		{
			if (field == value) return;

			field = value;
			Enqueue(SetMasterVolume.Borrow(value));
		}
	} = 1.0f;

	[ThreadCommand]
	internal partial class PlayCommand : AudioCommand
	{
		public AudioInstance Instance;
		public Sound SoundByte;
	}

	public IAudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		var instance = new AudioInstance(this, looping, gain);
		Enqueue(PlayCommand.Borrow(instance, sound));
		return instance;
	}

	[ThreadCommand]
	internal partial class PlayByteCommand : AudioCommand
	{
		public AudioByteInstance Instance;
		public SoundByte SoundByte;
	}

	public IAudioInstance PlayByte(SoundByte soundByte, float gain = 1, bool looping = false)
	{
		var instance = new AudioByteInstance(this, looping, gain, soundByte.Duration);
		Enqueue(PlayByteCommand.Borrow(instance, soundByte));
		return instance;
	}

	[ThreadCommand(awaitable: true)]
	internal partial class InitializeCommand : AudioCommand, ICommandAwaitable { }

	[ThreadCommand]
	internal partial class CreateSoundByteCommand : AudioCommand
	{
		public SoundByte SoundByte;
		public byte[] Data;
		public int DataLength;
		public int Format;
		public int Frequency;
	}

	internal SoundByte CreateSoundByte(byte[] data, int dataLength, int format, int frequency)
	{
		var duration = AudioUtils.CalculateDuration(dataLength, format, frequency);

		var soundByte = new SoundByte(duration);
		Enqueue(CreateSoundByteCommand.Borrow(soundByte, data, dataLength, format, frequency));
		return soundByte;
	}

	[ThreadCommand]
	internal partial class SetInstanceGainCommand : AudioCommand
	{
		public IAudioInstance Instance;
		public float Gain;
	}

	[ThreadCommand]
	internal partial class SetInstanceTimestampCommand : AudioCommand
	{
		public IAudioInstance Instance;
		public float Timestamp;
	}

	[ThreadCommand]
	internal partial class PauseInstanceCommand : AudioCommand
	{
		public IAudioInstance Instance;
	}

	[ThreadCommand]
	internal partial class UnpauseInstanceCommand : AudioCommand
	{
		public IAudioInstance Instance;
	}

	[ThreadCommand]
	internal partial class StopInstanceCommand : AudioCommand
	{
		public IAudioInstance Instance;
	}
}
