namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVStream
{
	public AVClass* av_class;
	public int index;
	public int id;
	public AVCodecParameters* codecpar;
	public void* priv_data;
	public AVRational time_base;
	public long start_time;
	public long duration;
	public long nb_frames;
	public int disposition;
	public int discard;
	public AVRational sample_aspect_ratio;
	public AVDictionary* metadata;
	public AVRational avg_frame_rate;
	public AVPacket attached_pic;
	public int event_flags;
	public AVRational r_frame_rate;
	public int pts_wrap_bits;
}
