using Azalea.Threading;

namespace Azalea.Platform.Windowing;

public abstract class WindowCommand : ThreadCommand
{
	internal static volatile new int TotalCreated = 0;

	internal WindowCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand]
internal partial class HideCommand : WindowCommand { }

[ThreadCommand]
internal partial class MaximizeCommand : WindowCommand { }

[ThreadCommand]
internal partial class MinimizeCommand : WindowCommand { }

[ThreadCommand]
internal partial class RestoreCommand : WindowCommand { }

[ThreadCommand]
internal partial class SetTitleCommand : WindowCommand
{
	public string Title;
}

[ThreadCommand]
internal partial class ShowCommand : WindowCommand { }
