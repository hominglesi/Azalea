namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVHWAccel
{
	public byte* name;
	public int type;
	public int id;
	public int pix_fmt;
	public int capabilities;
}
