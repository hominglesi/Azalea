namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVCodecDescriptor
{
	public int id;
	public int type;
	public byte* name;
	public byte* long_name;
	public int props;
	public byte** mime_types;
	public AVProfile* profiles;
}
