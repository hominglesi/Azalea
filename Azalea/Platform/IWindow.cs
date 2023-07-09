namespace Azalea.Platform;

public interface IWindow
{
    Vector2Int ClientSize { get; }// set; }

    string Title { get; set; }
}
