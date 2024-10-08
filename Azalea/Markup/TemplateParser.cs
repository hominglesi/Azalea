using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.IO.Resources;
using System;
using System.Diagnostics;
using System.Xml;

namespace Azalea.Markup;
public static class TemplateConverter
{
	public static GameObject Parse(string xmlString)
	{
		var xml = new XmlDocument();
		xml.LoadXml(xmlString);

		var rootElement = xml.DocumentElement;
		Debug.Assert(rootElement is not null);

		var rootObject = parseGameObjectNode(rootElement);

		return rootObject;
	}

	private static GameObject parseGameObjectNode(XmlNode node)
	{
		var objectType = Type.GetType(node.Name, true)!;
		var nodeObject = (GameObject)Activator.CreateInstance(objectType)!;

		foreach (XmlAttribute nodeAttribute in node.Attributes!)
		{
			var nodeProperty = objectType.GetProperty(nodeAttribute.Name);
			Debug.Assert(nodeProperty is not null);

			switch (nodeProperty.PropertyType.Name)
			{
				case "Single":
					var singleValue = float.Parse(nodeAttribute.InnerText);
					nodeProperty.SetValue(nodeObject, singleValue);
					break;
				case "Texture":
					var textureValue = Assets.GetTexture(nodeAttribute.InnerText);
					nodeProperty.SetValue(nodeObject, textureValue);
					break;
				case "ColorQuad":
					var colorValue = new ColorQuad(Color.FromHex(nodeAttribute.InnerText));
					nodeProperty.SetValue(nodeObject, colorValue);
					break;
				case "Axes":
					var axesValue = Enum.Parse(nodeProperty.PropertyType, nodeAttribute.InnerText);
					nodeProperty.SetValue(nodeObject, axesValue);
					break;
			}
		}

		if (nodeObject is Composition compositeObject)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				var childObject = parseGameObjectNode(childNode);
				compositeObject.Add(childObject);
			}
		}

		return nodeObject;
	}
}
