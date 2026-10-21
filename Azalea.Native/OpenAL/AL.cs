using Azalea.Native.Utils;

namespace Azalea.Native.OpenAL;

public static partial class AL
{
	static AL() => ImportResolving.AddDirectoryTranslation("soft_oal", "soft_oal.dll");
}
