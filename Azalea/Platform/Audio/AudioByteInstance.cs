using Azalea.Platform.Audio.OpenAL;
using System;

namespace Azalea.Platform.Audio;
internal class AudioByteInstance(PlatformAudio owner, double duration) : IAudioInstance
{
	public PlatformAudio Owner { get; } = owner;
	public double Duration { get; } = duration;

	internal ALByteSource? Source;

	public AudioInstanceState State = AudioInstanceState.Stopped;

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
