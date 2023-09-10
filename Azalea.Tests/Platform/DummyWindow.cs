using Azalea.Platform;
using System.IO;

namespace Azalea.Tests.Platform;

public class DummyWindow : IWindow
{
	public string Title { get => "Test title"; set { } }
	public Vector2Int ClientSize { get => new(1280, 720); set { } }
	public WindowState State { get => WindowState.Normal; set { } }
	public bool Resizable { get => false; set { } }
	public bool CursorVisible { get => true; set { } }

	public void Close()
	{

	}

	public void SetIconFromStream(Stream imageStream)
	{

	}
}
