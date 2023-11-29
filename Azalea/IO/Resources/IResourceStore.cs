using System.Collections.Generic;
using System.IO;

namespace Azalea.IO.Resources;
public interface IResourceStore
{
	Stream? GetStream(string path);

	IEnumerable<string> GetAvalibleResources();
}
