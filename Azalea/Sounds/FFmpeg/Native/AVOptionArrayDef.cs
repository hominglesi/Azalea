namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVOptionArrayDef
{
	public byte* def;
	public uint size_min;
	public uint size_max;
	public byte sep;
}
