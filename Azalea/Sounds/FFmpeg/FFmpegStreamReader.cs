using Azalea.Utils;
using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Azalea.Sounds.FFmpeg;
internal unsafe class FFmpegStreamReader : Disposable
{
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

	public unsafe FFmpegStreamReader(Stream stream)
	{
		_ioBuffer = (byte*)ffmpeg.av_malloc(__ioBufferSize);

		_streamHandle = GCHandle.Alloc(stream);

		AVIOContext* avioCtx = ffmpeg.avio_alloc_context(
			_ioBuffer,
			__ioBufferSize,
			0,
			(void*)GCHandle.ToIntPtr(_streamHandle),
			new avio_alloc_context_read_packet_func { Pointer = (nint)_readPacketPtr },
			null,
			stream.CanSeek ? new avio_alloc_context_seek_func() { Pointer = (nint)_seekPtr } : null
			);

		AVFormatContext* formatCtx = ffmpeg.avformat_alloc_context();
		formatCtx->pb = avioCtx;
		formatCtx->flags |= ffmpeg.AVFMT_FLAG_CUSTOM_IO;

		if (ffmpeg.avformat_open_input(&formatCtx, null, null, null) < 0)
			throw new Exception("Could not open AvFormat input!");

		ffmpeg.avformat_find_stream_info(formatCtx, null);

		TotalDuration = formatCtx->duration / (float)ffmpeg.AV_TIME_BASE;
		if (TotalDuration < 0) TotalDuration = -1;

		for (int i = 0; i < formatCtx->nb_streams; i++)
		{
			if (formatCtx->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
			{
				_audioStreamIndex = i;
				break;
			}
		}

		AVCodecParameters* codecPar = formatCtx->streams[_audioStreamIndex]->codecpar;
		AVCodec* codec = ffmpeg.avcodec_find_decoder(codecPar->codec_id);

		_codecContext = ffmpeg.avcodec_alloc_context3(codec);
		ffmpeg.avcodec_parameters_to_context(_codecContext, codecPar);
		_codecContext->pkt_timebase = formatCtx->streams[_audioStreamIndex]->time_base;
		ffmpeg.avcodec_open2(_codecContext, codec, null);

		AVChannelLayout outLayout;
		ffmpeg.av_channel_layout_default(&outLayout, 2);

		AVChannelLayout inLayout = _codecContext->ch_layout;
		if (inLayout.nb_channels == 0)
			ffmpeg.av_channel_layout_default(&inLayout, _codecContext->ch_layout.nb_channels);

		SwrContext* swr;

		var result = ffmpeg.swr_alloc_set_opts2(
			&swr,
			&outLayout,
			AVSampleFormat.AV_SAMPLE_FMT_S16,
			_codecContext->sample_rate,

			&inLayout,
			_codecContext->sample_fmt,
			_codecContext->sample_rate,
			0,
			null);

		ffmpeg.swr_init(swr);

		_packet = ffmpeg.av_packet_alloc();
		_frame = ffmpeg.av_frame_alloc();

		_formatContext = formatCtx;
		_avioContext = avioCtx;
		_outLayout = outLayout;
		_inLayout = inLayout;
		_swr = swr;
	}

	private readonly Queue<(byte[], float)> _pendingChunks = [];

	public bool ReadChunk(out byte[] pcm, out int sampleRate, out float startTime)
	{
		if (_pendingChunks.Count > 0)
		{
			(pcm, startTime) = _pendingChunks.Dequeue();
			sampleRate = _codecContext->sample_rate;
			return true;
		}

		double frameSeconds = 0;
		do
		{
			var result = ffmpeg.av_read_frame(_formatContext, _packet);

			if (result < 0)
			{
				pcm = [];
				sampleRate = -1;
				startTime = -1;
				return false;
			}

			frameSeconds = (ulong)_packet->pts * ffmpeg.av_q2d(_formatContext->streams[_audioStreamIndex]->time_base);
		}
		while (frameSeconds < _seekSeconds);

		if (_packet->stream_index == _audioStreamIndex)
		{
			ffmpeg.avcodec_send_packet(_codecContext, _packet);

			while (ffmpeg.avcodec_receive_frame(_codecContext, _frame) == 0)
			{
				var start = _frame->pts != ffmpeg.AV_NOPTS_VALUE
					? _frame->pts * (float)ffmpeg.av_q2d(_formatContext->streams[_audioStreamIndex]->time_base)
					: -1;

				_pendingChunks.Enqueue((convertFrameToPcm(_frame, _swr), start));
			}
		}

		ffmpeg.av_packet_unref(_packet);

		if (_pendingChunks.Count > 0)
		{
			(pcm, startTime) = _pendingChunks.Dequeue();
			sampleRate = _codecContext->sample_rate;
			return true;
		}
		else
		{
			var result = ReadChunk(out pcm, out sampleRate, out startTime);

			return result;
		}
	}

	private float _seekSeconds;

	public void Seek(float seconds)
	{
		_pendingChunks.Clear();

		_seekSeconds = seconds;
		var seekTimestamp = seconds * ffmpeg.AV_TIME_BASE;

		ffmpeg.av_seek_frame(_formatContext, -1, (long)seekTimestamp, ffmpeg.AV_TIME_BASE);

		ffmpeg.avcodec_flush_buffers(_codecContext);
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
			ffmpeg.av_frame_free(frame);
			ffmpeg.av_packet_free(packet);
			ffmpeg.swr_free(swr);
			ffmpeg.av_channel_layout_uninit(inLayout);
			ffmpeg.av_channel_layout_uninit(outLayout);
			ffmpeg.avcodec_free_context(codecCtx);
			ffmpeg.avformat_close_input(fmtCtx);
			ffmpeg.avio_context_free(avioCtx);
		}

		_streamHandle.Free();

		// Freeing the buffer results in a crash
		// We are currently just tanking the leak
		// ffmpeg.av_free(_ioBuffer);
	}

