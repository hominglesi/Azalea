using Azalea.Sounds.FFmpeg;
using Azalea.Sounds.OpenAL.Enums;
using Azalea.Threading;
using System;
using System.Diagnostics;
using System.IO;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioSource : IAudioSource
{
	private readonly ALAudioManager _audioManager;

	private const int __bufferCount = 8;
	private readonly ALSource _source;
	private readonly ALBuffer[] _buffers = new ALBuffer[__bufferCount];
	private readonly float[] _bufferStartTimes = new float[__bufferCount];
	private int _currentBufferStartTime;
	private int _nextBufferStartTime;

	public IAudioInstance? CurrentInstance { get; private set; }
	public float CurrentTimestamp => _bufferStartTimes[_currentBufferStartTime] + _sourceOffset;
	private float _sourceOffset;

	private Stream? _currentStream;
	private FFmpegStreamReader? _currentReader;

	public ALAudioSource(ALAudioManager audioManager)
	{
		_audioManager = audioManager;

		_source = new(_audioManager);

		for (int i = 0; i < _buffers.Length; i++)
			_buffers[i] = new ALBuffer(_audioManager);
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
			if (_currentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
			{
				_buffers[i].BufferAndFreeData(pcm, pcmLength, ALFormat.Stereo16, sampleRate);
				_source.QueueBuffer(_buffers[i]);
				_bufferStartTimes[_nextBufferStartTime] = startTime;
				_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
			}
		}

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

	public bool Looping { get; set; }

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
		_source.UnqueueAllBuffers();

		_currentReader?.Dispose();
		_currentStream?.Dispose();
		CurrentInstance = null;

		resetStartTimes();
		for (int i = 0; i < _bufferStartTimes.Length; i++)
			_bufferStartTimes[i] = 0;

		State = AudioSourceState.Stopped;
	}

	private AtomicCounter _seekCounter = new();

	public void Seek(float timestamp)
	{
		if (State == AudioSourceState.Stopped || _currentReader is null)
			return;

		_seekCounter.Increment();
		bool shouldPlay = State == AudioSourceState.Playing;

		_audioManager.BeginCommandQueue();

		_source.Stop();
		_source.UnqueueAllBuffers();
		_sourceOffset = 0;

		resetStartTimes();

		lock (_currentReader)
		{
			_currentReader!.Seek(timestamp);
			for (int i = 0; i < __bufferCount; i++)
			{
				if (_currentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
				{
					_buffers[i].BufferAndFreeData(pcm, pcmLength, ALFormat.Stereo16, sampleRate);
					_source.QueueBuffer(_buffers[i]);
					_bufferStartTimes[_nextBufferStartTime] = startTime;
					_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
				}
			}
		}

		if (shouldPlay)
			_source.Play();

		_audioManager.DecrementCounter(_seekCounter);
		_audioManager.SubmitCommandQueue();
	}

	internal void Update()
	{
		if (CurrentInstance is null || State != AudioSourceState.Playing)
			return;

		Debug.Assert(_currentReader is not null);

		lock (_currentReader)
		{
			int processed = _source.GetBuffersProcessed();
			while (processed-- > 0)
			{
				var buffer = _source.UnqueueBuffer();
				_currentBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;

				if (_currentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
				{
					_audioManager.BufferAndFreeData(buffer, pcm, pcmLength, ALFormat.Stereo16, sampleRate);
					_source.QueueBuffer(buffer);
					_bufferStartTimes[_nextBufferStartTime] = startTime;
				}

				_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
			}

			if (_source.GetState() != ALSourceState.Playing)
				_source.Play();

			if (_seekCounter.IsActive == false && _source.GetState() == ALSourceState.Stopped)
			{
				if (Looping == false)
					State = AudioSourceState.Paused;
				else
				{
					_currentReader!.Seek(0);
					for (int i = 0; i < __bufferCount; i++)
					{
						if (_currentReader.ReadChunk(out var pcm, out var pcmLength, out var sampleRate, out var startTime))
						{
							_buffers[i].BufferAndFreeData(pcm, pcmLength, ALFormat.Stereo16, sampleRate);
							_source.QueueBuffer(_buffers[i]);
							_bufferStartTimes[_nextBufferStartTime] = startTime;
							_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
						}
					}
				}
			}
		}

		_sourceOffset = _source.GetSecOffset();
	}
}
