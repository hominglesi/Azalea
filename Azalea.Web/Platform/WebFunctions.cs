using System;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Platform;
internal static partial class WebFunctions
{
	private const string ImportString = "JSImports";

	[JSImport("WebFunctions.GetCurrentPreciseTime", ImportString)]
	[return: JSMarshalAs<JSType.Date>]
	internal static partial DateTime GetCurrentPreciseTime();

	[JSImport("WebFunctions.SetTitle", ImportString)]
	internal static partial void SetTitle(string title);
}
