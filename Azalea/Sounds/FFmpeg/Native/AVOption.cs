using System.Runtime.InteropServices;

namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVOption
{
	public byte name;
	public byte help;
	public int offset;
	public int type;
	public _default_val default_val;
	public double min;
	public double max;
	public int flags;
	public byte* unit;

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct _default_val
	{
		[FieldOffset(0)]
		public long i64;
		[FieldOffset(0)]
		public double dbl;
		[FieldOffset(0)]
		public byte* str;
		[FieldOffset(0)]
		public AVRational q;
		[FieldOffset(0)]
		public AVOptionArrayDef* arr;
	}
}
