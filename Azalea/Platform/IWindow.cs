using System;
using System.IO;

namespace Azalea.Platform;

public interface IWindow : IDisposable
{
	internal static Vector2Int CenterOffset = new(0, -10);

	//Aproximate window center for 1920x1080 resolution
	internal static Vector2Int AproximateCenterWindowPosition(Vector2Int windowSize)
		=> new Vector2Int(960, 540) - (windowSize / 2) + CenterOffset;

	internal bool ShouldClose { get; }

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
	/// Centers the window on the users screen
	/// </summary>
	public void Center();

	/// <summary>
	/// Sets the window icon.
	/// </summary>
	public void SetIconFromStream(Stream imageStream);

	/// <summary>
	/// Closes the window
	/// </summary>
	public void Close();
}
