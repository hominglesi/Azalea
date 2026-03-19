namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVOutputFormat
{
	public byte* name;
	public byte* long_name;
	public byte* mime_type;
	public byte* extensions;
	public int audio_codec;
	public int video_codec;
	public int subtitle_codec;
	public int flags;
	public AVCodecTag** codec_tag;
	public AVClass* priv_class;
}
