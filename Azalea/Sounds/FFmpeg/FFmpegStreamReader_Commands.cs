using Azalea.Sounds.FFmpeg.Native;
using System.Runtime.InteropServices;

namespace Azalea.Sounds.FFmpeg;
internal unsafe partial class FFmpegStreamReader
{
	internal static void Preload()
	{
		var _ = avcodec_version();
	}

	[LibraryImport("avutil")]
	private static partial void av_channel_layout_default(AVChannelLayout* ch_layout, int nb_channels);

	[LibraryImport("avutil")]
	private static partial void av_channel_layout_uninit(AVChannelLayout* channel_layout);

	[LibraryImport("avutil")]
	private static partial AVFrame* av_frame_alloc();

	[LibraryImport("avutil")]
	private static partial void av_frame_free(AVFrame** frame);

	private static AVRational av_inv_q(AVRational q) => new(q.den, q.num);

	[LibraryImport("avutil")]
	private static partial int av_log_format_line2(void* ptr, int level, byte* fmt, byte* vl, byte* line, int line_size, int* print_prefix);

	[LibraryImport("avutil")]
	private static partial void av_log_set_callback(delegate* unmanaged[Cdecl]<void*, int, byte*, byte*, void> callback);

	[LibraryImport("avutil")]
	private static partial void* av_malloc(ulong size);

	[LibraryImport("avutil", StringMarshalling = StringMarshalling.Utf8)]
	private static partial int av_opt_set_int(void* obj, string name, long val, int search_flags);

	[LibraryImport("avcodec")]
	private static partial AVPacket* av_packet_alloc();

	[LibraryImport("avcodec")]
	private static partial void av_packet_free(AVPacket** packet);

	[LibraryImport("avcodec")]
	private static partial void av_packet_unref(AVPacket* packet);

	[LibraryImport("avformat")]
	private static partial int av_read_frame(AVFormatContext* s, AVPacket* pkt);

	[LibraryImport("avutil")]
	private static partial int av_samples_get_buffer_size(int* linesize, int nb_channels, int nb_samples, int sample_fmt, int align);

	[LibraryImport("avformat")]
	private static partial int av_seek_frame(AVFormatContext* s, int stream_index, long timestamp, int flags);

	private static double av_q2d(AVRational a) => a.num / (double)a.den;

	[LibraryImport("avcodec")]
	private static partial AVCodecContext* avcodec_alloc_context3(AVCodec* codec);

	[LibraryImport("avcodec")]
	private static partial AVCodec* avcodec_find_decoder(int id);

	[LibraryImport("avcodec")]
	private static partial void avcodec_flush_buffers(AVCodecContext* avctx);

	[LibraryImport("avcodec")]
	private static partial void avcodec_free_context(AVCodecContext** avctx);

	[LibraryImport("avcodec")]
	private static partial int avcodec_open2(AVCodecContext* avctx, AVCodec* codec, AVDictionary** options);

	[LibraryImport("avcodec")]
	private static partial int avcodec_parameters_to_context(AVCodecContext* context, AVCodecParameters* par);

	[LibraryImport("avcodec")]
	private static partial int avcodec_receive_frame(AVCodecContext* avctx, AVFrame* frame);

	[LibraryImport("avcodec")]
	private static partial int avcodec_send_packet(AVCodecContext* avctx, AVPacket* avpkt);

	[LibraryImport("avcodec")]
	private static partial uint avcodec_version();

	[LibraryImport("avformat")]
	private static partial AVFormatContext* avformat_alloc_context();

	[LibraryImport("avformat")]
	private static partial void avformat_close_input(AVFormatContext** s);

	[LibraryImport("avformat")]
	private static partial int avformat_find_stream_info(AVFormatContext* ic, AVDictionary** options);

	[LibraryImport("avformat", StringMarshalling = StringMarshalling.Utf8)]
	private static partial int avformat_open_input(AVFormatContext** ps, string? url, AVInputFormat* format, AVDictionary** options);

	[LibraryImport("avformat")]
	private static partial AVIOContext* avio_alloc_context(byte* buffer, int buffer_size, int write_flag, void* opaque, nint read_packet, nint write_packet, nint seek);

	[LibraryImport("avformat")]
	private static partial void avio_context_free(AVIOContext** s);

	[LibraryImport("swresample")]
	private static partial int swr_alloc_set_opts2(SwrContext** ps, AVChannelLayout* out_ch_layout, int out_sample_fmt, int out_sample_rate, AVChannelLayout* in_ch_layout, int in_sample_fmt, int in_sample_rate, int log_offset, void* log_ctx);

	[LibraryImport("swresample")]
	private static partial int swr_convert(SwrContext* ps, byte** @out, int out_count, byte** @in, int in_count);

	[LibraryImport("swresample")]
	private static partial void swr_free(SwrContext** s);

	[LibraryImport("swresample")]
	private static partial int swr_get_out_samples(SwrContext* s, int in_samples);

	[LibraryImport("swresample")]
	private static partial int swr_init(SwrContext* s);
}
