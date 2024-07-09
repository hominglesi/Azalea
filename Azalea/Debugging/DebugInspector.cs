using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Debugging;
public class DebugInspector : FlexContainer
{
	private GameObject? _observedObject;

	private DebugProperties _properties;

	public DebugInspector()
	{
		Wrapping = FlexWrapping.NoWrapping;
		Direction = FlexDirection.Vertical;
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;
		AddRange(new GameObject[]
		{
			_properties = new DebugProperties()
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
