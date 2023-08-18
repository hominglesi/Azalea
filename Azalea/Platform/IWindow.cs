using System.IO;

namespace Azalea.Platform;

public interface IWindow
{
    internal const string DefaultTitle = "Azalea Game";

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
    /// Sets the window icon.
    /// </summary>
    public void SetIconFromStream(Stream imageStream);
}