using System.Runtime.InteropServices;

namespace Azalea.Native.FFmpeg;

public static unsafe partial class FFmpeg
{
	public struct SwrContext;

	[LibraryImport("swresample")]
	public static partial int swr_alloc_set_opts2(SwrContext** ps, AVChannelLayout* out_ch_layout, int out_sample_fmt, int out_sample_rate, AVChannelLayout* in_ch_layout, int in_sample_fmt, int in_sample_rate, int log_offset, void* log_ctx);

	[LibraryImport("swresample")]
	public static partial int swr_convert(SwrContext* ps, byte** @out, int out_count, byte** @in, int in_count);

	[LibraryImport("swresample")]
	public static partial void swr_free(SwrContext** s);

	[LibraryImport("swresample")]
	public static partial int swr_get_out_samples(SwrContext* s, int in_samples);

	[LibraryImport("swresample")]
	public static partial int swr_init(SwrContext* s);
}
