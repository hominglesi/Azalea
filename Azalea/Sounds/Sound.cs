using Azalea.IO.Resources;
using System.IO;

namespace Azalea.Sounds;
public class Sound
{
	private readonly IResourceStore _containingStore;
	private readonly string _resourceName;

	internal Sound(IResourceStore store, string name)
	{
		_containingStore = store;
		_resourceName = name;
	}

	internal Stream? GetStream() => _containingStore.GetStream(_resourceName);
}
