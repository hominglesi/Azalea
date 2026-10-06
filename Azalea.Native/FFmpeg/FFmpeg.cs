using Azalea.Native.Utils;

namespace Azalea.Native.FFmpeg;

public static partial class FFmpeg
{
	static FFmpeg()
	{
		ImportResolving.AddDirectoryTranslation("avcodec", "avcodec-62.dll");
		ImportResolving.AddDirectoryTranslation("avdevice", "avdevice-62.dll");
		ImportResolving.AddDirectoryTranslation("avfilter", "avfilter-11.dll");
		ImportResolving.AddDirectoryTranslation("avformat", "avformat-62.dll");
		ImportResolving.AddDirectoryTranslation("avutil", "avutil-60.dll");
		ImportResolving.AddDirectoryTranslation("swresample", "swresample-6.dll");
		ImportResolving.AddDirectoryTranslation("swscale", "swscale-9.dll");
	}
}
