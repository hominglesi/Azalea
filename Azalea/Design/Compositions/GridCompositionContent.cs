using Azalea.Graphics;
using Azalea.Lists;

namespace Azalea.Design.Compositions;

public class GridCompositionContent : ObservableArray<ObservableArray<GameObject>>
{
	public GridCompositionContent(GameObject[][] objects)
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

	public static implicit operator GridCompositionContent?(GameObject[][] objects)
	{
		if (objects == null)
			return null;

		return new GridCompositionContent(objects);
	}
}
