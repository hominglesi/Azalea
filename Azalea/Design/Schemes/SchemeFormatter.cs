using Azalea.Graphics;
using System;
using System.Collections.Generic;

namespace Azalea.Design.Schemes;
public class SchemeFormatter
{
	private Dictionary<string, SchemeDelegate> _schemes = new();

	public void AddScheme(string tag, SchemeDelegate formatFunction)
	{
		_schemes.Add(tag, formatFunction);
	}

	public GameObject Format(string xml)
	{
		List<(string, int, List<object>)> openedTags = new();

		var tagStartIndex = -1;

		for (int i = 0; i <= xml.Length; i++)
		{
			if (xml[i] == '<')
			{
				if (tagStartIndex != -1) throw new Exception($"Unexpected character '<' at {i}");

				tagStartIndex = i + 1;
			}
			else if (xml[i] == '>')
			{
				if (tagStartIndex == -1) throw new Exception($"Unexpected character '>' at {i}");

				if (xml[tagStartIndex] == '/')
				{
					var tag = xml.Substring(tagStartIndex + 1, i - tagStartIndex - 1);

					if (openedTags[^1].Item1 != tag) throw new Exception($"Unexpected closing tag '{tag}' at {i}");

					var content = openedTags[^1].Item3;
					content.Add(xml.Substring(openedTags[^1].Item2, tagStartIndex - 1 - openedTags[^1].Item2));

					var obj = _schemes[tag].Invoke(content);

					if (openedTags.Count == 1)
						return obj;

					openedTags.RemoveAt(openedTags.Count - 1);
					openedTags[^1].Item3.Add(obj);
				}
				else if (xml[i - 1] == '/')
				{
					var tag = xml.Substring(tagStartIndex, i - tagStartIndex - 1).Trim();

					var content = new List<object>();

					var obj = _schemes[tag].Invoke(content);

					if (openedTags.Count == 1)
						return obj;

					openedTags[^1].Item3.Add(obj);
				}
				else
				{
					var tag = xml.Substring(tagStartIndex, i - tagStartIndex);

					openedTags.Add((tag, i + 1, new List<object>()));
				}

				tagStartIndex = -1;
			}
		}

		throw new Exception("Format error.");
	}

	public delegate GameObject SchemeDelegate(List<object> content);
}
