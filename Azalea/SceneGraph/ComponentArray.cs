using System.Collections;
using System.Collections.Generic;

namespace Azalea.SceneGraph;

public class ComponentArray<T> : IEnumerable<T>
	where T : struct
{
	private readonly List<T> _list = [];

	public void Add(T component) => _list.Add(component);

	public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
