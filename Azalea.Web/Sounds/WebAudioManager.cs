using Azalea.Sounds;
using System.Diagnostics;

namespace Azalea.Web.Sounds;
internal class WebAudioManager : AudioManager
{
	public override SoundByte CreateSoundByte(ISoundData data)
		=> new WebSound(data);

	public override AudioInstanceLegacyAudio PlayLegacyAudio(SoundByte sound, float gain = 1, bool looping = false)
	{
		Debug.Assert(sound is not null);

		var source = new WebAudioSource();
		return source.Play(sound, gain, looping);
	}

	public override AudioInstanceLegacyAudio PlayVitalLegacyAudio(SoundByte sound, float gain = 1, bool looping = false)
		=> PlayLegacyAudio(sound, gain, looping);

	public override AudioInstanceLegacyAudio PlayInternalLegacyAudio(SoundByte sound, float gain = 1, bool looping = false)
		=> PlayLegacyAudio(sound, gain, looping);

	protected override void SetMasterVolumeImplementation(float volume)
		=> WebAudio.SetMasterVolume(volume);

	protected override void OnDispose() { }
}
