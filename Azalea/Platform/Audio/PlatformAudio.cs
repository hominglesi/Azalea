using Azalea.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Azalea.Platform.Audio;

public abstract partial class PlatformAudio : ICommandHandler<AudioCommand>
{
	internal Action<IAudioInstance>? InstanceStarted;
	protected List<IAudioInstance> ActiveInstances = [];

	protected PlatformAudio()
	{
		Thread = new AudioThread(this);
		Thread.Start();
	}

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
		protected override void Initialize() { }

		protected override void Update()
		{
			_audio.UpdateLogic();
		}

		protected override void HandleCommand(AudioCommand command)
			 => _audio.HandleCommandLogic(command);
	}

	#endregion
}
