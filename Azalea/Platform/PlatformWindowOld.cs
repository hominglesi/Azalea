﻿using System;

namespace Azalea.Platform;
internal abstract class PlatformWindowOld : PlatformWindow, IWindow
{
	internal const bool DEFAULT_VISIBLE = false;
	internal const bool DEFAULT_RESIZABLE = false;
	internal const bool DEFAULT_DECORATED = true;
	internal const bool DEFAULT_TRANSPARENT_FRAMEBUFFER = false;
	internal const bool DEFAULT_VSYNC = false;

	public PlatformWindowOld(string title, Vector2Int clientSize, WindowState state) : base(title, clientSize, state)
	{

	}


	protected abstract Vector2Int GetFullscreenSize();
	/*
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

			_clientSize = value;

			//Updating client size while fullscreen should only save it so we have it when leaving fullscreen
			if (_state == WindowState.BorderlessFullscreen) return;

			//GLFW has an onResize callback which will get called anyway so we don't need to update it manually
			//_clientSize = value;

			SetClientSizeImplementation(_clientWidth, _clientHeight);
		}
	}*/

	/*
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
	}*/

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

	/*
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
					RestoreFullscreenImplementation(Position.X, Position.Y, ClientSize.X, ClientSize.Y);
					Resized?.Invoke(ClientSize);
				}
			}

			_targetState = null;
			_state = value;
		}
	}*/

	/*
	protected abstract Vector2Int GetWorkareaSizeImplementation();
	public void Center()
	{
		var workareaSize = GetWorkareaSizeImplementation();
		Position = workareaSize / 2 - Size / 2;
	}*/

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

	public abstract void ProcessEvents();

	public void Close()
	{
		ShouldClose = true;
		Closing?.Invoke();
	}
	public void PreventClosure()
		=> ShouldClose = false;
}