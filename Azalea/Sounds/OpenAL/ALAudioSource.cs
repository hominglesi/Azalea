using Azalea.Sounds.FFmpeg;
using Azalea.Sounds.OpenAL.Enums;
using System;
using System.Diagnostics;
using System.IO;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioSource : IAudioSource
{
	private const int __bufferCount = 8;
	private readonly ALSource _source;
	private readonly ALBuffer[] _buffers;

	public IAudioInstance? CurrentInstance { get; private set; }

	private Stream? _currentStream;
	private FFmpegStreamReader? _currentReader;

	public ALAudioSource()
	{
		_source = new ALSource();

		_buffers = new ALBuffer[__bufferCount];
		for (int i = 0; i < _buffers.Length; i++)
			_buffers[i] = new ALBuffer();
	}

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

	public IAudioInstance? Play(Sound sound, float gain, bool looping)
	{
		if (State == AudioSourceState.Playing || State == AudioSourceState.Paused)
			Stop();

		var stream = sound.GetStream();
		if (stream is null)
			return null;

		_currentStream = stream;
		_currentReader = new FFmpegStreamReader(_currentStream);
		CurrentInstance = new SoundInstance();

		for (int i = 0; i < __bufferCount; i++)
		{
			_currentReader.ReadChunk(out var pcm, out var sampleRate);
			ALC.BufferData(_buffers[i].Handle, ALFormat.Stereo16, pcm, pcm.Length, sampleRate);
			ALC.SourceQueueBuffer(_source.Handle, _buffers[i].Handle);
		}

		Volume = gain;
		ALC.SourcePlay(_source.Handle);

		State = AudioSourceState.Playing;
		return CurrentInstance;
	}

	private float _volume = 1.0f;
	public float Volume
	{
		get => _volume;
		set
		{
			if (_volume == value)
				return;

			ALC.SetSourceGain(_source.Handle, value);

			_volume = value;
			invokeStateUpdated();
		}
	}

	public void Pause()
	{
		if (State != AudioSourceState.Playing)
			return;

		ALC.SourcePause(_source.Handle);
		State = AudioSourceState.Paused;
	}

	public void Unpause()
	{
		if (State != AudioSourceState.Paused)
			return;

		ALC.SourcePlay(_source.Handle);
		State = AudioSourceState.Playing;
	}

	public void Stop()
	{
		if (State != AudioSourceState.Playing && State != AudioSourceState.Paused)
			return;

		ALC.SourceStop(_source.Handle);

		var queued = ALC.GetBuffersProcessed(_source.Handle);
		while (queued-- > 0)
			ALC.SourceUnqueueBuffer(_source.Handle);

		_currentReader?.Dispose();
		//_currentStream?.Dispose();
		CurrentInstance = null;

		State = AudioSourceState.Stopped;
	}

	public void Update()
	{
		if (CurrentInstance is null || State != AudioSourceState.Playing)
			return;

		Debug.Assert(_currentReader is not null);

		int processed = ALC.GetBuffersProcessed(_source.Handle);
		while (processed-- > 0)
		{
			var buffer = ALC.SourceUnqueueBuffer(_source.Handle);

			_currentReader.ReadChunk(out var pcm, out var sampleRate);

			ALC.BufferData(buffer, ALFormat.Stereo16, pcm, pcm.Length, sampleRate);
			ALC.SourceQueueBuffer(_source.Handle, buffer);
		}

		if (ALC.GetSourceState(_source.Handle) != ALSourceState.Playing)
			ALC.SourcePlay(_source.Handle);
	}
}
