using Silk.NET.OpenAL;
using System;

namespace Azalea.Audios;
public class AudioSource : IDisposable
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

	protected bool Disposed;
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (Disposed == false)
		{
			_al.DeleteSource(_handle);

			Disposed = true;
		}
	}
}
