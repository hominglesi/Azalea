using Azalea.Graphics;
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
internal partial class CenterCommand : WindowCommand { }

[ThreadCommand(generateHandler: false)]
internal partial class CreateTrayIconCommand : WindowCommand
{
	public TrayIcon TrayIcon;
	public string Title;
	public Image Icon;
}

internal static class CreateTrayIconCommand_Handler
{
	internal static TrayIcon CreateTrayIcon(this ICommandHandler<WindowCommand> handler, string title, Image icon)
	{
		var trayIcon = new TrayIcon();
		handler.Enqueue(CreateTrayIconCommand.Borrow(trayIcon, title, icon));
		return trayIcon;
	}
}

[ThreadCommand()]
internal partial class DeleteTrayIconCommand : WindowCommand
{
	public TrayIcon TrayIcon;
}

[ThreadCommand]
internal partial class FocusCommand : WindowCommand { }

[ThreadCommand]
internal partial class FullscreenCommand : WindowCommand { }

[ThreadCommand]
internal partial class HideCommand : WindowCommand { }

[ThreadCommand]
internal partial class MaximizeCommand : WindowCommand { }

[ThreadCommand]
internal partial class MinimizeCommand : WindowCommand { }

[ThreadCommand]
internal partial class RequestAttentionCommand : WindowCommand { }

[ThreadCommand]
internal partial class RestoreCommand : WindowCommand { }

[ThreadCommand]
internal partial class SetClientPositionCommand : WindowCommand
{
	public Vector2Int ClientPosition;
}

[ThreadCommand]
internal partial class SetClientSizeCommand : WindowCommand
{
	public Vector2Int ClientSize;
}

[ThreadCommand]
internal partial class SetCursorVisibleCommand : WindowCommand
{
	public bool IsVisible;
}

[ThreadCommand]
internal partial class SetIconCommand : WindowCommand
{
	public Image? Image;
}

[ThreadCommand]
internal partial class SetPositionCommand : WindowCommand
{
	public Vector2Int Position;
}

[ThreadCommand]
internal partial class SetResizableCommand : WindowCommand
{
	public bool Resizable;
}

[ThreadCommand]
internal partial class SetSizeCommand : WindowCommand
{
	public Vector2Int Size;
}

[ThreadCommand]
internal partial class SetTitleCommand : WindowCommand
{
	public string Title;
}

[ThreadCommand]
internal partial class ShowCommand : WindowCommand { }
