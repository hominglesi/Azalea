using System;

namespace Azalea.Audio;
public class AudioInstance
{
	internal AudioSource _source;
	internal Sound _sound;

	public bool Playing { get; internal set; }

	internal AudioInstance(AudioSource source, Sound sound)
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
