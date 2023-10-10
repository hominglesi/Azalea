using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Debugging;
public class DebugInspector : Composition
{
	private GameObject? _observedObject;

	private DebugProperties _properties;
	private DebugConsole _console;

	public DebugInspector()
	{
		Padding = new Boundary(5);
		AddRange(new GameObject[]
		{
			_properties = new DebugProperties()
			{
				RelativeSizeAxes = Axes.Both,
				Size = new(1, 0.6f)
			},
			_console = new DebugConsole()
			{
				RelativeSizeAxes = Axes.Both,
				Size = new(1, 0.4f),
				RelativePositionAxes = Axes.Both,
				Position = new(0, 0.6f)
			}
		});
	}

	public void SetObservedObject(GameObject obj)
	{
		_properties.SetObservedObject(obj);
	}
}
