namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVBufferRef
{
	public AVBuffer* buffer;
	public byte* data;
	public ulong size;
}
