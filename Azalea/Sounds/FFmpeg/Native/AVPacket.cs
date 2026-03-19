namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVPacket
{
	public AVBufferRef* buf;
	public long pts;
	public long dts;
	public byte* data;
	public int size;
	public int stream_index;
	public int flags;
	public AVPacketSideData* side_data;
	public int side_data_elems;
	public long duration;
	public long pos;
	public void* opaque;
	public AVBufferRef* opaque_ref;
	public AVRational time_base;
}
