using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Stores;

public interface IResourceStore<T>
{
	T? Get(string name);

	Stream? GetStream(string name);

	IEnumerable<string> GetAvalibleResources();
}
