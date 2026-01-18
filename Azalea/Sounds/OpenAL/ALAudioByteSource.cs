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

		ALC.BindBuffer(_source.Handle, alSound.Buffer.Handle);

		CurrentInstance = new SoundByteInstance();

		ALC.SourcePlay(_source.Handle);

		return CurrentInstance;
	}
}