	private static byte[] convertFrameToPcm(AVFrame* frame, SwrContext* swr)
	{
		int dstSamples = ffmpeg.swr_get_out_samples(swr, frame->nb_samples);
		int bufferSize = ffmpeg.av_samples_get_buffer_size(
			null, 2, dstSamples, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);

		byte[] buffer = new byte[bufferSize];
		fixed (byte* outPtr = buffer)
		{
			byte** outArr = stackalloc byte*[1];
			outArr[0] = outPtr;

			ffmpeg.swr_convert(swr, outArr, dstSamples,
				frame->extended_data, frame->nb_samples);
		}

		return buffer;
	}

	private delegate* unmanaged<void*, byte*, int, int> _readPacketPtr = &readPacket;
	[UnmanagedCallersOnly]
	private static unsafe int readPacket(void* ptr, byte* buffer, int bufferSize)
	{
		GCHandle handle = GCHandle.FromIntPtr((IntPtr)ptr);
		if (handle.Target is null)
			return ffmpeg.AVERROR_EOF;

		Stream stream = (Stream)handle.Target;

		byte[] managedBuffer = new byte[bufferSize];
		int bytesRead = stream.Read(managedBuffer, 0, bufferSize);

		if (bytesRead > 0)
			Marshal.Copy(managedBuffer, 0, (IntPtr)buffer, bytesRead);

		return bytesRead > 0 ? bytesRead : ffmpeg.AVERROR_EOF;
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

		switch (whence)
		{
			case /* SEEK_SET */ 0:
				stream.Position = offset;
				break;
			case /* SEEK_CUR */ 1:
				stream.Position += offset;
				break;
			case /* SEEK_END */ 2:
				stream.Position = stream.Length + offset;
				break;
			case ffmpeg.AVSEEK_SIZE:
				return stream.Length;
		}

		return stream.Position;
	}
}
