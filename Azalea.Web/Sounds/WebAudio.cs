using System;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Sounds;
internal static partial class WebAudio
{
	private const string ImportString = "JSImports";

	[JSImport("WebAudio.BufferData", ImportString)]
	internal static partial void BufferData([JSMarshalAs<JSType.Any>] object buffer, [JSMarshalAs<JSType.MemoryView>] Span<byte> data);

	[JSImport("WebAudio.Connect", ImportString)]
	internal static partial void Connect([JSMarshalAs<JSType.Any>] object source, [JSMarshalAs<JSType.Any>] object destination);

	[JSImport("WebAudio.ConnectToContext", ImportString)]
	internal static partial void ConnectToContext([JSMarshalAs<JSType.Any>] object source);

	[JSImport("WebAudio.CreateBuffer", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateBuffer(int channels, int size, int sampleRate);

	[JSImport("WebAudio.CreateBufferSource", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateBufferSource();

	[JSImport("WebAudio.CreateGain", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateGain();

	[JSImport("WebAudio.SetBuffer", ImportString)]
	internal static partial void SetBuffer([JSMarshalAs<JSType.Any>] object source, [JSMarshalAs<JSType.Any>] object buffer);

	[JSImport("WebAudio.SetGain", ImportString)]
	internal static partial void SetGain([JSMarshalAs<JSType.Any>] object gainNode, double gain);

	[JSImport("WebAudio.SetLoop", ImportString)]
	internal static partial void SetLoop([JSMarshalAs<JSType.Any>] object source, bool loop);

	[JSImport("WebAudio.SetMasterVolume", ImportString)]
	internal static partial void SetMasterVolume(double volume);

	[JSImport("WebAudio.StartSource", ImportString)]
	internal static partial void StartSource([JSMarshalAs<JSType.Any>] object source);

	[JSImport("WebAudio.StopSource", ImportString)]
	internal static partial void StopSource([JSMarshalAs<JSType.Any>] object source);
}
