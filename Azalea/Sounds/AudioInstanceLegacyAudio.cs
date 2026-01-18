using System;

namespace Azalea.Sounds;
public class AudioInstanceLegacyAudio
{
	internal AudioSource _source;
	internal SoundByte _sound;

	public bool Playing { get; internal set; }

	internal AudioInstanceLegacyAudio(AudioSource source, SoundByte sound)
	{
		_source = source;
		_sound = sound;
	}

	public float Gain
	{
		get => checkIfPlaying() ? _source.Gain : 0;
		set { if (checkIfPlaying()) _source.Gain = value; }
	}

	public void Stop()
	{
		if (checkIfPlaying())
			_source.Stop();
	}

	private bool checkIfPlaying()
	{
		if (Playing == false)
			Console.WriteLine("Tried to access an AudioInstance that has finished playing");

		return Playing;
	}
}
