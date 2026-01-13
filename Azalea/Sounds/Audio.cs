using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Sounds.OpenAL;
using Azalea.Utils;
using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Azalea.Sounds;
public static class Audio
{
	private static IAudioManager? _instance;
	public static IAudioManager Instance => _instance ??= GameHost.Main.AudioManager;

	public static float MasterVolume
	{
		get => Instance.MasterVolume;
		set => Instance.MasterVolume = value;
	}

	public static AudioInstance Play(Sound sound, float gain = 1, bool looping = false)
		=> Instance.Play(sound, gain, looping);

	public static AudioInstance PlayVital(Sound sound, float gain = 1, bool looping = false)
		=> Instance.PlayVital(sound, gain, looping);

	internal static AudioInstance PlayInternal(Sound sound, float gain = 1, bool looping = false)
		=> Instance.PlayInternal(sound, gain, looping);

	internal static Sound CreateSound(ISoundData data)
		=> Instance.CreateSound(data);

	public unsafe static void TestFunction()
	{
		ffmpeg.av_log_set_level(ffmpeg.AV_LOG_ERROR);

		var streamSource = new SoundStreamSource(Assets.FileSystemStore, @"E:\cut 3.mp3");
		var ffmpegReader = new FFmpegStreamReader(streamSource.GetStream()!);

		((ALAudioManager)Audio.Instance).Stream(ffmpegReader);
	}

	class FFmpegSoundData(byte[] data, int sampleRate) : ISoundData
	{
		private readonly byte[] _data = data;
		private readonly int _sampleRate = sampleRate;

		ALFormat ISoundData.Format => ALFormat.Stereo16;

		int ISoundData.ChannelCount => 2;

		Span<byte> ISoundData.Data => _data;

		int ISoundData.Size => _data.Length;

		int ISoundData.Frequency => _sampleRate;
	}

	public unsafe class FFmpegStreamReader : Disposable
	{
		const int __ioBufferSize = 4096;

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

		public AVFormatContext* FormatContext => _formatContext;

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

		private readonly Queue<byte[]> _pendingChunks = [];

		public bool ReadChunk(out byte[] pcm, out int sampleRate)
		{
			if (_pendingChunks.Count > 0)
			{
				pcm = _pendingChunks.Dequeue();
				sampleRate = _codecContext->sample_rate;
				return true;
			}

			if (ffmpeg.av_read_frame(_formatContext, _packet) < 0)
			{
				pcm = [];
				sampleRate = -1;
				return false;
			}

			if (_packet->stream_index == _audioStreamIndex)
			{
				ffmpeg.avcodec_send_packet(_codecContext, _packet);

				while (ffmpeg.avcodec_receive_frame(_codecContext, _frame) == 0)
					_pendingChunks.Enqueue(convertFrameToPcm(_frame, _swr));

			}

			ffmpeg.av_packet_unref(_packet);

			if (_pendingChunks.Count > 0)
			{
				pcm = _pendingChunks.Dequeue();
				sampleRate = _codecContext->sample_rate;
				return true;
			}
			else
			{
				pcm = [];
				sampleRate = _codecContext->sample_rate;
				return false;
			}

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
			{
				ffmpeg.avformat_close_input(fmtCtx);
				ffmpeg.avio_context_free(avioCtx);
				_streamHandle.Free();
				ffmpeg.av_free(_ioBuffer);
				ffmpeg.av_channel_layout_uninit(inLayout);
				ffmpeg.av_channel_layout_uninit(outLayout);
				ffmpeg.av_packet_free(packet);
				ffmpeg.av_frame_free(frame);
				ffmpeg.swr_free(swr);
			}
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

	public class SoundStreamSource(IResourceStore store, string name)
	{
		private readonly IResourceStore _store = store;
		private readonly string _name = name;

		public Stream? GetStream() => _store.GetStream(_name);
	}
}
