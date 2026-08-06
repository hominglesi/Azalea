using Azalea.Sounds;
using Azalea.Threading;
using Azalea.Utils;

namespace Azalea.Platform.Audio;
public abstract class PlatformAudio : ICommandHandler<AudioCommand>
{
	public PlatformAudio()
	{
		MasterVolume = new(1.0f);

		Thread = new AudioThread();
		Thread.Start();
	}

	// All audio channels should total up to 32
	internal abstract IAudioSource[] AudioChannels { get; }
	protected const int AudioChannelCount = 4;
	internal abstract IAudioSource[] AudioByteChannels { get; }
	protected const int AudioByteChannelCount = 24;
	internal abstract IAudioSource[] AudioByteInternalChannels { get; }
	protected const int AudioByteChannelInternalCount = 4;

	public ReadOnlyObservable<float> MasterVolume { get; }

	public ICommandAwaitable? Enqueue(AudioCommand command) => Thread.Enqueue(command);

	#region AudioThread

	internal readonly AudioThread Thread;

	internal class AudioThread() : GameThread<AudioCommand>(1)
	{
		public override string DisplayName => "Audio Thread";

		protected override void Initialize()
		{
			throw new System.NotImplementedException();
		}

		protected override void Update()
		{
			throw new System.NotImplementedException();
		}
	}

	#endregion
}
