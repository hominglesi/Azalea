using System.Runtime.InteropServices;

namespace Azalea.Native.FFmpeg;

public static unsafe partial class FFmpeg
{
	public const int AVSEEK_SIZE = 0x10000;
	public const int AVSEEK_FORCE = 0x20000;
	public const int AVFMT_FLAG_CUSTOM_IO = 0x0080;

	public struct AVChapter
	{
		public long id;
		public AVRational time_base;
		public long start;
		public long end;
		public AVDictionary* metadata;
	}

	public struct AVCodecTag;

	public struct AVFormatContext
	{
		public AVClass* av_class;
		public AVInputFormat* iformat;
		public AVOutputFormat* oformat;
		public void* priv_data;
		public AVIOContext* pb;
		public int ctx_flags;
		public uint nb_streams;
		public AVStream** streams;
		public uint nb_stream_groups;
		public AVStreamGroup** stream_groups;
		public uint nb_chapters;
		public AVChapter** chapters;
		public byte* url;
		public long start_time;
		public long duration;
		public long bit_rate;
		public uint packet_size;
		public int max_delay;
		public int flags;
		public long probesize;
		public long max_analyze_duration;
		public byte* key;
		public int keylen;
		public uint nb_programs;
		public AVProgram** programs;
		public int video_codec_id;
		public int audio_codec_id;
		public int subtitle_codec_id;
		public int data_codec_id;
		public AVDictionary* metadata;
		public long start_time_realtime;
		public int fps_probe_size;
		public int error_recognition;
		public AVIOInterruptCB interrupt_callback;
		public int debug;
		public int max_streams;
		public uint max_index_size;
		public uint max_picture_buffer;
		public long max_interleave_delta;
		public int max_ts_probe;
		public int max_chunk_duration;
		public int max_chunk_size;
		public int max_probe_packets;
		public int strict_std_compliance;
		public int event_flags;
		public int avoid_negative_ts;
		public int audio_preload;
		public int use_wallclock_as_timestamps;
		public int skip_estimate_duration_from_pts;
		public int avio_flags;
		public int duration_estimation_method;
		public long skip_initial_bytes;
		public uint correct_ts_overflow;
		public int seek2any;
		public int flush_packets;
		public int probe_score;
		public int format_probesize;
		public byte* codec_whitelist;
		public byte* format_whitelist;
		public byte* protocol_whitelist;
		public byte* protocol_blacklist;
		public int io_repositioned;
		public AVCodec* video_codec;
		public AVCodec* audio_codec;
		public AVCodec* subtitle_codec;
		public AVCodec* data_codec;
		public int metadata_header_padding;
		public void* opaque;
		public nint control_message_cb;
		public long output_ts_offset;
		public byte* dump_separator;
		public nint io_open;
		public nint io_close2;
		public long duration_probesize;
	}

	public struct AVInputFormat
	{
		public byte* name;
		public byte* long_name;
		public int flags;
		public byte* extensions;
		public AVCodecTag** codec_tag;
		public AVClass* priv_class;
		public byte* @mime_type;
	}

	public struct AVIOContext
	{
		public AVClass* av_class;
		public byte* buffer;
		public int buffer_size;
		public byte* buf_ptr;
		public byte* buf_end;
		public void* opaque;
		public nint read_packet;
		public nint write_packet;
		public nint seek;
		public long pos;
		public int eof_reached;
		public int error;
		public int write_flag;
		public int max_packet_size;
		public int min_packet_size;
		public ulong checksum;
		public byte* checksum_ptr;
		public nint update_checksum;
		public nint read_pause;
		public nint read_seek;
		public int seekable;
		public int direct;
		public byte* protocol_whitelist;
		public byte* protocol_blacklist;
		public nint write_data_type;
		public int ignore_boundary_point;
		public byte* buf_ptr_max;
		public long bytes_read;
		public long bytes_written;
	}

	public struct AVIOInterruptCB
	{
		public nint callback;
		public void* opaque;
	}

