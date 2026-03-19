namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVIOContext
{
	public AVClass* av_class;
	public byte* buffer;
	public int buffer_size;
	public byte* buf_ptr;
	public byte* buf_end;
	public void* opaque;
	public nint read_packet;
	public nint write_packet;
	public nint seek;
	public long pos;
	public int eof_reached;
	public int error;
	public int write_flag;
	public int max_packet_size;
	public int min_packet_size;
	public ulong checksum;
	public byte* checksum_ptr;
	public nint update_checksum;
	public nint read_pause;
	public nint read_seek;
	public int seekable;
	public int direct;
	public byte* protocol_whitelist;
	public byte* protocol_blacklist;
	public nint write_data_type;
	public int ignore_boundary_point;
	public byte* buf_ptr_max;
	public long bytes_read;
	public long bytes_written;
}
