namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVChannelCustom
{
	public int id;
	public byte_array16 name;
	public void* opaque;
}
