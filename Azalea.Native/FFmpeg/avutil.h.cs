using Azalea.Native.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azalea.Native.FFmpeg;

public static unsafe partial class FFmpeg
{
	public struct AVBuffer;

	public struct AVBufferRef
	{
		public AVBuffer* buffer;
		public byte* data;
		public ulong size;
	}

	[InlineArray(8)]
	public struct AVBufferRefPtrArray8
	{
		private nint _element0;
	}

	public struct AVChannelCustom
	{
		public int id;
		public fixed byte name[16];
		public void* opaque;
	}

	public struct AVChannelLayout
	{
		public int order;
		public int nb_channels;
		public _u u;
		public void* opaque;

		[StructLayout(LayoutKind.Explicit)]
		public struct _u
		{
			[FieldOffset(0)]
			public ulong mask;
			[FieldOffset(0)]
			public AVChannelCustom* map;
		}
	}

	public unsafe struct AVClass
	{
		public byte* class_name;
		public nint item_name;
		public AVOption* option;
		public int version;
		public int log_level_offset_offset;
		public int parent_log_context_offset;
		public int category;
		public nint get_category;
		public nint query_ranges;
		public nint child_next;
		public nint child_class_iterate;
		public int state_flags_offset;
	}

	public struct AVDictionary;

	public struct AVFrame
	{
		public BytePtrArray8 data;
		public fixed int linesize[8];
		public byte** extended_data;
		public int width;
		public int height;
		public int nb_samples;
		public int format;
		public int pict_type;
		public AVRational sample_aspect_ratio;
		public long pts;
		public long pkt_dts;
		public AVRational time_base;
		public int quality;
		public void* opaque;
		public int repeat_pict;
		public int sample_rate;
		public AVBufferRefPtrArray8 buf;
		public AVBufferRef** extended_buf;
		public int nb_extended_buf;
		public AVFrameSideData** side_data;
		public int nb_side_data;
		public int flags;
		public int color_range;
		public int color_primaries;
		public int color_trc;
		public int colorspace;
		public int chroma_location;
		public long best_effort_timestamp;
		public AVDictionary* metadata;
		public int decode_error_flags;
		public AVBufferRef* hw_frames_ctx;
		public AVBufferRef* opaque_ref;
		public ulong crop_top;
		public ulong crop_bottom;
		public ulong crop_left;
		public ulong crop_right;
		public void* private_ref;
		public AVChannelLayout ch_layout;
		public long duration;
	}

	public struct AVFrameSideData
	{
		public int type;
		public byte* data;
		public ulong size;
		public AVDictionary* metadata;
		public AVBufferRef* buf;
	}

	public struct AVIAMFAudioElement;

	public struct AVIAMFMixPresentation;

	public struct AVOption
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

	public struct AVOptionArrayDef
	{
		public byte* def;
		public uint size_min;
		public uint size_max;
		public byte sep;
	}

	public struct AVRational(int num, int den)
	{
		public int num = num;
		public int den = den;
	}

	[LibraryImport("avutil")]
	public static partial void av_channel_layout_default(AVChannelLayout* ch_layout, int nb_channels);

	[LibraryImport("avutil")]
	public static partial void av_channel_layout_uninit(AVChannelLayout* channel_layout);

	[LibraryImport("avutil")]
	public static partial AVFrame* av_frame_alloc();

	[LibraryImport("avutil")]
	public static partial void av_frame_free(AVFrame** frame);

	public static AVRational av_inv_q(AVRational q) => new(q.den, q.num);

	[LibraryImport("avutil")]
	public static partial int av_log_format_line2(void* ptr, int level, byte* fmt, byte* vl, byte* line, int line_size, int* print_prefix);

	[LibraryImport("avutil")]
	public static partial void av_log_set_callback(delegate* unmanaged[Cdecl]<void*, int, byte*, byte*, void> callback);

	[LibraryImport("avutil")]
	public static partial void* av_malloc(ulong size);

	[LibraryImport("avutil", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int av_opt_set_int(void* obj, string name, long val, int search_flags);

	public static double av_q2d(AVRational a) => a.num / (double)a.den;

	[LibraryImport("avutil")]
	public static partial int av_samples_get_buffer_size(int* linesize, int nb_channels, int nb_samples, int sample_fmt, int align);
}
