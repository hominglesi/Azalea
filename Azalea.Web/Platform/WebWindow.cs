using Azalea.Platform;
using System;
using System.IO;

namespace Azalea.Web.Platform;

public class WebWindow : IWindow
{
	public Vector2Int Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public Vector2Int ClientSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public Vector2Int Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public Vector2Int ClientPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public WindowState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public bool Resizable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public bool VSync { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public bool CanChangeVSync => throw new NotImplementedException();

	public bool CursorVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public Action? Closing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	Vector2Int IWindow.Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	Vector2Int IWindow.ClientSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	Action<Vector2Int>? IWindow.OnClientResized { get; set; }
	Vector2Int IWindow.Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	Vector2Int IWindow.ClientPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	WindowState IWindow.State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	string IWindow.Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	bool IWindow.Resizable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	bool IWindow.VSync { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	bool IWindow.CanChangeVSync => throw new NotImplementedException();

	bool IWindow.CursorVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	bool IWindow.ShouldClose => throw new NotImplementedException();

	Action? IWindow.Closing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public void Center()
	{
		throw new NotImplementedException();
	}

	public void Close()
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public void Focus()
	{
		throw new NotImplementedException();
	}

	public void PreventClosure()
	{
		throw new NotImplementedException();
	}

	public void RequestAttention()
	{
		throw new NotImplementedException();
	}

	public void SetIconFromStream(Stream? imageStream)
	{
		throw new NotImplementedException();
	}

	void IWindow.Center()
	{
		throw new NotImplementedException();
	}

	void IWindow.Close()
	{
		throw new NotImplementedException();
	}

	void IDisposable.Dispose()
	{
		throw new NotImplementedException();
	}

	void IWindow.Focus()
	{
		throw new NotImplementedException();
	}

	void IWindow.Hide()
	{
		throw new NotImplementedException();
	}

	void IWindow.PreventClosure()
	{
		throw new NotImplementedException();
	}

	void IWindow.ProcessEvents()
	{
		throw new NotImplementedException();
	}

	void IWindow.RequestAttention()
	{
		throw new NotImplementedException();
	}

	void IWindow.SetIconFromStream(Stream? imageStream)
	{
		throw new NotImplementedException();
	}

	void IWindow.Show(bool firstTime)
	{
		throw new NotImplementedException();
	}

	void IWindow.SwapBuffers()
	{
		throw new NotImplementedException();
	}
}
