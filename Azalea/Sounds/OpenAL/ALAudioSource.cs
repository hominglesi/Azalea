using Azalea.Sounds.FFmpeg;
using Azalea.Sounds.OpenAL.Enums;
using System;
using System.Diagnostics;
using System.IO;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioSource : IAudioSource
{
	private const int __bufferCount = 8;
	private readonly ALSource _source = new();
	private readonly ALBuffer[] _buffers = new ALBuffer[__bufferCount];
	private readonly float[] _bufferStartTimes = new float[__bufferCount];
	private int _currentBufferStartTime;
	private int _nextBufferStartTime;

	public IAudioInstance? CurrentInstance { get; private set; }
	public float CurrentTimestamp => _bufferStartTimes[_currentBufferStartTime];

	private Stream? _currentStream;
	private FFmpegStreamReader? _currentReader;

	public ALAudioSource()
	{
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

	private void resetStartTimes()
	{
		_nextBufferStartTime = 0;
		_currentBufferStartTime = 0;
	}

	public IAudioInstance? Play(Sound sound, float gain, bool looping)
	{
		if (State == AudioSourceState.Playing || State == AudioSourceState.Paused)
			Stop();

		var stream = sound.GetStream();
		if (stream is null)
			return null;

		_currentStream = stream;
		_currentReader = new FFmpegStreamReader(_currentStream);
		CurrentInstance = new AudioInstance(this, _currentReader.TotalDuration);

		for (int i = 0; i < __bufferCount; i++)
		{
			if (_currentReader.ReadChunk(out var pcm, out var sampleRate, out var startTime))
			{
				_buffers[i].SetData(pcm, ALFormat.Stereo16, sampleRate);
				_source.QueueBuffer(_buffers[i]);
				_bufferStartTimes[_nextBufferStartTime] = startTime;
				_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
			}
		}

		Volume = gain;
		_source.Play();

		State = AudioSourceState.Playing;
		return CurrentInstance;
	}
	public float Volume
	{
		get => _source.Gain;
		set => _source.Gain = value;
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

		State = AudioSourceState.Playing;
	}

	public void Stop()
	{
		if (State != AudioSourceState.Playing && State != AudioSourceState.Paused)
			return;

		_source.Stop();

		var queued = _source.GetBuffersProcessed();
		while (queued-- > 0)
			_source.UnqueueBuffer();

		_currentReader?.Dispose();
		_currentStream?.Dispose();
		CurrentInstance = null;

		resetStartTimes();
		for (int i = 0; i < _bufferStartTimes.Length; i++)
			_bufferStartTimes[i] = 0;

		State = AudioSourceState.Stopped;
	}

	public void Seek(float timestamp)
	{
		if (State == AudioSourceState.Stopped)
			return;

		_source.Stop();

		var queued = _source.GetBuffersProcessed();
		while (queued-- > 0)
			_source.UnqueueBuffer();
		resetStartTimes();

		_currentReader!.Seek(timestamp);
		for (int i = 0; i < __bufferCount; i++)
		{
			if (_currentReader.ReadChunk(out var pcm, out var sampleRate, out var startTime))
			{
				_buffers[i].SetData(pcm, ALFormat.Stereo16, sampleRate);
				_source.QueueBuffer(_buffers[i]);
				_bufferStartTimes[_nextBufferStartTime] = startTime;
				_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
			}
		}

		if (State == AudioSourceState.Playing)
			_source.Play();
	}

	internal void Update()
	{
		if (CurrentInstance is null || State != AudioSourceState.Playing)
			return;

		Debug.Assert(_currentReader is not null);

		int processed = _source.GetBuffersProcessed();
		while (processed-- > 0)
		{
			var buffer = _source.UnqueueBuffer();
			_currentBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;

			if (_currentReader.ReadChunk(out var pcm, out var sampleRate, out var startTime))
			{
				ALC.BufferData(buffer, ALFormat.Stereo16, pcm, pcm.Length, sampleRate);
				_source.QueueBuffer(buffer);
				_bufferStartTimes[_nextBufferStartTime] = startTime;
				_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
			}
		}

		if (ALC.GetSourceState(_source.Handle) != ALSourceState.Playing)
			ALC.SourcePlay(_source.Handle);

		if (State == AudioSourceState.Playing && _source.GetBuffersQueued() == 0)
			State = AudioSourceState.Paused;
	}
}
