using Azalea.Platform.Audio.OpenAL;
using Azalea.Utils;
using System;

namespace Azalea.Platform.Audio;
internal partial class AudioByteInstance(PlatformAudio owner, bool looping, float gain, double duration) : IAudioInstance
{
	internal ALByteSource? Source;

	public PlatformAudio Owner { get; } = owner;
	public bool Looping { get; } = looping;
	public double Duration { get; } = duration;
	public ReadOnlyObservable<AudioInstanceState> State { get; } = new(AudioInstanceState.Stopped);

	[ObservableProperty] public partial float Gain { get; internal set; } = gain;
	[ObservableProperty] public partial float Timestamp { get; internal set; }

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
