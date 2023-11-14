using Azalea.Utils;

namespace Azalea.Audio.OpenAL;
internal class ALBuffer : Disposable
{
	public uint Handle;

	public ALBuffer()
	{
		Handle = ALC.GenBuffer();
	}

	public void SetData(ISoundData data)
	{
		ALC.BufferData(Handle, data);
	}

	protected override void OnDispose()
	{
		ALC.DeleteBuffer(Handle);
	}
}
