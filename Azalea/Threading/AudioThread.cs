using Azalea.Platform;
using Azalea.Sounds;
using Azalea.Sounds.OpenAL;
using System;
using System.Diagnostics;
using System.Threading;

namespace Azalea.Threading;
internal class AudioThread : GameThread
{
	private readonly GameHost _host;

	public IAudioManager AudioManager { get; private set; }

	private readonly SemaphoreSlim _readyGate = new(0, 1);

	public override string DisplayName => "Audio Thread";

	public AudioThread(GameHost host) : base(1)
	{
		_host = host;

		// We start the thread manually because it's necessary to create the audio manager
		base.Start();
		_readyGate.Wait();
		Debug.Assert(AudioManager is not null);
	}

	public override void Start()
		=> throw new Exception($"{nameof(AudioThread)} starts when constructed");

	protected override void Initialize()
	{
		AudioManager = _host.CreateAudioManager();

		_readyGate.Release();
	}

	protected override void Update()
	{
		AudioManager.HandleCommands();
		AudioManager.Update();

		((ALAudioManager)AudioManager).PrintErrors();
	}
}
