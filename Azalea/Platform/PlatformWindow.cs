using Azalea.Graphics.Textures;
using Azalea.Utils;
using System;
using System.IO;

namespace Azalea.Platform;
internal abstract class PlatformWindow : Disposable, IWindow
{
	internal const string DEFAULT_TITLE = "Azalea Window";
	internal const int DEFAULT_CLIENT_WIDTH = 1280;
	internal const int DEFAULT_CLIENT_HEIGHT = 720;
	internal const WindowState DEFAULT_STATE = WindowState.Normal;
	internal const int DEFAULT_POSITION_X = 100;
	internal const int DEFAULT_POSITION_Y = 100;
	internal const bool DEFAULT_VISIBLE = true;
	internal const bool DEFAULT_RESIZABLE = false;
	internal const bool DEFAULT_DECORATED = true;
	internal const bool DEFAULT_TRANSPARENT_FRAMEBUFFER = false;
	internal const bool DEFAULT_VSYNC = false;
	internal const float DEFAULT_OPACITY = 1f;

	protected abstract void SetTitleImplementation(string title);
	protected string _title = DEFAULT_TITLE;
	public string Title
	{
		get => _title;
		set
		{
			if (value == _title) return;
			_title = value;
			SetTitleImplementation(value);
		}
	}

	public Action<Vector2Int>? Resized { get; set; }

	protected abstract void SetClientSizeImplementation(int width, int height);
	protected abstract Vector2Int GetFullscreenSize();
	protected int _clientWidth = DEFAULT_CLIENT_WIDTH;
	protected int _clientHeight = DEFAULT_CLIENT_HEIGHT;
	protected Vector2Int _clientSize
	{
		get => new(_clientWidth, _clientHeight);
		set
		{
			if (value.X == _clientWidth && value.Y == _clientHeight) return;

			_clientWidth = value.X;
			_clientHeight = value.Y;

			Resized?.Invoke(_clientSize);
		}
	}

	public Vector2Int ClientSize
	{
		get
		{
			if (_state == WindowState.BorderlessFullscreen) return GetFullscreenSize();
			else if (_state == WindowState.Minimized) return Vector2Int.Zero;
			else return new(_clientWidth, _clientHeight);
		}
		set
		{
			if (value == _clientSize) return;

			if (_state == WindowState.BorderlessFullscreen)
			{
				//Updating client size while fullscreen should only save it so we have it when leaving fullscreen
				_clientSize = value;
				return;
			}

			//GLFW has an onResize callback which will get called anyway so we don't need to update it manually
			//_clientSize = value;

			SetClientSizeImplementation(_clientWidth, _clientHeight);
		}
	}

	protected abstract void SetPositionImplementation(int x, int y);
	protected Vector2Int _position = new(DEFAULT_POSITION_X, DEFAULT_POSITION_Y);
	public Vector2Int Position
	{
		get
		{
			if (_state == WindowState.BorderlessFullscreen) return Vector2Int.Zero;
			else return _position;
		}
		set
		{
			if (value == _position) return;

			//GLFW has an onMove callback which will get called anyway so we don't need to update it manually
			//_position = value;

			SetPositionImplementation(value.X, value.Y);
		}
	}

	protected abstract void SetVisibleImplementation(bool visible);
	protected bool _visible = DEFAULT_VISIBLE;
	public bool Visible
	{
		get => _visible;
		set
		{
			if (value == _visible) return;
			_visible = value;
			SetVisibleImplementation(value);
		}
	}

	protected abstract void SetVSyncImplementation(bool enabled);
	private bool _vSync = DEFAULT_VSYNC;
	public bool VSync
	{
		get => _vSync;
		set
		{
			if (value == _vSync) return;
			_vSync = value;
			SetVSyncImplementation(value);
		}
	}

	protected abstract void SetResizableImplementation(bool enabled);
	protected bool _resizable = DEFAULT_RESIZABLE;
	public bool Resizable
	{
		get => _resizable;
		set
		{
			if (value == _resizable) return;
			_resizable = value;
			SetResizableImplementation(value);
		}
	}

	protected abstract void SetDecoratedImplementation(bool enabled);
	protected bool _decorated = DEFAULT_DECORATED;
	public bool Decorated
	{
		get => _decorated;
		set
		{
			if (value == _decorated) return;
			_decorated = value;
			SetDecoratedImplementation(value);
		}
	}

	protected abstract void SetOpacityImplementation(float opacity);
	protected float _opacity = DEFAULT_OPACITY;
	public float Opacity
	{
		get => _opacity;
		set
		{
			var clampedValue = Math.Clamp(value, 0, 1);

			if (clampedValue == _opacity) return;
			_opacity = clampedValue;
			SetOpacityImplementation(clampedValue);
		}
	}

	protected WindowState _state = DEFAULT_STATE;
	protected WindowState? _targetState;
	public WindowState State
	{
		get => _state;
		set
		{
			if (value == _state) return;

			_targetState = value;

			if (value == WindowState.BorderlessFullscreen)
			{
				FullscreenImplementation();
				Resized?.Invoke(GetFullscreenSize());
			}
			else if (value == WindowState.Minimized)
			{
				MinimizeImplementation();
			}
			else if (value == WindowState.Normal)
			{
				if (_state == WindowState.BorderlessFullscreen)
				{
					RestoreFullscreenImplementation(_position.X, _position.Y, _clientWidth, _clientHeight);
					Resized?.Invoke(_clientSize);
				}
			}

			_targetState = null;
			_state = value;
		}
	}
	protected abstract void FullscreenImplementation();
	protected abstract void RestoreFullscreenImplementation(int lastX, int lastY, int lastWidth, int lastHeight);
	protected abstract void MinimizeImplementation();

	protected abstract Vector2Int GetWorkareaSizeImplementation();
	public void Center()
	{
		var workareaSize = GetWorkareaSizeImplementation();
		Position = workareaSize / 2 - ClientSize / 2;
	}

	protected abstract void RequestAttentionImplementation();
	public void RequestAttention() => RequestAttentionImplementation();

	protected abstract void FocusImplementation();
	public void Focus() => FocusImplementation();

	protected abstract void SetIconImplementation(ITextureData? data);
	public void SetIconFromStream(Stream? imageStream)
	{
		var data = imageStream is null ? null : new TextureData(TextureData.LoadFromStream(imageStream));
		SetIconImplementation(data);
	}

	protected abstract void SwapBuffersImplementation();
	public void SwapBuffers() => SwapBuffersImplementation();

	public Action? Closing { get; set; }

	protected abstract void SetShouldCloseImplementation(bool shouldClose);
	protected bool _shouldClose;
	public bool ShouldClose
	{
		get => _shouldClose;
		set
		{
			if (value == _shouldClose) return;
			_shouldClose = value;
			SetShouldCloseImplementation(value);
		}
	}
	public void Close()
	{
		ShouldClose = true;
		Closing?.Invoke();
	}
	public void PreventClosure()
		=> ShouldClose = false;
}
