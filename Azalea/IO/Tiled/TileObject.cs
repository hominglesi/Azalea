using Azalea.Extentions;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;

namespace Azalea.IO.Tiled;
public readonly struct TileObject
{
	public int Id { get; init; }
	public int TileId { get; init; }
	public string Template { get; init; }
	public string Name { get; init; }
	public bool Visible { get; init; }
	public float X { get; init; }
	public float Y { get; init; }
	public float Width { get; init; }
	public float Height { get; init; }
	public float Rotation { get; init; }
	public Dictionary<string, string> Properties { get; init; }

	public Vector2 Position => new(X, Y);
	public Vector2 Size => new(Width, Height);

	internal static TileObject Parse(XmlNode objectNode)
	{
		int id = 0;
		if (objectNode.ContainsAttribute("id"))
			id = objectNode.GetIntAttribute("id");

		int tileId = -1;
		if (objectNode.ContainsAttribute("gid"))
			tileId = objectNode.GetIntAttribute("gid");

		float x = 0;
		if (objectNode.ContainsAttribute("x"))
			x = objectNode.GetFloatAttribute("x");

		float y = 0f;
		if (objectNode.ContainsAttribute("y"))
			y = objectNode.GetFloatAttribute("y");

		string template = "";
		if (objectNode.ContainsAttribute("template"))
			template = objectNode.GetAttribute("template");

		string name = "";
		if (objectNode.ContainsAttribute("name"))
			name = objectNode.GetAttribute("name");

		bool visible = true;
		if (objectNode.ContainsAttribute("visible"))
			visible = objectNode.GetBoolAttribute("visible");

		float width = 0;
		if (objectNode.ContainsAttribute("width"))
			width = objectNode.GetFloatAttribute("width");

		float height = 0;
		if (objectNode.ContainsAttribute("height"))
			height = objectNode.GetFloatAttribute("height");

		float rotation = 0;
		if (objectNode.ContainsAttribute("rotation"))
			rotation = objectNode.GetFloatAttribute("rotation");

		Dictionary<string, string> properties = new();

		if (objectNode.HasNode("properties"))
		{
			var propertyNodes = objectNode.GetNode("properties").GetNodes("property");
			foreach (var propertyNode in propertyNodes)
			{
				var propertyName = propertyNode.GetAttribute("name");
				var propertyValue = propertyNode.GetAttribute("value");
				properties[propertyName] = propertyValue;
			}
		}

		return new TileObject()
		{
			Id = id,
			TileId = tileId,
			Template = template,
			Name = name,
			Visible = visible,
			X = x,
			Y = y,
			Width = width,
			Height = height,
			Rotation = rotation,
			Properties = properties
		};
	}

	public static TileObject ImplementTemplate(TileObject obj, TileObject template)
	{
		var properties = new Dictionary<string, string>();
		foreach (var prop in template.Properties)
		{
			properties[prop.Key] = prop.Value;
		}

		foreach (var prop in obj.Properties)
		{
			properties[prop.Key] = prop.Value;
		}

		return new TileObject()
		{
			Id = obj.Id,
			TileId = obj.TileId == -1 ? template.TileId : obj.TileId,
			Template = obj.Template,
			Name = obj.Name == "" ? template.Name : obj.Name,
			Visible = obj.Visible,
			X = obj.X,
			Y = obj.Y,
			Width = obj.Width == 0 ? template.Width : obj.Width,
			Height = obj.Height == 0 ? template.Height : obj.Height,
			Rotation = obj.Rotation == 0 ? template.Rotation : obj.Rotation,
			Properties = properties
		};
	}
}
