using System.Runtime.InteropServices;

namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVChannelLayout
{
	public int order;
	public int nb_channels;
	public _u u;
	public void* opaque;

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct _u
	{
		[FieldOffset(0)]
		public ulong mask;
		[FieldOffset(0)]
		public AVChannelCustom* map;
	}
}
