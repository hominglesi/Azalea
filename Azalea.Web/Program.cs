using Azalea.IO.Resources;
using Azalea.Web.Platform;

namespace Azalea.Web;
public class Program
{
	static void Main(string[] args)
	{
		Assets.AddToMainStore(new NamespacedResourceStore(new EmbeddedResourceStore(typeof(Program).Assembly), "Resources"));
		new WebHost().Run(new VisualTests.VisualTests());
	}
}
