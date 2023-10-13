using Azalea.Debugging.BindableDisplays;
using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.UserInterface;
using System.Reflection;

namespace Azalea.Debugging;
public class DebugProperties : FlexContainer
{
	private GameObject _observedObject;

	private SpriteText _name;
	private BasicButton _highlightButton;

	private int _invalidationCounter;
	private SpriteText _invalidationCount;
	private SpriteText _latestInvalidation;

	private Composition _propertiesContainer;

	public DebugProperties()
	{
		Direction = FlexDirection.Vertical;
		Wrapping = FlexWrapping.NoWrapping;
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
			_invalidationCount = new SpriteText()
			{
				Color = Palette.Flowers.Azalea,
				Text = "0"
			},
			_latestInvalidation = new SpriteText()
			{
				Color = Palette.Flowers.Azalea,
			},
			_propertiesContainer = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				Width = 1,
				Direction = FlexDirection.Vertical,
				Wrapping = FlexWrapping.NoWrapping
			}
		});
	}

	public void SetObservedObject(GameObject obj)
	{
		if (_observedObject == obj) return;

		if (_observedObject is not null)
			_observedObject.Invalidated -= AddInvalidation;

		_observedObject = obj;

		_name.Text = obj.GetType().Name ?? "";
		_invalidationCounter = 0;
		_invalidationCount.Text = "0";
		_latestInvalidation.Text = "";
		_observedObject.Invalidated += AddInvalidation;

		loadAllProperties(obj);
	}

	private void loadAllProperties(GameObject obj)
	{
		_propertiesContainer.Clear();

		var properties = obj.GetType().GetProperties();

		foreach (var property in properties)
		{
			if (property.CanWrite == false || property.CanRead == false) continue;

			var propertyDisplay = getDisplayForProperty(property);
			if (propertyDisplay is null) continue;

			_propertiesContainer.Add(propertyDisplay);
		}
	}

	private GameObject? getDisplayForProperty(PropertyInfo property)
	{
		return property.PropertyType.Name switch
		{
			"String" => new DebugStringDisplay(_observedObject, property.Name),
			_ => null
		};
	}

	public void AddInvalidation(GameObject _, Invalidation invalidation)
	{
		_invalidationCounter++;
		_invalidationCount.Text = _invalidationCounter.ToString();
		_latestInvalidation.Text = invalidation.ToString();
	}
}
