namespace Azalea.Extentions.ObjectExtentions;

public static class ObjectExtentions
{
	public static T AsNotNull<T>(this T? obj) => obj!;
}
