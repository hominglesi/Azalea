using Azalea.Native.OpenAL;
using Azalea.Sounds;
using Azalea.Sounds.FFmpeg;
using System;
using System.Buffers;
using System.IO;

namespace Azalea.Platform.Audio.OpenAL;
internal class ALAudioSource : IAudioSource
{
	internal readonly uint Handle;

	public AudioSourceState State { get; private set; } = AudioSourceState.Stopped;
	public Action<AudioSourceState>? StateUpdated { get; set; }
	public IAudioInstance? CurrentInstance { get; private set; }
	public float CurrentTimestamp { get; }

	private const int __bufferCount = 8;
	private readonly uint[] _buffers = new uint[__bufferCount];
	private readonly float[] _bufferStartTimes = new float[__bufferCount];
	private int _currentBufferStartTime;
	private int _nextBufferStartTime;

	private Stream? _currentStream;
	private FFmpegStreamReader? _currentReader;

	internal ALAudioSource()
	{
		AL.GenSources(1, ref Handle);
		AL.Source3f(Handle, AL.POSITION, 0, 0, 0);
		AL.Source3f(Handle, AL.VELOCITY, 0, 0, 0);
		AL.Sourcei(Handle, AL.SOURCE_RELATIVE, 0);

		AL.GenBuffers(__bufferCount, ref _buffers[0]);
	}

	internal IAudioInstance Play(Sound sound, float gain, bool looping)
	{
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
				AL.BufferData(_buffers[i], AL.FORMAT_STEREO16, ref pcm[0], pcmLength, sampleRate);
				ArrayPool<byte>.Shared.Return(pcm);
				AL.SourceQueueBuffers(Handle, 1, ref _buffers[i]);
				_bufferStartTimes[_nextBufferStartTime] = startTime;
				_nextBufferStartTime = (_nextBufferStartTime + 1) % __bufferCount;
			}
		}

		AL.Sourcef(Handle, AL.GAIN, gain);

		AL.SourcePlay(Handle);
		State = AudioSourceState.Playing;
		return CurrentInstance;
	}

	public void Stop()
	{

	}

	public float Volume { get; set; }
	public float Pitch { get; set; }
	public bool Looping { get; set; }

	public void Pause() { }
	public void Unpause() { }
	public void Seek(float timestamp) { }
}
