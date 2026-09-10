using Azalea.Platform.Audio.OpenAL;
using Azalea.Platform.Audio.Windows;
using Azalea.Threading;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Platform.Audio;
public abstract class PlatformAudio : ICommandHandler<AudioCommand>
{
	internal Action<IAudioInstance>? InstanceStarted;
	protected List<IAudioInstance> ActiveInstances = [];

	protected PlatformAudio()
	{
		MasterVolume = new(1.0f);

		Thread = new AudioThread(this);
		Thread.Start();
	}

	public ReadOnlyObservable<float> MasterVolume { get; }

	protected abstract void HandleCommandLogic(AudioCommand command);
	protected abstract void UpdateLogic();
	public ICommandAwaitable? Enqueue(AudioCommand command) => Thread.Enqueue(command);

	#region AudioThread

	internal readonly AudioThread Thread;

	internal class AudioThread(PlatformAudio audio) : GameThread<AudioCommand>(1)
	{
		private readonly PlatformAudio _audio = audio;

		public override string DisplayName => "Audio Thread";

		public ManualResetEvent InitializedEvent = new(false);
		protected override void Initialize()
		{

		}

		protected override void Update()
			=> _audio.UpdateLogic();

		protected override void HandleCommand(AudioCommand command)
			 => _audio.HandleCommandLogic(command);
	}

	#endregion

	public static PlatformAudio Create()
	{
		var newAudio = RuntimeInformation.ProcessArchitecture switch
		{
			Architecture.X64 or Architecture.X86 => new ALAudio(new WindowsAudioDeviceNotificationClient()),
			_ => throw new NotSupportedException(
				$"Platform '{RuntimeInformation.ProcessArchitecture}' is not supported")
		};

		return newAudio;
	}
}
