using Azalea.Extentions;
using Azalea.Numerics;
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

    public static implicit operator Quad(Rectangle r) => FromRectangle(r);

    public static Quad FromRectangle(Rectangle rectangle) =>
        new(rectangle.TopLeft,
            rectangle.BottomLeft,
            rectangle.BottomRight,
            rectangle.TopRight);

    public static Quad operator *(Quad r, Matrix3 m) =>
        new(Vector2Extentions.Transform(r.TopLeft, m),
            Vector2Extentions.Transform(r.BottomLeft, m),
            Vector2Extentions.Transform(r.BottomRight, m),
            Vector2Extentions.Transform(r.TopRight, m));

    public float Width => Vector2Extentions.Distance(TopLeft, TopRight);
    public float Height => Vector2Extentions.Distance(TopLeft, BottomLeft);

    public bool Contains(Vector2 pos)
    {
        if (Width == 0 && Height == 0)
            return pos == TopLeft;


        float perpDot1 = Vector2Extentions.PerpDot(BottomLeft - TopLeft, pos - TopLeft);
        if (float.IsNaN(perpDot1))
            return false;

        float perpDot2 = Vector2Extentions.PerpDot(BottomRight - BottomLeft, pos - BottomLeft);
        if (float.IsNaN(perpDot2) || perpDot1 * perpDot2 < 0)
            return false;

        float perpDot3 = Vector2Extentions.PerpDot(TopRight - BottomRight, pos - BottomRight);
        if (float.IsNaN(perpDot3) || perpDot1 * perpDot3 < 0 || perpDot2 * perpDot3 < 0)
            return false;

        float perpDot4 = Vector2Extentions.PerpDot(TopLeft - TopRight, pos - TopRight);
        if (float.IsNaN(perpDot4) || perpDot1 * perpDot4 < 0 || perpDot2 * perpDot4 < 0 || perpDot3 * perpDot4 < 0)
            return false;

        return true;
    }
}
