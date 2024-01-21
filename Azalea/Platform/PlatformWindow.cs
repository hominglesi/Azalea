using Azalea.Utils;
using System;

namespace Azalea.Platform;
internal abstract class PlatformWindow : Disposable
{
	public PlatformWindow(string title, Vector2Int clientSize, WindowState state)
	{
		_title = title;
		_clientSize = clientSize;
		_state = state;
	}

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

	private Vector2Int _size;
	protected abstract void SetSizeImplementation(Vector2Int size);
	public Vector2Int Size
	{
		get => _size;
		set
		{
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
			if (_clientSize == value) return;
			SetClientSizeImplementation(value);
		}
	}

	public Action<Vector2Int>? Resized { get; set; }

	protected void UpdateSize(Vector2Int size, Vector2Int clientSize)
	{
		_size = size;
		_clientSize = clientSize;
		Resized?.Invoke(clientSize);
	}

	private Vector2Int _position;
	protected abstract void SetPositionImplementation(Vector2Int position);
	public Vector2Int Position
	{
		get => _position;
		set
		{
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
			if (_clientPosition == value) return;
			SetClientPositionImplementation(value);
		}
	}

	protected void UpdatePosition(Vector2Int position, Vector2Int clientPosition)
	{
		_position = position;
		_clientPosition = clientPosition;
	}

	protected WindowState _state;
	protected abstract void MinimizeImplementation();
	protected abstract void MaximizeImplementation();
	protected abstract void RestoreImplementation();
	public WindowState State
	{
		get => _state;
		set
		{
			if (_state == value) return;

			switch (value)
			{
				case WindowState.Minimized:
					MinimizeImplementation();
					break;
				case WindowState.Maximized:
					MaximizeImplementation();
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
}
