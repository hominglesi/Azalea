using Azalea.Utils;
using Silk.NET.OpenAL;

namespace Azalea.Audios;
public class AudioSource : Disposable
{
	private AL _al;

	private uint _handle;
	private AudioInstance? _currentInstance;
	public AudioSource(uint handle, AL al)
	{
		_al = al;
		_handle = handle;
	}

	public AudioInstance Play(Sound sound)
	{
		Stop();

		_al.SetSourceProperty(_handle, SourceInteger.Buffer, sound.Buffer);
		_al.SetSourceProperty(_handle, SourceBoolean.Looping, false);

		_al.SourcePlay(_handle);

		return _currentInstance = new AudioInstance(this);
	}

	public void Stop()
	{
		if (_currentInstance is not null)
		{
			_al.SourceStop(_handle);
			_currentInstance.Stop();
			_currentInstance = null;
		}
	}

	protected override void OnDispose()
	{
		_al.DeleteSource(_handle);
	}
}
