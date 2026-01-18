using Azalea.Utils;

namespace Azalea.Sounds.OpenAL;
internal class ALBuffer : Disposable
{
	public uint Handle;

	public ALBuffer(uint handle)
	{
		Handle = handle;
	}

	public ALBuffer()
		: this(ALC.GenBuffer()) { }

	public void SetData(byte[] data, ALFormat format, int frequency)
		=> ALC.BufferData(Handle, format, data, data.Length, frequency);

	protected override void OnDispose()
		=> ALC.DeleteBuffer(Handle);
}
