namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVClass
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
