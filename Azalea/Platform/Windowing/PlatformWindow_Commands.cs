using Azalea.Graphics;
using Azalea.Threading;
using System;

namespace Azalea.Platform.Windowing;

public abstract class WindowCommand : ThreadCommand
{
	internal static volatile new int TotalCreated = 0;

	internal WindowCommand()
	{
		TotalCreated++;
	}
}

public abstract partial class PlatformWindow
{
	[ThreadCommand]
	internal partial class SetTitleCommand : WindowCommand
	{
		public string Title;
	}

	public string Title
	{
		get;
		set
		{
			if (field == value) return;

			field = value;
			Enqueue(SetTitleCommand.Borrow(value));
		}
	} = "Azalea Window";

	[ThreadCommand]
	internal partial class SetPositionCommand : WindowCommand
	{
		public Vector2Int Position;
	}

	protected Vector2Int _position;
	public Vector2Int Position
	{
		get => _position;
		set
		{
			if (_position == value) return;

			_clientPosition = value + (_clientPosition - _position);
			_position = value;
			Enqueue(SetPositionCommand.Borrow(value));
		}
	}


	[ThreadCommand]
	internal partial class SetClientPositionCommand : WindowCommand
	{
		public Vector2Int ClientPosition;
	}

	protected Vector2Int _clientPosition = new(100, 100);
	public Vector2Int ClientPosition
	{
		get => _clientPosition;
		set
		{
			if (_clientPosition == value) return;

			_position = value - (_clientPosition - _position);
			_clientPosition = value;
			Enqueue(SetClientPositionCommand.Borrow(value));
		}
	}

	[ThreadCommand]
	internal partial class SetSizeCommand : WindowCommand
	{
		public Vector2Int Size;
	}

	protected Vector2Int _size;
	public Vector2Int Size
	{
		get => _size;
		set
		{
			if (_size == value) return;

			_size = value;
			Enqueue(SetSizeCommand.Borrow(value));
		}
	}

	[ThreadCommand]
	internal partial class SetClientSizeCommand : WindowCommand
	{
		public Vector2Int ClientSize;
	}

	protected Vector2Int _clientSize;
	public event Action<Vector2Int>? OnResized;
	protected void InvokeOnResized() => OnResized?.Invoke(_clientSize);
	public Vector2Int ClientSize
	{
		get => _clientSize;
		set
		{
			if (_clientSize == value) return;

			_clientSize = value;
			Enqueue(SetClientSizeCommand.Borrow(value));
		}
	}

	[ThreadCommand]
	internal partial class SetResizableCommand : WindowCommand
	{
		public bool Resizable;
	}

	public bool Resizable
	{
		get;
		set
		{
			if (field == value) return;

			field = value;
			Enqueue(SetResizableCommand.Borrow(value));
		}
	} = true;

	[ThreadCommand]
	internal partial class SetCursorVisibleCommand : WindowCommand
	{
		public bool IsVisible;
	}

	public bool CursorVisible
	{
		get;
		set
		{
			if (field == value) return;

			field = value;
			Enqueue(SetCursorVisibleCommand.Borrow(value));
		}
	} = true;

	[ThreadCommand]
	internal partial class ShowCommand : WindowCommand;
	public void Show() => Enqueue(ShowCommand.Borrow());

	[ThreadCommand]
	internal partial class HideCommand : WindowCommand;
	public void Hide() => Enqueue(HideCommand.Borrow());

	[ThreadCommand]
	internal partial class FocusCommand : WindowCommand;
	public void Focus() => Enqueue(FocusCommand.Borrow());

	[ThreadCommand]
	internal partial class CenterCommand : WindowCommand { }
	public void Center() => Enqueue(CenterCommand.Borrow());

	[ThreadCommand]
	internal partial class FullscreenCommand : WindowCommand;
	public void Fullscreen() => Enqueue(FullscreenCommand.Borrow());

	[ThreadCommand]
	internal partial class MaximizeCommand : WindowCommand;
	public void Maximize() => Enqueue(MaximizeCommand.Borrow());

	[ThreadCommand]
	internal partial class MinimizeCommand : WindowCommand;
	public void Minimize() => Enqueue(MinimizeCommand.Borrow());

	[ThreadCommand]
	internal partial class RestoreCommand : WindowCommand;
	public void Restore() => Enqueue(RestoreCommand.Borrow());

	[ThreadCommand]
	internal partial class RequestAttentionCommand : WindowCommand;
	public void RequestAttention() => Enqueue(RequestAttentionCommand.Borrow());

	[ThreadCommand]
	internal partial class SetIconCommand : WindowCommand
	{
		public Image? Image;
	}
	public void SetIcon(Image? image) => Enqueue(SetIconCommand.Borrow(image));

	[ThreadCommand]
	internal partial class CreateTrayIconCommand : WindowCommand
	{
		public TrayIcon TrayIcon;
		public string Title;
		public Image Icon;
	}

	public TrayIcon CreateTrayIcon(string title, Image icon)
	{
		var trayIcon = new TrayIcon();
		Enqueue(CreateTrayIconCommand.Borrow(trayIcon, title, icon));
		return trayIcon;
	}

	[ThreadCommand]
	internal partial class DeleteTrayIconCommand : WindowCommand
	{
		public TrayIcon TrayIcon;
	}

	public void DeleteTrayIcon(TrayIcon icon) => Enqueue(DeleteTrayIconCommand.Borrow(icon));
}
