using Azalea.Extentions;
using Azalea.Extentions.EnumExtentions;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Primitives;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Inputs.States;
using Azalea.Numerics;
using System;
using System.Numerics;

namespace Azalea.Graphics;

public abstract class GameObject : IGameObject
{
    public event Action<GameObject>? OnUpdate;

    public virtual bool UpdateSubTree()
    {
        UpdateInput();
        Update();
        OnUpdate?.Invoke(this);
        return true;
    }

    protected virtual void Update() { }

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

    public virtual Vector2 Size
    {
        get => size;
        set
        {
            if (size == value) return;

            size = value;
        }
    }

    public virtual float Width
    {
        get => width;
        set
        {
            if (width == value) return;

            width = value;
        }
    }

    public virtual float Height
    {
        get => height;
        set
        {
            if (height == value) return;

            height = value;
        }
    }

    private Axes _relativeSizeAxes;

    public virtual Axes RelativeSizeAxes
    {
        get => _relativeSizeAxes;
        set
        {
            if (_relativeSizeAxes == value) return;


            _relativeSizeAxes = value;
        }
    }

    public Rectangle DrawRectangle => new(0, 0, Size.X, Size.Y);

    protected Vector2 ApplyRelativeAxes(Axes relativeAxes, Vector2 v, FillMode fillMode)
    {
        if (relativeAxes != Axes.None)
        {
            Vector2 conversion = _relativeToAbsoluteFactor;

            if (relativeAxes.HasFlagFast(Axes.X))
                v.X *= conversion.X;

            if (relativeAxes.HasFlagFast(Axes.Y))
                v.Y *= conversion.Y;

            if (relativeAxes == Axes.Both && fillMode != FillMode.Stretch)
            {
                if (fillMode == FillMode.Fill)
                    v = new Vector2(Math.Max(v.X, v.Y * _fillAspectRatio));
                else if (fillMode == FillMode.Fit)
                    v = new Vector2(Math.Min(v.X, v.Y * _fillAspectRatio));
                v.Y /= _fillAspectRatio;
            }
        }

        return v;
    }

    private Vector2 _relativeToAbsoluteFactor => /*Parent?.RelativeToAbsoluteFactor ??*/ Vector2.One;

    private float _fillAspectRatio = 1;

    public float FillAspectRatio
    {
        get => _fillAspectRatio;
        set
        {
            if (_fillAspectRatio == value) return;

            if (float.IsFinite(value) == false) throw new ArgumentException($"{nameof(FillAspectRatio)} must be finite, but is {value}.");
            if (value == 0) throw new ArgumentException($@"{nameof(FillAspectRatio)} must be non-zero.");

            _fillAspectRatio = value;
            /*
            if (_fillMode != FillMode.Stretch && RelativeSizeAxes == Axes.Both)
                Invalidate(Invalidation.DrawSize);*/
        }
    }

    #endregion

    private Vector2 _scale = Vector2.One;

    public Vector2 Scale
    {
        get => _scale;
        set
        {
            if (_scale == value) return;

            //if(Validation.IsFinite(value) == false) throw new ArgumentException($@"{nameof(Scale)} must be finite, but is {value}.");

            bool wasPresent = IsPresent;

            _scale = value;

            /*
            if (IsPresent != wasPresent)
                Invalidate(Invalidation.MiscGeometry | Invalidation.Presence);
            else
                Invalidate(Invalidation.MiscGeomerty);*/
        }
    }

    private FillMode _fillMode;

    public FillMode FillMode
    {
        get => _fillMode;
        set
        {
            if (_fillMode == value) return;

            _fillMode = value;

            //Invalidate(Invalidation.DrawSize);
        }
    }

    protected virtual Vector2 DrawScale => Scale;

    private Anchor _origin = Anchor.TopLeft;

    public Anchor Origin
    {
        get => _origin;
        set
        {
            if (_origin == value) return;

            if (value == 0)
                throw new ArgumentException("Cannot set origin to 0.", nameof(value));

            _origin = value;
        }
    }

    public Vector2 RelativeOriginPosition => computeAnchorPosition(_origin);

    public Vector2 OriginPosition
    {
        get
        {
            Vector2 result;
            if (Origin == Anchor.TopLeft)
                result = Vector2.Zero;
            else
                result = Size * computeAnchorPosition(Origin);

            return result;
        }
    }

    private Anchor _anchor = Anchor.TopLeft;

    public Anchor Anchor
    {
        get => _anchor;
        set
        {
            if (_anchor == value) return;

            if (value == 0) throw new ArgumentException("Cannot set anchor to 0.", nameof(value));

            _anchor = value;
        }
    }

    public Vector2 RelativeAnchorPosition => computeAnchorPosition(_anchor);

    public Vector2 AnchorPosition => RelativeAnchorPosition * Parent?.Size ?? Vector2.Zero;

