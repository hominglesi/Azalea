using System.Numerics;

namespace Azalea.Extentions;

public static class Vector2Extentions
{
    public static Vector2 ComponentMax(Vector2 a, Vector2 b)
    {
        a.X = a.X > b.X ? a.X : b.X;
        a.Y = a.Y > b.Y ? a.Y : b.Y;
        return a;
    }
}
