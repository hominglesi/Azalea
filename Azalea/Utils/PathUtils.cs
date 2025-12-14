using System;

namespace Azalea.Utils;
public static class PathUtils
{
	public static string GetDirectoryFromPath(string path)
		=> GetDirectoryFromPath(path.AsSpan()).ToString();

	public static ReadOnlySpan<char> GetDirectoryFromPath(ReadOnlySpan<char> path)
	{
		if (path.Contains('/') == false)
			return ReadOnlySpan<char>.Empty;

		var lastSlash = path.LastIndexOf('/');

		return path[..(lastSlash + 1)]; ;
	}

	public static string GetFileFromPath(string path)
		=> GetFileFromPath(path.AsSpan()).ToString();

	public static ReadOnlySpan<char> GetFileFromPath(ReadOnlySpan<char> path)
	{
		if (path.Contains('/') == false)
			return path;

		var lastSlash = path.LastIndexOf('/');

		return path[(lastSlash + 1)..];
	}

	public static string GetResourcePathFromAssemblyPath(string path, string prefix = "")
	{
		// We need to convert 
		//    Textures/Backgrounds/night-sky.png
		// to Assembly.Textures.Backgrounds.night-sky.png

		Span<char> span = stackalloc char[prefix.Length + 1 + path.Length];

		for (int i = 0; i < prefix.Length; i++)
			span[i] = prefix[i];

		span[prefix.Length] = '.';

		for (int i = 0; i < path.Length; i++)
		{
			var chr = path[i];

			if (chr == '/')
				chr = '.';

			span[prefix.Length + 1 + i] = chr;
		}

		return span.ToString();
	}

	public static string GetDirectoryPathFromResourcePath(string path, string prefix = "")
	{
		// We need to convert 
		//    Assembly.Texture.Backgrounds.night-sky.png
		// to Textures/Backgrounds/night-sky.png

		var startOffset = 0;

		if (path.StartsWith(prefix))
			startOffset = prefix.Length + 1;

		Span<char> span = stackalloc char[path.Length - startOffset];

		var lastDot = path.LastIndexOf('.') - startOffset;

		for (int i = 0; i < span.Length; i++)
		{
			var chr = path[i + startOffset];

			if (chr == '.' && i != lastDot)
				chr = '/';

			span[i] = chr;
		}

		return span.ToString();
	}

	public static string CombinePaths(string path1, string path2)
	{
		path1 = path1.Replace("\\", "/");
		path2 = path2.Replace("\\", "/");

		if (path1.Length > 0 && path1.EndsWith('/') == false)
			path1 += '/';

		if (path2.StartsWith('/'))
			path2 = path2[1..];

		var joined = path1 + path2;

		while (joined.Contains("/.."))
		{
			var backspaceIndex = joined.IndexOf("/..");
			var previousIndex = -1;

			int i = backspaceIndex;
			while (i > 0)
			{
				i--;
				if (joined[i] == '/')
				{
					previousIndex = i;
					break;
				}
			}

			var pathEnd = joined[(backspaceIndex + 3)..];

			var pathStart = "";
			if (previousIndex != -1)
			{
				pathStart = joined[..previousIndex];

			}
			else
			{
				pathEnd = pathEnd[1..];
			}

			joined = pathStart + pathEnd;
		}

		return joined;
	}
}
