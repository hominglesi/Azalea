using Azalea.Native.Utils;
using System.Runtime.InteropServices;

namespace Azalea.Native.FFmpeg;

public static unsafe partial class FFmpeg
{
	public struct SwsContext;

	public struct SwsFilter
	{
		public SwsVector* lumH;
		public SwsVector* lumV;
		public SwsVector* chrH;
		public SwsVector* chrV;
	}

	public struct SwsVector
	{
		public double* coeff;
		public int length;
	}

	[LibraryImport("swscale")]
	public static partial SwsContext* sws_getContext(int srcW, int srcH, AVPixelFormat srcFormat, int dstW, int dstH, AVPixelFormat dstFormat, int flags, SwsFilter* srcFilter, SwsFilter* dstFilter, double* param);

	[LibraryImport("swscale")]
	public static partial void sws_freeContext(SwsContext* swsContext);

	[LibraryImport("swscale")]
	public static partial int sws_scale(SwsContext* c, BytePtrArray8 srcSlice, int* srcStride, int srcSliceY, int srcSliceH, byte** dst, int* dstStride);
}
