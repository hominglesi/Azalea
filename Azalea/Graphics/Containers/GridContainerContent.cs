using Azalea.Lists;

namespace Azalea.Graphics.Containers;

public class GridContainerContent : ObservableArray<ObservableArray<GameObject>>
{
	public GridContainerContent(GameObject[][] objects)
		: base(new ObservableArray<GameObject>[objects.Length])
	{
		for (int i = 0; i < objects.Length; i++)
		{
			if (objects[i] != null)
			{
				var observableArray = new ObservableArray<GameObject>(objects[i]);
				this[i] = observableArray;
			}
		}
	}

	public static implicit operator GridContainerContent?(GameObject[][] objects)
	{
		if (objects == null)
			return null;

		return new GridContainerContent(objects);
	}
}
