namespace Azalea.Utils;
public static class FileSystemUtils
{
	public static string CombinePaths(string path1, string path2)
	{
		path1 = path1.Replace("\\", "/");
		path2 = path2.Replace("\\", "/");

		if (path1.EndsWith('/') == false)
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
