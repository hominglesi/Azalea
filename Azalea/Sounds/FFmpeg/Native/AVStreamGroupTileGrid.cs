namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVStreamGroupTileGrid
{
	public AVClass* av_class;
	public uint nb_tiles;
	public int coded_width;
	public int coded_height;
	public _offsets* offsets;
	public byte_array4 background;
	public int horizontal_offset;
	public int vertical_offset;
	public int width;
	public int height;
	public AVPacketSideData* coded_side_data;
	public int nb_coded_side_data;

	public unsafe struct _offsets
	{
		public uint idx;
		public int horizontal;
		public int vertical;
	}
}
