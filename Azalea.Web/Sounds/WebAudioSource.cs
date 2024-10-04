using Azalea.Sounds;

namespace Azalea.Web.Sounds;
internal class WebAudioSource : AudioSource
{
	public object Handle;
	private object _gainNode;

	public WebAudioSource()
	{
		Handle = WebAudio.CreateBufferSource();
		_gainNode = WebAudio.CreateGain();
		WebAudio.Connect(Handle, _gainNode);
		WebAudio.ConnectToContext(_gainNode);
	}

	protected override void BindBufferImplementation(Sound sound)
		=> WebAudio.SetBuffer(Handle, ((WebSound)sound).Buffer.Handle);

	protected override void OnDispose()
	{

	}

	protected override void PlayImplementation()
		=> WebAudio.StartSource(Handle);

	protected override void SetGainImplementation(float gain)
		=> WebAudio.SetGain(_gainNode, gain);

	protected override void SetLoopingImplementation(bool looping)
		=> WebAudio.SetLoop(Handle, looping);

	protected override void StopImplementation()
		=> WebAudio.StopSource(Handle);
}
