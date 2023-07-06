using System.Numerics;

namespace Azalea.Platform;

public interface IWindow
{
    Vector2 ClientSize { get; }

    string Title { get; set; }
}
