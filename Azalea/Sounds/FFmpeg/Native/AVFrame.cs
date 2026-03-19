namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVFrame
{
	public byte_ptrArray8 data;
	public int_array8 linesize;
	public byte** extended_data;
	public int width;
	public int height;
	public int nb_samples;
	public int format;
	public int pict_type;
	public AVRational sample_aspect_ratio;
	public long pts;
	public long pkt_dts;
	public AVRational time_base;
	public int quality;
	public void* opaque;
	public int repeat_pict;
	public int sample_rate;
	public AVBufferRef_ptrArray8 buf;
	public AVBufferRef** extended_buf;
	public int nb_extended_buf;
	public AVFrameSideData** side_data;
	public int nb_side_data;
	public int flags;
	public int color_range;
	public int color_primaries;
	public int color_trc;
	public int colorspace;
	public int chroma_location;
	public long best_effort_timestamp;
	public AVDictionary* metadata;
	public int decode_error_flags;
	public AVBufferRef* hw_frames_ctx;
	public AVBufferRef* opaque_ref;
	public ulong crop_top;
	public ulong crop_bottom;
	public ulong crop_left;
	public ulong crop_right;
	public void* private_ref;
	public AVChannelLayout ch_layout;
	public long duration;
}
