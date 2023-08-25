using Azalea.Inputs;

namespace Azalea.Web.Platform.Blazor;

public static class BlazorExtentions
{
	public static Keys ToAzaleaKey(string blazorKey)
	{
		if (blazorKey.Length == 4 && blazorKey.StartsWith("Key"))
		{
			var code = blazorKey[3] - 'A';
			return Keys.A + code;
		}

		return blazorKey switch
		{
			"ControlLeft" => Keys.ControlLeft,
			"ControlRight" => Keys.ControlRight,
			"ShiftLeft" => Keys.ShiftLeft,
			"ShiftRight" => Keys.ShiftRight,
			"Enter" => Keys.Enter,
			"Space" => Keys.Space,
			_ => Keys.Unknown,
		};
	}
}
