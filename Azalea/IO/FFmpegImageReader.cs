using Azalea.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static Azalea.Native.FFmpeg.FFmpeg;

namespace Azalea.IO;

internal unsafe class FFmpegImageReader
{
	public unsafe static bool IsWebP(Stream stream)
	{
		const int probeSize = 4096;
		var buffer = new byte[probeSize + AV_INPUT_BUFFER_PADDING_SIZE];

		int index = 0;
		int read;
		while (index < probeSize && (read = stream.Read(buffer, index, probeSize - index)) > 0)
			index += read;

		if (index == 0)
			return false;

		fixed (byte* p = buffer)
		{
			var probeData = new AVProbeData()
			{
				buf = p,
				buf_size = index
			};

			int score = 0;
			var fmt = av_probe_input_format2(ref probeData, 1, ref score);

			if (fmt is null)
				return false;

			var formatName = Marshal.PtrToStringAnsi((nint)fmt->name);

			return formatName == "webp_pipe";
		}
	}

	public unsafe static Image ReadWebP(Stream stream)
	{
		Debug.Assert(stream.Position == 0);

		var readerHandle = GCHandle.Alloc(stream);

		const int ioBufferSize = 32 * 1024;

		AVIOContext* io = avio_alloc_context((byte*)av_malloc(ioBufferSize), ioBufferSize, 0,
			(void*)GCHandle.ToIntPtr(readerHandle), (nint)_readPacketFunction, nint.Zero, (nint)_seekFunction);

		if (io is null)
			throw new Exception("Couldn't allocate context!");

		AVFormatContext* fmt = avformat_alloc_context();
		fmt->pb = io;
		fmt->flags |= AVFMT_FLAG_CUSTOM_IO;

		check(avformat_open_input(&fmt, null, null, null), "avformat_open_input");
		check(avformat_find_stream_info(fmt, null), "avformat_find_stream_info");

		AVCodec* codec = null;

		int bestStream = av_find_best_stream(fmt, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
		check(bestStream, "av_find_best_stream");

		AVCodecContext* cc = avcodec_alloc_context3(codec);
		check(avcodec_parameters_to_context(cc, fmt->streams[bestStream]->codecpar), "avcodec_parameters_to_context");
		check(avcodec_open2(cc, codec, null), "avcodec_open2");

		AVPacket* pkt = av_packet_alloc();
		AVFrame* frame = av_frame_alloc();

		bool found = false;
		while (found == false && av_read_frame(fmt, pkt) >= 0)
		{
			if (pkt->stream_index != bestStream)
				continue;

			check(avcodec_send_packet(cc, pkt), "avcodec_send_packet");
			int recieved = avcodec_receive_frame(cc, frame);
			if (recieved == 0)
				found = true;
			else if (recieved != AVERROR(EAGAIN))
				check(recieved, "avcodec_recieve_frame");

			av_packet_unref(pkt);
		}

		if (found == false)
		{
			avcodec_send_packet(cc, null);
			check(avcodec_receive_frame(cc, frame), "avcodec_receive_frame");
		}

		int width = frame->width;
		int height = frame->height;

		SwsContext* sws = sws_getContext(
			width, height, (AVPixelFormat)frame->format,
			width, height, AVPixelFormat.AV_PIX_FMT_RGBA,
			SWS_BILINEAR, null, null, null);

		if (sws is null)
			throw new Exception("sws_getContext failed");

		var rgba = new byte[checked(width * height * 4)];
		fixed (byte* dst = rgba)
		{
			var dstData = stackalloc byte*[4] { dst, null, null, null };
			var dstLinesize = stackalloc int[4] { width * 4, 0, 0, 0 };

			check(sws_scale(sws, frame->data, frame->linesize, 0, height, dstData, dstLinesize), "sws_scale");
		}

		sws_freeContext(sws);
		av_frame_free(&frame);
		av_packet_free(&pkt);
		avcodec_free_context(&cc);
		avformat_close_input(&fmt);
		av_freep(&io->buffer);
		avio_context_free(&io);
		readerHandle.Free();
		stream.Dispose();

		return new Image(width, height, rgba);
	}

	private static readonly delegate* unmanaged<void*, byte*, int, int> _readPacketFunction = &readPacket;
	[UnmanagedCallersOnly]
	private static int readPacket(void* ptr, byte* buffer, int bufferSize)
	{
		var target = GCHandle.FromIntPtr((nint)ptr).Target;
		if (target is not Stream)
			return AVERROR_EOF;

		var read = ((Stream)target).Read(new Span<byte>(buffer, bufferSize));

		return read > 0 ? read : AVERROR_EOF;
	}

	private static readonly delegate* unmanaged<void*, long, int, long> _seekFunction = &seek;
	[UnmanagedCallersOnly]
	private static long seek(void* ptr, long offset, int whence)
	{
		GCHandle handle = GCHandle.FromIntPtr((nint)ptr);
		if (handle.Target is null)
			return -1;

		Stream stream = (Stream)handle.Target;

		if (stream.CanSeek == false)
			return -1;

		if ((whence & AVSEEK_SIZE) != 0) return stream.Length;
		whence &= ~AVSEEK_FORCE;

		const int SEEK_SET = 0, SEEK_CUR = 1, SEEK_END = 2;
		long target = whence switch
		{
			SEEK_SET => offset,
			SEEK_CUR => stream.Position + offset,
			SEEK_END => stream.Length + offset,
			_ => -1
		};

		if (target < 0)
			return -1;

		stream.Position = target;
		return target;
	}

	private static void check(int error, string caller)
	{
		if (error >= 0)
			return;

		var buffer = stackalloc byte[1024];
		if (av_strerror(error, buffer, 1024) == 0)
			throw new Exception($"{caller} failed: {Marshal.PtrToStringAnsi((nint)buffer)}");
		else
			throw new Exception($"av_strerror couldn't describe {error}");
	}
}
