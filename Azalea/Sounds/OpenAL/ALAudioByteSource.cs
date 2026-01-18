namespace Azalea.Sounds.OpenAL;
internal class ALAudioByteSource
{
	private readonly ALSource _source;

	public IAudioInstance? CurrentInstance { get; private set; }

	public ALAudioByteSource()
	{
		_source = new ALSource();
	}

	public IAudioInstance? Play(SoundByte soundByte, float gain, bool looping)
	{
		if (soundByte is not ALSound alSound)
			return null;

		// TODO: Stop playing

		_source.BindBuffer(alSound.Buffer);

		CurrentInstance = new SoundByteInstance();

		_source.Play();

		return CurrentInstance;
	}
}