	public struct AVOutputFormat
	{
		public byte* name;
		public byte* long_name;
		public byte* mime_type;
		public byte* extensions;
		public int audio_codec;
		public int video_codec;
		public int subtitle_codec;
		public int flags;
		public AVCodecTag** codec_tag;
		public AVClass* priv_class;
	}

	public struct AVProbeData
	{
		public byte* filename;
		public byte* buf;
		public int buf_size;
		public byte* mime_type;
	}

	public struct AVProgram
	{
		public int id;
		public int flags;
		public int discard;
		public uint* stream_index;
		public uint nb_stream_indexes;
		public AVDictionary* metadata;
		public int program_num;
		public int pmt_pid;
		public int pcr_pid;
		public int pmt_version;
		public long start_time;
		public long end_time;
		public long pts_wrap_reference;
		public int pts_wrap_behavior;
	}

	public struct AVStream
	{
		public AVClass* av_class;
		public int index;
		public int id;
		public AVCodecParameters* codecpar;
		public void* priv_data;
		public AVRational time_base;
		public long start_time;
		public long duration;
		public long nb_frames;
		public int disposition;
		public int discard;
		public AVRational sample_aspect_ratio;
		public AVDictionary* metadata;
		public AVRational avg_frame_rate;
		public AVPacket attached_pic;
		public int event_flags;
		public AVRational r_frame_rate;
		public int pts_wrap_bits;
	}

	public struct AVStreamGroup
	{
		public AVClass* av_class;
		public void* priv_data;
		public uint index;
		public long id;
		public int type;
		public _params @params;
		public AVDictionary* metadata;
		public uint nb_streams;
		public AVStream** streams;
		public int disposition;

		[StructLayout(LayoutKind.Explicit)]
		public struct _params
		{
			[FieldOffset(0)]
			public AVIAMFAudioElement* iamf_audio_element;
			[FieldOffset(0)]
			public AVIAMFMixPresentation* iamf_mix_presentation;
			[FieldOffset(0)]
			public AVStreamGroupTileGrid* tile_grid;
			[FieldOffset(0)]
			public AVStreamGroupLCEVC* lcevc;
		}
	}

	public struct AVStreamGroupLCEVC
	{
		public AVClass* av_class;
		public uint lcevc_index;
		public int width;
		public int height;
	}

	public struct AVStreamGroupTileGrid
	{
		public AVClass* av_class;
		public uint nb_tiles;
		public int coded_width;
		public int coded_height;
		public _offsets* offsets;
		public fixed byte background[4];
		public int horizontal_offset;
		public int vertical_offset;
		public int width;
		public int height;
		public AVPacketSideData* coded_side_data;
		public int nb_coded_side_data;

		public struct _offsets
		{
			public uint idx;
			public int horizontal;
			public int vertical;
		}
	}

	[LibraryImport("avformat")]
	public static partial AVInputFormat* av_probe_input_format2(ref AVProbeData pd, int is_opened, ref int score_max);

	[LibraryImport("avformat")]
	public static partial int av_read_frame(AVFormatContext* s, AVPacket* pkt);

	[LibraryImport("avformat")]
	public static partial int av_seek_frame(AVFormatContext* s, int stream_index, long timestamp, int flags);

	[LibraryImport("avformat")]
	public static partial int av_find_best_stream(AVFormatContext* ic, AVMediaType type, int wanted_stream_nb, int related_stream, AVCodec** decoder_ret, int flags);

	[LibraryImport("avformat")]
	public static partial AVFormatContext* avformat_alloc_context();

	[LibraryImport("avformat")]
	public static partial void avformat_close_input(AVFormatContext** s);

	[LibraryImport("avformat")]
	public static partial int avformat_find_stream_info(AVFormatContext* ic, AVDictionary** options);

	[LibraryImport("avformat", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int avformat_open_input(AVFormatContext** ps, string? url, AVInputFormat* format, AVDictionary** options);

	[LibraryImport("avformat")]
	public static partial AVIOContext* avio_alloc_context(byte* buffer, int buffer_size, int write_flag, void* opaque, nint read_packet, nint write_packet, nint seek);

	[LibraryImport("avformat")]
	public static partial void avio_context_free(AVIOContext** s);
}
