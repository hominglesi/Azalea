namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVIOInterruptCB
{
	public nint callback;
	public void* opaque;
}
