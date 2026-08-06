using Azalea.Sounds.OpenAL;

namespace Azalea.Platform.Audio.OpenAL;
internal class ALAudio : PlatformAudio
{
	private readonly ALAudioSource[] _audioSources = new ALAudioSource[AudioChannelCount];
	private readonly ALAudioByteSource[] _audioByteSources = new ALAudioByteSource[AudioByteChannelCount];
	private readonly ALAudioByteSource[] _audioByteSourcesInternal = new ALAudioByteSource[AudioByteChannelInternalCount];

	public ALAudio()
	{

	}

	protected override void HandleCommandLogic(AudioCommand command)
	{
		switch (command)
		{
			case CreateSoundByteCommand()
		}
	}
}
