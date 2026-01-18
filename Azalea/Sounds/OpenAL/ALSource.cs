namespace Azalea.Sounds.OpenAL;
internal class ALSource : AudioSource
{
	public uint Handle;

	public ALSource()
	{
		Handle = ALC.GenSource();
	}

	protected override void SetGainImplementation(float gain)
		=> ALC.SetSourceGain(Handle, gain);

	protected override void SetLoopingImplementation(bool looping)
		=> ALC.SetSourceLooping(Handle, looping);

	protected override void PlayImplementation()
		=> ALC.SourcePlay(Handle);

	protected override void BindBufferImplementation(SoundByte sound)
		=> ALC.BindBuffer(Handle, ((ALSound)sound).Buffer.Handle);

	protected override void StopImplementation()
		=> ALC.SourceStop(Handle);

	protected override void OnDispose()
		=> ALC.DeleteSource(Handle);
}
