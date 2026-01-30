using Azalea.Sounds.OpenAL.Enums;
using System;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioByteSource : IAudioSource
{
	private readonly ALSource _source = new();

	public IAudioInstance? CurrentInstance { get; private set; }
	public float CurrentTimestamp { get; private set; }

	private AudioSourceState _state = AudioSourceState.Stopped;
	public AudioSourceState State
	{
		get => _state;
		private set
		{
			if (_state == value)
				return;

			_state = value;
			invokeStateUpdated();
		}
	}
	public Action<AudioSourceState>? StateUpdated { get; set; }
	private void invokeStateUpdated() => StateUpdated?.Invoke(State);

	public IAudioInstance? Play(SoundByte soundByte, float gain, bool looping)
	{
		if (soundByte is not ALSound alSound)
			return null;

		if (State == AudioSourceState.Playing || State == AudioSourceState.Paused)
			Stop();

		_source.BindBuffer(alSound.Buffer);

		CurrentInstance = new AudioInstance(this, alSound.Duration);

		Volume = gain;
		Looping = looping;
		_source.Play();

		State = AudioSourceState.Playing;
		return CurrentInstance;
	}

	public float Volume
	{
		get => _source.Gain;
		set => _source.Gain = value;
	}

	public float Pitch
	{
		get => _source.Pitch;
		set => _source.Pitch = value;
	}

	public bool Looping
	{
		get => _source.Looping;
		set => _source.Looping = value;
	}

	public void Pause()
	{
		if (State != AudioSourceState.Playing)
			return;

		_source.Pause();
		State = AudioSourceState.Paused;
	}

	public void Unpause()
	{
		if (State != AudioSourceState.Paused)
			return;

		_source.Play();
		State = AudioSourceState.Playing;
	}

	public void Stop()
	{
		if (State != AudioSourceState.Playing && State != AudioSourceState.Paused)
			return;

		_source.Stop();
		CurrentInstance = null;

		State = AudioSourceState.Stopped;
	}

	public void Seek(float timestamp)
	{
		if (State == AudioSourceState.Stopped)
			return;

		_source.Stop();

		ALC.SetSecOffset(_source.Handle, timestamp);
		CurrentTimestamp = Math.Min(timestamp, CurrentInstance!.TotalDuration);

		if (State == AudioSourceState.Playing)
			_source.Play();
	}

	internal void Update()
	{
		if (CurrentInstance is null || State != AudioSourceState.Playing)
			return;

		var state = _source.GetState();

		if (State == AudioSourceState.Playing && state == ALSourceState.Stopped)
			State = AudioSourceState.Paused;

		if (State == AudioSourceState.Playing)
			CurrentTimestamp = _source.GetSecOffset();
	}
}
