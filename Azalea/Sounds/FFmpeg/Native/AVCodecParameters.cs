namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVCodecParameters
{
	public int codec_type;
	public int codec_id;
	public uint codec_tag;
	public byte* extradata;
	public int extradata_size;
	public AVPacketSideData* coded_side_data;
	public int nb_coded_side_data;
	public int format;
	public long bit_rate;
	public int bits_per_coded_sample;
	public int bits_per_raw_sample;
	public int profile;
	public int level;
	public int width;
	public int height;
	public AVRational sample_aspect_ratio;
	public AVRational framerate;
	public int field_order;
	public int color_range;
	public int color_primaries;
	public int color_trc;
	public int color_space;
	public int chroma_location;
	public int video_delay;
	public AVChannelLayout ch_layout;
	public int sample_rate;
	public int block_align;
	public int frame_size;
	public int initial_padding;
	public int trailing_padding;
	public int seek_preroll;
}
