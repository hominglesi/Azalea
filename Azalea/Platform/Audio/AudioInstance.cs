using Azalea.Platform.Audio.OpenAL;
using System;

namespace Azalea.Platform.Audio;

internal partial class AudioInstance(PlatformAudio owner, bool looping, float gain) : IAudioInstance
{
	internal ALSource? Source;

	public PlatformAudio Owner { get; } = owner;
	public bool Looping { get; } = looping;
	public double Duration { get; internal set; }
	public AudioInstanceState State { get; internal set; } = AudioInstanceState.Stopped;
	public float Gain { get; internal set; } = gain;
	public float Timestamp { get; internal set; }

	private bool _stopped = false;
	public event Action? Stopped;
	internal void InvokeStopped()
	{
		if (_stopped)
			throw new Exception("Tried to stop an already stopped instance.");

		Stopped?.Invoke();
		_stopped = true;
	}
}
