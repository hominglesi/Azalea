using Azalea.Utils;

namespace Azalea.Platform.Audio.OpenAL;
internal partial class ALByteSource(uint handle)
{
	public readonly uint Handle = handle;

	[Observable] public partial AudioByteInstance? CurrentInstance { get; internal set; }
}
