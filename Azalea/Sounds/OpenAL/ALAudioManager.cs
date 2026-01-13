using Azalea.Sounds.OpenAL.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using static Azalea.Sounds.Audio;

namespace Azalea.Sounds.OpenAL;
internal class ALAudioManager : AudioManager
{
	protected const int SourceCount = 32;

	private readonly ALC_Device _device;
	private readonly ALC_Context _context;
	private readonly ALSource[] _sources;

	public ALAudioManager()
	{
		_device = ALC.OpenDevice();
		_context = ALC.CreateContext(_device);
		ALC.MakeContextCurrent(_context);

		_sources = new ALSource[SourceCount];
		for (int i = 0; i < SourceCount; i++)
		{
			_sources[i] = new ALSource();
		}
	}

	public override Sound CreateSound(ISoundData data)
		=> new ALSound(data);
	protected override void SetMasterVolumeImplementation(float volume)
		=> ALC.SetListenerGain(volume);

	private AudioInstance playOnChannel(int channel, Sound sound, float gain, bool looping)
	{
		Debug.Assert(sound is not null);

		return _sources[channel].Play(sound, gain, looping);
	}

	public override AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false)
		=> playOnChannel(SourceCount - 1, sound, gain, looping);

	private const int _vitalChannels = 4;
	private int _currentVitalChannel = 0;
	public override AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
	{
		var played = playOnChannel(_currentVitalChannel, sound, gain, looping);

		_currentVitalChannel += 1;
		if (_currentVitalChannel >= _vitalChannels)
			_currentVitalChannel = 0;

		return played;
	}

	private const int _audioChannels = SourceCount - _vitalChannels - 1;
	private int _currentAudioChannel = _vitalChannels;
	public override AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		var played = playOnChannel(_currentAudioChannel, sound, gain, looping);

		_currentAudioChannel += 1;
		if (_currentAudioChannel >= _audioChannels)
			_currentAudioChannel = _vitalChannels;

		return played;
	}

	private readonly List<StreamInstance> _streamInstances = [];
	public void Stream(FFmpegStreamReader reader)
	{
		var instance = new StreamInstance(_sources[25], reader);
		_streamInstances.Add(instance);
	}

	class StreamInstance
	{
		private const int __bufferCount = 8;

		private ALSource _source;
		private FFmpegStreamReader _reader;
		private ALBuffer[] _buffers = new ALBuffer[__bufferCount];

		public StreamInstance(ALSource source, FFmpegStreamReader reader)
		{
			_source = source;
			_reader = reader;

			for (int i = 0; i < __bufferCount; i++)
				_buffers[i] = new ALBuffer();

			for (int i = 0; i < __bufferCount; i++)
			{
				reader.ReadChunk(out var pcm, out var sampleRate);
				ALC.BufferData(_buffers[i].Handle, ALFormat.Stereo16, pcm, pcm.Length, sampleRate);
				ALC.SourceQueueBuffer(_source.Handle, _buffers[i].Handle);
			}

			ALC.SourcePlay(_source.Handle);
		}

		public void Update()
		{
			int processed = ALC.GetBuffersProcessed(_source.Handle);
			while (processed-- > 0)
			{
				var buffer = ALC.SourceUnqueueBuffer(_source.Handle);

				_reader.ReadChunk(out var pcm, out var sampleRate);

				ALC.BufferData(buffer, ALFormat.Stereo16, pcm, pcm.Length, sampleRate);
				ALC.SourceQueueBuffer(_source.Handle, buffer);
			}

			if (ALC.GetSourceState(_source.Handle) != ALSourceState.Playing)
				ALC.SourcePlay(_source.Handle);
		}
	}

	public override void Update()
	{
		foreach (var source in _sources)
			source.Update();

		foreach (var streamInstance in _streamInstances)
			streamInstance.Update();
	}

	protected override void OnDispose()
	{
		ALC.CloseDevice(_device);
	}
}
