using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editing.Legacy;
public class LegacyInspector : FlexContainer
{
	private GameObject? _observedObject;

	private LegacySelectPointer _selectPointer;
	private LegacyProperties _properties;

	public LegacyInspector()
	{
		Wrapping = FlexWrapping.NoWrapping;
		Direction = FlexDirection.Vertical;
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;
		AddRange(new GameObject[]
		{
			_selectPointer = new LegacySelectPointer(),
			_properties = new LegacyProperties()
			{
				RelativeSizeAxes = Axes.X,
			}
		});
	}

	public void SetObservedObject(GameObject obj)
	{
		_properties.SetObservedObject(obj);
	}
}
