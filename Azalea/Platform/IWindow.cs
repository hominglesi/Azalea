using System.IO;

namespace Azalea.Platform;

public interface IWindow
{
    internal const string DefaultTitle = "Azalea Game";

    Vector2Int ClientSize { get; set; }

    string Title { get; set; }

    WindowState State { get; set; }

    public void SetIconFromStream(Stream imageStream);
}
