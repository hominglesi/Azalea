using System;
using System.IO;

namespace Azalea.Platform;

public interface IWindow : IDisposable
{
	/// <summary>
	/// The size of the window on the users desktop.
	/// This includes all the active windows features, like the caption, and the resize border.
	/// Even if the window is not resizable this will include the area around the window used for resizing.
	/// </summary>
	Vector2Int Size { get; set; }

	/// <summary>
	/// The size of the window client on the users desktop.
	/// This does not include the caption and the resize border.
	/// </summary>
	Vector2Int ClientSize { get; set; }

	internal Action<Vector2Int>? OnClientResized { get; set; }

	/// <summary>
	/// The position of the window on the users desktop
	/// This position is the top-left point of the window including all the active windows features,
	/// like the caption, and the resize border.
	/// Even if the window is not resizable this will account for the area around the window used for resizing.
	/// </summary>
	Vector2Int Position { get; set; }

	/// <summary>
	/// The position of the window client on the users desktop.
	/// This position is the top-left point of the window client.
	/// </summary>
	Vector2Int ClientPosition { get; set; }

	/// <summary>
	/// The current state of the window.
	/// </summary>
	WindowState State { get; set; }

	/// <summary>
	/// The title of the window.
	/// </summary>
	string Title { get; set; }

	/// <summary>
	/// Whether the window can be resized using the resize border.
	/// </summary>
	bool Resizable { get; set; }

	/// <summary>
	/// Whether V-Sync is enabled
	/// </summary>
	bool VSync { get; set; }

	/// <summary>
	/// Whether the application can change VSync setting
	/// </summary>
	bool CanChangeVSync { get; }

	/// <summary>
	/// Whether Cursor is shown
	/// </summary>
	bool CursorVisible { get; set; }

	/// <summary>
	/// Whether the window accepts files dropped over it
	/// </summary>
	bool AcceptFiles { get; set; }

	/// <summary>
	/// Centers the window on the users most overlapped monitor.
	/// </summary>
	void Center();

	/// <summary>
	/// Focuses the window on the users desktop.
	/// </summary>
	void Focus();

	/// <summary>
	/// Highlights the window icon in the taskbar.
	/// </summary>
	void RequestAttention();

	/// <summary>
	/// Sets the window icon.
	/// </summary>
	void SetIconFromStream(Stream? imageStream);

	internal void SwapBuffers();

	internal void Show(bool firstTime = false);

	internal void Hide();

	internal void ProcessEvents();

	/// <summary>
	/// Called when the window is being closed. Can be used to prevent closure.
	/// </summary>
	Action? Closing { get; set; }

	/// <summary>
	/// Closes the window.
	/// </summary>
	void Close();

	/// <summary>
	/// Prevents the window closure attempt. This method is only valid within the <see cref="Closing"/> event.
	/// </summary>
	void PreventClosure();
}
