using System;
using System.IO;

namespace Azalea.Platform;

public interface IWindow : IDisposable
{
	internal bool ShouldClose { get; set; }

	internal Action<Vector2Int>? Resized { get; set; }

	/// <summary>
	/// The window title.
	/// </summary>
	string Title { get; set; }

	/// <summary>
	/// The size of the window, excluding the title bar and border.
	/// </summary>
	Vector2Int ClientSize { get; set; }

	/// <summary>
	/// Controls the state of the window.
	/// For possible states see <seealso cref="WindowState"/>.
	/// </summary>
	WindowState State { get; set; }

	/// <summary>
	/// Controls if the window can be resized by the user. (Default: false)
	/// </summary>
	public bool Resizable { get; set; }

	/// <summary>
	/// Controls the visibility of the cursor. (Default: true)
	/// </summary>
	public bool CursorVisible { get; set; }

	/// <summary>
	/// The position of this window on the users desktop
	/// </summary>
	public Vector2Int Position { get; set; }

	/// <summary>
	/// The opacity of this window and its decoration
	/// </summary>
	public float Opacity { get; set; }

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

	/// <summary>
	/// Called when the window is being closed. Can be used to prevent closure.
	/// </summary>
	public Action? Closing { get; set; }

	/// <summary>
	/// Closes the window
	/// </summary>
	public void Close();

	/// <summary>
	/// Shows a hidden window
	/// </summary>
	public void Show();

	/// <summary>
	/// Hides the window from the user
	/// </summary>
	public void Hide();

	/// <summary>
	/// Prevents the window closure attempt. This method is only valid within the <see cref="Closing"/> event.
	/// </summary>
	public void PreventClosure();
}
