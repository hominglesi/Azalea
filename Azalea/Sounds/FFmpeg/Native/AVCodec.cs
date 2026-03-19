namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVCodec
{
	public byte* name;
	public byte* long_name;
	public int type;
	public int id;
	public int capabilities;
	public byte max_lowres;
	public AVRational* supported_framerates;
	public int* pix_fmts;
	public int* supported_samplerates;
	public int* sample_fmts;
	public AVClass* priv_class;
	public AVProfile* profiles;
	public byte* wrapper_name;
	public AVChannelLayout* ch_layouts;
}
