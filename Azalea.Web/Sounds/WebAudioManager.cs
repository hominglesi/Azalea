using Azalea.Sounds;
using System.Diagnostics;

namespace Azalea.Web.Sounds;
internal class WebAudioManager : AudioManager
{
	public override Sound CreateSound(ISoundData data)
		=> new WebSound(data);

	public override AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
	{
		Debug.Assert(sound is not null);

		var source = new WebAudioSource();
		return source.Play(sound, gain, looping);
	}

	public override AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
		=> Play(sound, gain, looping);

	public override AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false)
		=> Play(sound, gain, looping);

	protected override void SetMasterVolumeImplementation(float volume)
		=> WebAudio.SetMasterVolume(volume);

	protected override void OnDispose() { }
}
