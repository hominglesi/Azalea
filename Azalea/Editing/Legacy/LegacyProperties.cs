﻿using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Editing.Legacy.BindableDisplays;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using System;
using System.Reflection;

namespace Azalea.Editing.Legacy;
public class LegacyProperties : FlexContainer
{
	private GameObject _observedObject;

	private SpriteText _name;
	private BasicButton _highlightButton;

	private Composition _propertiesContainer;

	public LegacyProperties()
	{
		Direction = FlexDirection.Vertical;
		Wrapping = FlexWrapping.NoWrapping;
		AutoSizeAxes = Axes.Y;
		Spacing = new(0, 5);
		AddRange(new GameObject[]
		{
			new Composition()
			{
				RelativeSizeAxes = Axes.X,
				Size = new(1, 40),
				Child = _name = new SpriteText()
				{
					Anchor = Anchor.Center,
					Origin = Anchor.Center,
					Font = FontUsage.Default.With(size: 28)
				}
			},
			new Composition()
			{
				RelativeSizeAxes = Axes.X,
				Size = new(1, 35),
				Child = _highlightButton = new BasicButton()
				{
					Anchor = Anchor.Center,
					Origin = Anchor.Center,
					BackgroundColor = new Color(231, 92, 175),
					HoveredColor = Palette.Flowers.Azalea,
					TextColor = Palette.Black,
					FontSize = 24,
					RelativeSizeAxes = Axes.Both,
					Size = new(0.65f, 1),
					Text = "Highlight",
					Action = () => { if (_observedObject is not null) Editor.HighlightObject(_observedObject); }
				}
			},
			_propertiesContainer = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				Width = 1,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical,
				Wrapping = FlexWrapping.NoWrapping
			}
		});
	}

	public void SetObservedObject(GameObject obj)
	{
		if (_observedObject == obj) return;

		_observedObject = obj;

		_name.Text = obj.GetType().Name ?? "";

		loadAllProperties(obj);
	}

	private void loadAllProperties(GameObject obj)
	{
		_propertiesContainer.Clear();

		var properties = obj.GetType().GetProperties();

		foreach (var property in properties)
		{
			if (property.CanWrite == false || property.CanRead == false) continue;
			if (property.GetCustomAttribute<HideInInspector>() != null) continue;

			var propertyDisplay = getDisplayForProperty(property);
			if (propertyDisplay is null) continue;

			_propertiesContainer.Add(propertyDisplay);
		}
	}

	private GameObject? getDisplayForProperty(PropertyInfo property)
	{
		return property.PropertyType.Name switch
		{
			"String" => new LegacyStringDisplay(_observedObject, property.Name),
			"Single" => new LegacyFloatDisplay(_observedObject, property.Name),
			"Int32" => new LegacyIntDisplay(_observedObject, property.Name),
			"Vector2" => new LegacyVector2Display(_observedObject, property.Name),
			"Boundary" => new LegacyBoundaryDisplay(_observedObject, property.Name),
			_ => null
		};
	}
}

[AttributeUsage(AttributeTargets.Property)]
public class HideInInspector : Attribute { }
