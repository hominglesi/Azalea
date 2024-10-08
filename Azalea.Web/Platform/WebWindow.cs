﻿using Azalea.Platform;
using System;
using System.IO;

namespace Azalea.Web.Platform;

public class WebWindow : IWindow
{
	public WebWindow()
	{
		WebEvents.ClientResized += (size) =>
		{
			_clientSize = size;
			OnClientResized?.Invoke(size);
		};
		WebEvents.CheckClientResized();

		WebEvents.WindowClosing += () =>
		{
			WebEvents.ShouldClose = true;
			Closing?.Invoke();
		};
	}

	private Vector2Int _clientSize;
	public Vector2Int ClientSize
	{
		get => _clientSize;
		set { return; }
	}

	public Action<Vector2Int>? OnClientResized { get; set; }

	public Vector2Int Size
	{
		get => Vector2Int.Zero;
		set { return; }
	}

	public Vector2Int Position
	{
		get => Vector2Int.Zero;
		set { return; }
	}

	public Vector2Int ClientPosition
	{
		get => Vector2Int.Zero;
		set { return; }
	}

	public WindowState State
	{
		get => WindowState.Normal;
		set { return; }
	}

	private string _title = "AzaleaGame";
	public string Title
	{
		get => _title;
		set
		{
			if (value == _title) return;

			_title = value;

			WebFunctions.SetTitle(_title);
		}
	}

	public bool Resizable
	{
		get => true;
		set { return; }
	}

	public bool VSync
	{
		get => true;
		set { return; }
	}

	public bool CanChangeVSync => false;

	public bool CursorVisible
	{
		get => true;
		set { return; }
	}

	public Action? Closing { get; set; }

	public void Center() { }

	public void Close() { }

	public void Dispose() { }

	public void Focus() { }

	public void PreventClosure()
		=> WebEvents.ShouldClose = false;

	public void RequestAttention() { }

	public void SetIconFromStream(Stream? imageStream) { }

	void IWindow.SwapBuffers() { }

	void IWindow.Show(bool firstTime) { }

	void IWindow.Hide() { }

	void IWindow.ProcessEvents() => WebInput.HandleEvents();
}
