namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVFrameSideData
{
	public int type;
	public byte* data;
	public ulong size;
	public AVDictionary* metadata;
	public AVBufferRef* buf;
}
