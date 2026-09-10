using Azalea.Utils;

namespace Azalea.Platform.Audio.OpenAL;
internal class ALByteSource(uint handle)
{
	public readonly uint Handle = handle;
	public ReadOnlyObservable<AudioByteInstance?> CurrentInstance = new(null);
}
