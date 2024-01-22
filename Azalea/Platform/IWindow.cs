using System;
using System.IO;

namespace Azalea.Platform;

public interface IWindow : IDisposable
{
	/// <summary>
	/// The size of the window on the users desktop
	/// </summary>
	Vector2Int Size { get; set; }

	/// <summary>
	/// The size of the window, excluding the title bar and border.
	/// </summary>
	Vector2Int ClientSize { get; set; }

	internal Action<Vector2Int>? OnClientResized { get; set; }

	/// <summary>
	/// The position of this window on the users desktop
	/// </summary>
	public Vector2Int Position { get; set; }

	/// <summary>
	/// The position of the window, excluding the title bar and border.
	/// </summary>
	Vector2Int ClientPosition { get; set; }

	/// <summary>
	/// Controls the state of the window.
	/// For possible states see <seealso cref="WindowState"/>.
	/// </summary>
	WindowState State { get; set; }

	/// <summary>
	/// The window title
	/// </summary>
	string Title { get; set; }

	/// <summary>
	/// Controls if the window can be resized by the user.
	/// </summary>
	public bool Resizable { get; set; }

	public bool VSync { get; set; }

	/// <summary>
	/// Centers the window on the users screen
	/// </summary>
	public void Center();

	/// <summary>
	/// Focuses the window for input
	/// </summary>
	public void Focus();

	/// <summary>
	/// Requests the users attention by highlighting the window
	/// </summary>
	public void RequestAttention();

	/// <summary>
	/// Sets the window icon.
	/// </summary>
	public void SetIconFromStream(Stream? imageStream);




	internal bool ShouldClose { get; set; }

	/// <summary>
	/// Called when the window is being closed. Can be used to prevent closure.
	/// </summary>
	public Action? Closing { get; set; }

	/// <summary>
	/// Closes the window
	/// </summary>
	public void Close();

	public bool Visible { get; set; }

	/// <summary>
	/// Prevents the window closure attempt. This method is only valid within the <see cref="Closing"/> event.
	/// </summary>
	public void PreventClosure();

	internal void SwapBuffers();

	internal void ProcessEvents();
}
