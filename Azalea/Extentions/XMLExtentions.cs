using System;
using System.Collections.Generic;
using System.Xml;

namespace Azalea.Extentions;
public static class XMLExtentions
{
	public static bool ContainsAttribute(this XmlNode node, string attibuteName)
	{
		var attributes = node.Attributes
			?? throw new ArgumentException("This node does not have any attributes");

		return attributes[attibuteName] is not null;
	}

	public static string GetAttribute(this XmlNode node, string attributeName)
	{
		var attributes = node.Attributes
			?? throw new ArgumentException("This node does not have any attributes");
		var attribute = attributes[attributeName]
			?? throw new ArgumentException($"This node does not have an attribute named {attributeName}");
		return attribute.Value;
	}

	public static int GetIntAttribute(this XmlNode node, string attributeName)
	{
		var stringValue = node.GetAttribute(attributeName);

		if (int.TryParse(stringValue, out int result))
			return result;

		throw new ArgumentException("Attribute is not an integer");
	}

	public static float GetFloatAttribute(this XmlNode node, string attributeName)
	{
		var stringValue = node.GetAttribute(attributeName);

		if (float.TryParse(stringValue, out float result))
			return result;

		throw new ArgumentException("Attribute is not a float");
	}

	public static bool GetBoolAttribute(this XmlNode node, string attributeName)
	{
		var intValue = node.GetIntAttribute(attributeName);

		if (intValue == 0) return false;
		if (intValue == 1) return true;

		throw new ArgumentException("Attribute is not a bool");
	}

	public static IEnumerable<XmlNode> GetNodes(this XmlNode parentNode, string nodeName)
	{
		var nodes = parentNode.SelectNodes(nodeName);
		if (nodes is null)
			yield break;

		for (var i = 0; i < nodes.Count; i++)
		{
			yield return nodes[i]!;
		}
	}

	public static XmlNode GetNode(this XmlNode parentNode, string nodeName)
	{
		var node = parentNode.SelectSingleNode(nodeName)
			?? throw new ArgumentException($"Node '{nodeName}' not found");

		return node;
	}

	public static bool HasNode(this XmlNode parentNode, string nodeName)
	{
		var node = parentNode.SelectSingleNode(nodeName);

		return node is not null;
	}
}
