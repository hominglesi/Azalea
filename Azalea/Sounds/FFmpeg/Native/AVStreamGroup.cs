using System.Runtime.InteropServices;

namespace Azalea.Sounds.FFmpeg.Native;

internal unsafe struct AVStreamGroup
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
	public unsafe struct _params
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
