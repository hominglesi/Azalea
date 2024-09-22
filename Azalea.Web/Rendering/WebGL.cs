using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Rendering;

public partial class WebGL
{
	[JSImport("draw", "JSImports")]
	internal static partial void DrawSquares(string a, string b);
	[JSImport("clearColor", "JSImports")]
	internal static partial void SetClearColor(float r, float g, float b, float a);

	[JSImport("clearScreen", "JSImports")]
	internal static partial void ClearCanvas();
	
}
