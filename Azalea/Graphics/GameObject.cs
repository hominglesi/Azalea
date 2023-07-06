using Azalea.Graphics.Containers;
using Azalea.Graphics.Primitives;
using System;
using System.Numerics;

namespace Azalea.Graphics;

public abstract class GameObject : IGameObject
{

    #region Position & Size

    private float x;
    private float y;

    private Vector2 position
    {
        get => new(x, y);
        set
        {
            x = value.X;
            y = value.Y;
        }
    }

    public Vector2 Position
    {
        get => position;
        set
        {
            if (position == value) return;

            position = value;
        }
    }

    public float X
    {
        get => x;
        set
        {
            if (x == value) return;

            x = value;
        }
    }

    public float Y
    {
        get => y;
        set
        {
            if (y == value) return;

            y = value;
        }
    }

    private float width;
    private float height;

    private Vector2 size
    {
        get => new(width, height);
        set
        {
            width = value.X;
            height = value.Y;
        }
    }

    public Vector2 Size
    {
        get => size;
        set
        {
            if (size == value) return;

            size = value;
        }
    }

    public float Width
    {
        get => width;
        set
        {
            if (width == value) return;

            width = value;
        }
    }

    public float Height
    {
        get => height;
        set
        {
            if (height == value) return;

            height = value;
        }
    }

    #endregion

    #region Color & Alpha

    private Color color = Color.White;

    public Color Color
    {
        get => color;
        set
        {
            if (color == value) return;

            color = value;
        }
    }

    private float alpha = 1.0f;

    public float Alpha
    {
        get => alpha;
        set
        {
            if (alpha == value) return;

            alpha = value;
        }
    }

    #endregion

    internal ulong ChildID { get; set; }

    internal bool IsPartOfComposite => ChildID != 0;

    private float depth;
    public float Depth
    {
        get => depth;
        set
        {
            if (IsPartOfComposite)
            {
                throw new InvalidOperationException(
                    $"May not change {nameof(Depth)} while inside a parent {nameof(CompositeGameObject)}.");
            }

            depth = value;
        }
    }

    protected virtual DrawNode CreateDrawNode() => new(this);

    public virtual Quad ScreenSpaceDrawQuad => new(Position, Size);

    public virtual DrawColorInfo DrawColorInfo => computeDrawColorInfo();

    private DrawColorInfo computeDrawColorInfo()
    {
        return new DrawColorInfo(Color, Alpha);
    }

    private DrawNode? drawNode;

    public virtual DrawNode? GenerateDrawNodeSubtree()
    {
        drawNode ??= CreateDrawNode();

        return drawNode;
    }
}
