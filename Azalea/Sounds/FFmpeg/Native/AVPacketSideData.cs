namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVPacketSideData
{
	public byte* data;
	public ulong size;
	public int type;
}
