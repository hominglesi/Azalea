using Azalea.Threading;
using Azalea.Utils;

namespace Azalea.Sounds.OpenAL;
internal class ALBuffer : Disposable
{
	public readonly ValuePromise<uint> Handle;
	private readonly ALAudioManager _audioManager;

	public ALBuffer(ALAudioManager audioManager, uint handle)
	{
		_audioManager = audioManager;
		Handle = new(handle);
	}

	public ALBuffer(ALAudioManager audioManager)
	{
		_audioManager = audioManager;
		Handle = _audioManager.GenerateBuffer();
	}

	public void BufferData(byte[] data, int dataLength, ALFormat format, int frequency)
		=> _audioManager.BufferData(Handle, data, dataLength, format, frequency);

	public void BufferAndFreeData(byte[] data, int dataLength, ALFormat format, int frequency)
		=> _audioManager.BufferAndFreeData(Handle, data, dataLength, format, frequency);

	protected override void OnDispose()
		=> _audioManager.DeleteBuffer(Handle);
}
