namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVInputFormat
{
	public byte* name;
	public byte* long_name;
	public int flags;
	public byte* extensions;
	public AVCodecTag** codec_tag;
	public AVClass* priv_class;
	public byte* @mime_type;
}
