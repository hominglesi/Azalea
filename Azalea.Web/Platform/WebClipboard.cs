using Azalea.Platform;
using System;

namespace Azalea.Web.Platform;
internal class WebClipboard : IClipboard
{
	public string? GetText()
	{
		Console.WriteLine("Accessing clipboard is not supported");

		return null;
	}

	public bool SetText(string text)
	{
		Console.WriteLine("Accessing clipboard is not supported");

		return false;
	}
}
