using Azalea.Sounds.FFmpeg.Native;
using Azalea.Sounds.OpenAL;
using Azalea.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azalea.Sounds.FFmpeg;
internal unsafe partial class FFmpegStreamReader : Disposable
{
	const int AV_TIME_BASE = 0xf4240;
	const int AVERROR_EOF = -541478213;

	const int __ioBufferSize = 4096;

	public readonly float TotalDuration;

	private readonly byte* _ioBuffer;
	private readonly int _audioStreamIndex = -1;
	private readonly GCHandle _streamHandle;
	private readonly AVFormatContext* _formatContext;
	private readonly AVIOContext* _avioContext;
	private readonly AVCodecContext* _codecContext;
	private readonly AVChannelLayout _inLayout;
	private readonly AVChannelLayout _outLayout;
	private readonly SwrContext* _swr;
	private readonly AVPacket* _packet;
	private readonly AVFrame* _frame;

	static FFmpegStreamReader()
	{
		av_log_set_callback(&logCallback);
	}

	[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
	private static void logCallback(void* ptr, int level, byte* format, byte* args)
	{
		const int AV_LOG_WARNING = 0x18;
		if (level > AV_LOG_WARNING)
			return;

		var buffer = stackalloc byte[1024];
		var printPrefix = 1;
		av_log_format_line2(ptr, level, format, args, buffer, 1024, &printPrefix);
		var message = Marshal.PtrToStringAnsi((IntPtr)buffer);

		if (message is null) return;

		if (_supressLogging)
		{
			if (message.Contains("timescale not set"))
				return;
		}

		Console.WriteLine(message);
	}

	private static bool _supressLogging = false;

	public unsafe FFmpegStreamReader(Stream stream)
	{
		_ioBuffer = (byte*)av_malloc(__ioBufferSize);

		_streamHandle = GCHandle.Alloc(stream);

		AVIOContext* avioCtx = avio_alloc_context(
			_ioBuffer,
			__ioBufferSize,
			0,
			(void*)GCHandle.ToIntPtr(_streamHandle),
			(nint)_readPacketPtr,
			nint.Zero,
			stream.CanSeek ? (nint)_seekPtr : nint.Zero
			);

		const int AVFMT_FLAG_CUSTOM_IO = 0x80;
		AVFormatContext* formatCtx = avformat_alloc_context();
		formatCtx->pb = avioCtx;
		formatCtx->flags |= AVFMT_FLAG_CUSTOM_IO;

		_supressLogging = true;
		if (avformat_open_input(&formatCtx, null, null, null) < 0)
			throw new Exception("Could not open AvFormat input!");
		_supressLogging = false;

		for (int i = 0; i < formatCtx->nb_streams; i++)
		{
			// If the stream doesn't have time scale data we infer it
			AVStream* str = formatCtx->streams[i];
			if (str->time_base.num == 0 || str->time_base.den == 0)
			{
				const int AVMEDIA_TYPE_VIDEO = 0;
				if (str->codecpar->codec_type == AVMEDIA_TYPE_VIDEO &&
					str->r_frame_rate.den != 0)
				{
					str->time_base = av_inv_q(str->r_frame_rate);
				}
				else if (str->codecpar->sample_rate != 0)
				{
					str->time_base.num = 1;
					str->time_base.den = str->codecpar->sample_rate;
				}
				else
				{
					str->time_base.num = 1;

					str->time_base.den = AV_TIME_BASE;
				}
			}
		}

		avformat_find_stream_info(formatCtx, null);

		TotalDuration = formatCtx->duration / (float)AV_TIME_BASE;
		if (TotalDuration < 0) TotalDuration = -1;

		for (int i = 0; i < formatCtx->nb_streams; i++)
		{
			const int AVMEDIA_TYPE_AUDIO = 1;
			if (formatCtx->streams[i]->codecpar->codec_type == AVMEDIA_TYPE_AUDIO)
			{
				_audioStreamIndex = i;
				break;
			}
		}

		AVCodecParameters* codecPar = formatCtx->streams[_audioStreamIndex]->codecpar;
		AVCodec* codec = avcodec_find_decoder(codecPar->codec_id);

		_codecContext = avcodec_alloc_context3(codec);
		avcodec_parameters_to_context(_codecContext, codecPar);

		_codecContext->pkt_timebase = formatCtx->streams[_audioStreamIndex]->time_base;
		avcodec_open2(_codecContext, codec, null);

		AVChannelLayout outLayout;
		av_channel_layout_default(&outLayout, 2);

		AVChannelLayout inLayout = _codecContext->ch_layout;
		if (inLayout.nb_channels == 0)
			av_channel_layout_default(&inLayout, _codecContext->ch_layout.nb_channels);

		SwrContext* swr;

		const int AV_SAMPLE_FMT_S16 = 1;
		var result = swr_alloc_set_opts2(
			&swr,
			&outLayout,
			AV_SAMPLE_FMT_S16,
			ALAudioManager.DEVICE_FREQUENCY, // This is kinda hard-coded for now but it will be changed

			&inLayout,
			_codecContext->sample_fmt,
			_codecContext->sample_rate,
			0,
			null);

		const int SWR_RESAMPLER_SOXR = 1;
		av_opt_set_int(swr, "resampler", SWR_RESAMPLER_SOXR, 0);

		swr_init(swr);

		_packet = av_packet_alloc();
		_frame = av_frame_alloc();

		_formatContext = formatCtx;
		_avioContext = avioCtx;
		_outLayout = outLayout;
		_inLayout = inLayout;
		_swr = swr;
	}

	private readonly Queue<(byte[], int, float)> _pendingChunks = [];

	public bool ReadChunk(out byte[] pcm, out int pcmLength, out int sampleRate, out float startTime)
	{
		// Since we now do the resampling ourselves, this will always just be
		// the device frequency
		sampleRate = ALAudioManager.DEVICE_FREQUENCY;

		if (Disposed)
		{
			pcm = [];
			startTime = -1;
			pcmLength = 0;
			return false;
		}

		lock (DisposeLock)
		{
			if (Disposed)
			{
				pcm = [];
				startTime = -1;
				pcmLength = 0;
				return false;
			}

			return readChunk(out pcm, out pcmLength, out sampleRate, out startTime);
		}
	}

	private bool readChunk(out byte[] pcm, out int pcmLength, out int sampleRate, out float startTime)
	{
		sampleRate = ALAudioManager.DEVICE_FREQUENCY;

		if (_pendingChunks.Count > 0)
		{
			(pcm, pcmLength, startTime) = _pendingChunks.Dequeue();
			return true;
		}

		double frameSeconds = 0;
		do
		{
			if (av_read_frame(_formatContext, _packet) < 0)
			{
				pcm = [];
				startTime = -1;
				pcmLength = 0;
				return false;
			}

			frameSeconds = (ulong)_packet->pts * av_q2d(_formatContext->streams[_audioStreamIndex]->time_base);
		}
		while (frameSeconds < _seekSeconds);

		if (_packet->stream_index == _audioStreamIndex)
		{
			avcodec_send_packet(_codecContext, _packet);

			while (avcodec_receive_frame(_codecContext, _frame) == 0)
			{
				const long AV_NOPT_VALUE = long.MinValue;
				var start = _frame->pts != AV_NOPT_VALUE
					? _frame->pts * (float)av_q2d(_formatContext->streams[_audioStreamIndex]->time_base)
					: -1;

				var readPcm = convertFrameToPcm(_frame, _swr, out var bufferLength);

				_pendingChunks.Enqueue((readPcm, bufferLength, start));
			}
		}

		av_packet_unref(_packet);

		if (_pendingChunks.Count > 0)
		{
			(pcm, pcmLength, startTime) = _pendingChunks.Dequeue();
			return true;
		}
		else
		{
			return ReadChunk(out pcm, out pcmLength, out sampleRate, out startTime);
		}
	}

	private float _seekSeconds;

	public void Seek(float seconds)
	{
		if (Disposed)
			return;

		lock (DisposeLock)
		{
			if (Disposed)
				return;

			seek(seconds);
		}
	}

	private void seek(float seconds)
	{
		_pendingChunks.Clear();

		_seekSeconds = seconds;
		var seekTimestamp = seconds * AV_TIME_BASE;

		const int AVSEEK_FLAG_BACKWARD = 0x1;
		av_seek_frame(_formatContext, -1, (long)seekTimestamp, AVSEEK_FLAG_BACKWARD);
		avcodec_flush_buffers(_codecContext);
		swr_init(_swr);
	}

	protected override void OnDispose()
	{
		fixed (AVFormatContext** fmtCtx = &_formatContext)
		fixed (AVIOContext** avioCtx = &_avioContext)
		fixed (AVChannelLayout* inLayout = &_inLayout)
		fixed (AVChannelLayout* outLayout = &_outLayout)
		fixed (AVPacket** packet = &_packet)
		fixed (AVFrame** frame = &_frame)
		fixed (SwrContext** swr = &_swr)
		fixed (AVCodecContext** codecCtx = &_codecContext)
		{
			av_frame_free(frame);
			av_packet_free(packet);
			swr_free(swr);
			av_channel_layout_uninit(inLayout);
			av_channel_layout_uninit(outLayout);
			avcodec_free_context(codecCtx);
			avformat_close_input(fmtCtx);
			avio_context_free(avioCtx);
		}

		_streamHandle.Free();

		// Freeing the buffer results in a crash
		// We are currently just tanking the leak
		// ffmpeg.av_free(_ioBuffer);
	}

	private static byte[] convertFrameToPcm(AVFrame* frame, SwrContext* swr, out int bufferLength)
	{
		const int AV_SAMPLE_FMT_S16 = 1;
		int dstSamples = swr_get_out_samples(swr, frame->nb_samples);
		int bufferSize = av_samples_get_buffer_size(
			null, 2, dstSamples, AV_SAMPLE_FMT_S16, 1);

		byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
		fixed (byte* outPtr = buffer)
		{
			byte** outArr = stackalloc byte*[2];
			outArr[0] = outPtr;
			outArr[1] = null;

			var sampleCount = swr_convert(swr, outArr, dstSamples,
				frame->extended_data, frame->nb_samples);

			// Since we're using signed 16 byte stereo samples we multiply the resulting samples
			// by 4 to get the buffer size
			bufferLength = sampleCount * 4;
		}

		return buffer;
	}

	private delegate* unmanaged<void*, byte*, int, int> _readPacketPtr = &readPacket;
	[UnmanagedCallersOnly]
	private static unsafe int readPacket(void* ptr, byte* buffer, int bufferSize)
	{
		GCHandle handle = GCHandle.FromIntPtr((IntPtr)ptr);
		if (handle.Target is null)
			return AVERROR_EOF;

		Stream stream = (Stream)handle.Target;

		byte[] managedBuffer = new byte[bufferSize];
		int bytesRead = stream.Read(managedBuffer, 0, bufferSize);

		if (bytesRead > 0)
			Marshal.Copy(managedBuffer, 0, (IntPtr)buffer, bytesRead);

		return bytesRead > 0 ? bytesRead : AVERROR_EOF;
	}

	private delegate* unmanaged<void*, long, int, long> _seekPtr = &seek;
	[UnmanagedCallersOnly]
	private static unsafe long seek(void* ptr, long offset, int whence)
	{
		GCHandle handle = GCHandle.FromIntPtr((IntPtr)ptr);
		if (handle.Target is null)
			return -1;

		Stream stream = (Stream)handle.Target;

		if (stream.CanSeek == false)
			return -1;

		const int SEEK_SET = 0, SEEK_CUR = 1, SEEK_END = 2, AVSEEK_SIZE = 0x10000;
		switch (whence)
		{
			case SEEK_SET:
				stream.Position = offset;
				break;
			case SEEK_CUR:
				stream.Position += offset;
				break;
			case SEEK_END:
				stream.Position = stream.Length + offset;
				break;
			case AVSEEK_SIZE:
				return stream.Length;
		}

		return stream.Position;
	}
}
