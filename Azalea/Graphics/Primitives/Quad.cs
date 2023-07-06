using System.Numerics;

namespace Azalea.Graphics.Primitives;

public readonly struct Quad
{
    public readonly Vector2 TopLeft;
    public readonly Vector2 BottomLeft;
    public readonly Vector2 BottomRight;
    public readonly Vector2 TopRight;

    public Quad(Vector2 topLeft, Vector2 bottomLeft, Vector2 bottomRight, Vector2 topRight)
    {
        TopLeft = topLeft;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
        TopRight = topRight;
    }

    public Quad(float x, float y, float width, float height)
    {
        TopLeft = new Vector2(x, y);
        TopRight = new Vector2(x + width, y);
        BottomLeft = new Vector2(x, y + height);
        BottomRight = new Vector2(x + width, y + height);
    }

    public Quad(Vector2 position, Vector2 size)
        : this(position.X, position.Y, size.X, size.Y) { }
}
