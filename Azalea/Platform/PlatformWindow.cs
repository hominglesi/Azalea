using Azalea.Graphics;
using Azalea.Utils;
using System;
using System.IO;

namespace Azalea.Platform;
internal abstract class PlatformWindow : Disposable, IWindow
{
	public PlatformWindow(string title, Vector2Int clientSize, WindowState state)
	{
		_title = title;
		_clientSize = clientSize;
		_state = state;
	}

	#region Size

	private Vector2Int _size;
	protected abstract void SetSizeImplementation(Vector2Int size);
	public Vector2Int Size
	{
		get => _size;
		set
		{
			if (State == WindowState.Fullscreen || State == WindowState.Minimized)
			{
				Console.WriteLine($"Cannot change size while state is {State}");
				return;
			}

			if (_size == value) return;
			SetSizeImplementation(value);
		}
	}

	private Vector2Int _clientSize;
	protected abstract void SetClientSizeImplementation(Vector2Int clientSize);
	public Vector2Int ClientSize
	{
		get => _clientSize;
		set
		{
			if (State == WindowState.Fullscreen || State == WindowState.Minimized)
			{
				Console.WriteLine($"Cannot change client size while state is {State}");
				return;
			}

			if (_clientSize == value) return;
			SetClientSizeImplementation(value);
		}
	}

	public Action<Vector2Int>? OnClientResized { get; set; }

	protected void UpdateSize(Vector2Int size, Vector2Int clientSize)
	{
		_size = size;
		_clientSize = clientSize;
		OnClientResized?.Invoke(clientSize);
	}

	#endregion Size

	#region Position

	private Vector2Int _position;
	protected abstract void SetPositionImplementation(Vector2Int position);
	public Vector2Int Position
	{
		get => _position;
		set
		{
			if (State == WindowState.Fullscreen || State == WindowState.Minimized)
			{
				Console.WriteLine($"Cannot change position while state is {State}");
				return;
			}

			if (_position == value) return;
			SetPositionImplementation(value);
		}
	}

	private Vector2Int _clientPosition = Vector2Int.One;
	protected abstract void SetClientPositionImplementation(Vector2Int clientPosition);
	public Vector2Int ClientPosition
	{
		get => _clientPosition;
		set
		{
			if (State == WindowState.Fullscreen || State == WindowState.Minimized)
			{
				Console.WriteLine($"Cannot change client position while state is {State}");
				return;
			}

			if (_clientPosition == value) return;
			SetClientPositionImplementation(value);
		}
	}

	protected void UpdatePosition(Vector2Int position, Vector2Int clientPosition)
	{
		_position = position;
		_clientPosition = clientPosition;
	}

	#endregion Position

	#region State

	protected WindowState _state;
	private Vector2Int _preFullscreenPosition;
	private Vector2Int _preFullscreenSize;
	protected abstract void MinimizeImplementation();
	protected abstract void MaximizeImplementation();
	protected abstract void RestoreImplementation();
	protected abstract void FullscreenImplementation();
	protected abstract void RestoreFullscreenImplementation(Vector2Int lastPosition, Vector2Int lastSize);
	public WindowState State
	{
		get => _state;
		set
		{
			if (_state == value) return;

			if (_state == WindowState.Fullscreen && value != WindowState.Minimized)
				RestoreFullscreenImplementation(_preFullscreenPosition, _preFullscreenSize);

			switch (value)
			{
				case WindowState.Minimized:
					MinimizeImplementation();
					break;
				case WindowState.Maximized:
					MaximizeImplementation();
					break;
				case WindowState.Fullscreen:
					_preFullscreenPosition = Position;
					_preFullscreenSize = Size;
					FullscreenImplementation();
					break;
				default:
					RestoreImplementation();
					break;
			}
		}
	}

	protected void UpdateState(WindowState state)
	{
		_state = state;
	}

	#endregion State

	#region OtherProperties

	private string _title;
	protected abstract void SetTitleImplementation(string title);
	public string Title
	{
		get => _title;
		set
		{
			if (_title == value) return;
			SetTitleImplementation(value);
			_title = value;
		}
	}

	private bool _resizable = true;
	protected abstract void SetResizableImplementation(bool enabled);
	public bool Resizable
	{
		get => _resizable;
		set
		{
			if (_resizable == value) return;
			SetResizableImplementation(value);
			_resizable = value;
		}
	}

	private bool _vSync = false;
	protected abstract void SetVSyncImplementation(bool enabled);
	public bool VSync
	{
		get => _vSync;
		set
		{
			if (_vSync == value) return;
			SetVSyncImplementation(value);
			_vSync = value;
		}
	}

	private bool _cursorVisible = true;
	protected abstract void SetCursorVisible(bool show);
	public bool CursorVisible
	{
		get => _cursorVisible;
		set
		{
			if (_cursorVisible == value) return;
			SetCursorVisible(value);
			_cursorVisible = value;
		}
	}

	#endregion

	#region Methods

	public abstract void Center();

	public abstract void Focus();

	public abstract void RequestAttention();

	protected abstract void SetIconImplementation(Image? data);
	public void SetIconFromStream(Stream? imageStream)
	{
		var data = imageStream is null ? null : Image.FromStream(imageStream);
		SetIconImplementation(data);
	}

	public abstract void SwapBuffers();

	public abstract void Show(bool firstTime);

	public abstract void Hide();

	public abstract void ProcessEvents();

	#endregion

	#region Closing

	private bool _shouldClose;
	public bool ShouldClose
	{
		get => _shouldClose;
		set
		{
			if (value == _shouldClose) return;
			_shouldClose = value;
		}
	}

	public Action? Closing { get; set; }

	public void Close()
	{
		ShouldClose = true;
		Closing?.Invoke();
	}

	public void PreventClosure()
		=> ShouldClose = false;

	#endregion
}