    private static Vector2 computeAnchorPosition(Anchor anchor)
    {
        Vector2 result = Vector2.Zero;
        if (anchor.HasFlagFast(Anchor.x1))
            result.X = 0.5f;
        else if (anchor.HasFlagFast(Anchor.x2))
            result.X = 1;

        if (anchor.HasFlagFast(Anchor.y1))
            result.Y = 0.5f;
        else if (anchor.HasFlagFast(Anchor.y2))
            result.Y = 1;

        return result;
    }

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

    protected InputManager GetContainingInputManager() => this.FindClosestParent<InputManager>()
        ?? throw new Exception("This drawable is not part of the scene graph");

    private CompositeGameObject? parent;

    public CompositeGameObject? Parent
    {
        get => parent;
        set
        {
            if (value == null)
                ChildID = 0;

            if (parent == value) return;

            if (value != null && parent != null)
                throw new Exception("Cannot add GameObject to multiple containers");

            parent = value;
        }
    }

    private const float visibility_cutoff = 0.0001f;

    public virtual bool IsPresent => AlwaysPresent || (Alpha > visibility_cutoff && DrawScale.X != 0 && DrawScale.Y != 0);

    private bool _alwaysPresent;

    public bool AlwaysPresent
    {
        get => _alwaysPresent;
        set
        {
            if (_alwaysPresent == value) return;

            bool wasPresent = IsPresent;

            _alwaysPresent = value;

            /*
            if (IsPresent != wasPresent)
                Invalidate(Invalidation.Presence)*/
        }
    }

    protected virtual DrawNode CreateDrawNode() => new(this);

    private DrawInfo? drawInfo;
    public DrawInfo DrawInfo => (DrawInfo)(drawInfo = computeDrawInfo());
    private DrawInfo computeDrawInfo()
    {
        DrawInfo di = Parent?.DrawInfo ?? new DrawInfo(null);

        Vector2 pos = Position + AnchorPosition;

        di.ApplyTransformations(pos, Vector2.One, 0, Vector2.Zero, OriginPosition);

        return di;
    }


    #region DrawInfo-based conversions

    public Vector2 ToScreenSpace(Vector2 input) => Vector2Extentions.Transform(input, DrawInfo.Matrix);

    public Quad ToScreenSpace(Rectangle input) => Quad.FromRectangle(input) * DrawInfo.Matrix;

    public Vector2 ToLocalSpace(Vector2 screenSpacePosition) => Vector2Extentions.Transform(screenSpacePosition, DrawInfo.MatrixInverse);

    public Quad ToLocalSpace(Quad screenSpaceQuad) => screenSpaceQuad * DrawInfo.MatrixInverse;

    #endregion

    #region Input

    private void UpdateInput()
    {
        if (ToScreenSpace(DrawRectangle).Contains(Input.MousePosition))
        {
            if (Input.GetMouseButton(MouseButton.Left).Down)
                TriggerEvent(new MouseDownEvent(GetContainingInputManager()?.CurrentState ?? new InputState(), MouseButton.Left));
            else if (Input.GetMouseButton(MouseButton.Left).Up)
            {
                TriggerEvent(new MouseUpEvent(GetContainingInputManager()?.CurrentState ?? new InputState(), MouseButton.Left));
                TriggerClick();
            }
        }

    }

    protected virtual bool Handle(UIEvent e) => false;

    public bool TriggerEvent(UIEvent e)
    {
        e.Target = this;

        return e switch
        {
            ClickEvent click => OnClick(click),
            MouseDownEvent mouseDown => OnMouseDown(mouseDown),
            MouseUpEvent mouseUp => OnMouseUp(mouseUp),
            _ => Handle(e),
        };
    }

    public bool TriggerClick() => TriggerEvent(new ClickEvent(GetContainingInputManager()?.CurrentState ?? new InputState(), MouseButton.Left));

    protected virtual bool OnClick(ClickEvent e) => Handle(e);
    protected virtual bool OnMouseDown(MouseDownEvent e) => Handle(e);
    protected virtual bool OnMouseUp(MouseUpEvent e) => Handle(e);

    #endregion

    public virtual Quad ScreenSpaceDrawQuad => ToScreenSpace(DrawRectangle);

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

[Flags]
public enum Anchor
{
    TopLeft = y0 | x0,
    TopCenter = y0 | x1,
    TopRight = y0 | x2,

    CenterLeft = y1 | x0,
    Center = y1 | x1,
    CenterRight = y1 | x2,

    BottomLeft = y2 | x0,
    BottomCenter = y2 | x1,
    BottomRight = y2 | x2,


    y0 = 1,
    y1 = 1 << 1,
    y2 = 1 << 2,
    x0 = 1 << 3,
    x1 = 1 << 4,
    x2 = 1 << 5,
}

[Flags]
public enum Axes
{
    None = 0,

    X = 1,
    Y = 1 << 1,

    Both = X | Y
}

public enum FillMode
{
    Stretch,
    Fill,
    Fit
}
