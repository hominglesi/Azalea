using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Rendering;

public partial class WebGL
{
	[JSExport]
	public static void MainLoop()
	{
		//while(true) update loop
		DrawSquares("pink", "purple");
	}


	[JSImport("draw", "JSImports")]
	internal static partial void DrawSquares([JSMarshalAs<JSType.String>] string a, [JSMarshalAs<JSType.String>] string b);


}
