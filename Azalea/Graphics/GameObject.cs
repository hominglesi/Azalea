using Azalea.Amends;
using Azalea.Design.Components;
using Azalea.Design.Containers;
using Azalea.Editing.Legacy;
using Azalea.Extentions;
using Azalea.Extentions.EnumExtentions;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Inputs.Events;
using Azalea.Layout;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics;

public partial class GameObject : Amendable, IGameObject
{
	public GameObject()
	{
		AddLayout(_drawInfoBacking);
	}

	public event Action<GameObject>? OnUpdate;
	internal event Action<GameObject, Invalidation>? Invalidated;

	#region Transform

	private float _x;
	[HideInInspector]
	public float X
	{
		get => _x;
		set
		{
			if (value == _x) return;

			_x = value;

			Invalidate(Invalidation.MiscGeometry);
		}
	}

	private float _y;
	[HideInInspector]
	public float Y
	{
		get => _y;
		set
		{
			if (value == _y) return;

			_y = value;

			Invalidate(Invalidation.MiscGeometry);
		}
	}

	public Vector2 Position
	{
		get => new(_x, _y);
		set
		{
			if (value == Position) return;

			_x = value.X;
			_y = value.Y;

			Invalidate(Invalidation.MiscGeometry);
		}
	}

	private float _width;
	[HideInInspector]
	public virtual float Width
	{
		get => _width;
		set
		{
			if (value == _width) return;

			_width = value;

			invalidateParentSizeDependencies(Invalidation.DrawSize, Axes.X);
		}
	}

	private float _height;
	[HideInInspector]
	public virtual float Height
	{
		get => _height;
		set
		{
			if (value == _height) return;

			_height = value;

			invalidateParentSizeDependencies(Invalidation.DrawSize, Axes.Y);
		}
	}

	public virtual Vector2 Size
	{
		get => new(_width, _height);
		set
		{
			if (value == Size) return;

			var changedAxes = Axes.None;

			if (_width != value.X)
				changedAxes |= Axes.X;

			if (_height != value.Y)
				changedAxes |= Axes.Y;

			_width = value.X;
			_height = value.Y;

			invalidateParentSizeDependencies(Invalidation.DrawSize, changedAxes);
		}
	}

	private float _rotation;
	public float Rotation
	{
		get => _rotation;
		set
		{
			if (value == _rotation) return;

			_rotation = value;

			Invalidate(Invalidation.MiscGeometry);
		}
	}

	#endregion

	public bool Active { get; set; } = true;

	public virtual void UpdateSubTree()
	{
		if (Active == false) return;

		UpdateAmends();
		Update();
		updateComponents();
		OnUpdate?.Invoke(this);
	}

	public virtual void FixedUpdateSubTree()
	{
		if (Active == false) return;

		FixedUpdate();
	}

	protected virtual void Update() { }

	protected virtual void FixedUpdate() { }

	#region Position & Size

	private Axes _relativePositionAxes;

	public Axes RelativePositionAxes
	{
		get => _relativePositionAxes;
		set
		{
			if (value == _relativePositionAxes) return;

			Vector2 conversion = _relativeToAbsoluteFactor;
			if ((value & Axes.X) > (_relativePositionAxes & Axes.X))
				X = Precision.AlmostEquals(conversion.X, 0) ? 0 : X / conversion.X;
			else if ((_relativePositionAxes & Axes.X) > (value & Axes.X))
				X *= conversion.X;

			if ((value & Axes.Y) > (_relativePositionAxes & Axes.Y))
				Y = Precision.AlmostEquals(conversion.Y, 0) ? 0 : Y / conversion.Y;
			else if ((_relativePositionAxes & Axes.Y) > (value & Axes.Y))
				Y *= conversion.Y;

			_relativePositionAxes = value;
		}
	}

	public Vector2 DrawPosition
	{
		get
		{
			Vector2 offset = Vector2.Zero;

			if (Parent != null && RelativePositionAxes != Axes.None)
			{
				offset = Parent.RelativeChildOffset;

				if (RelativePositionAxes.HasFlagFast(Axes.X) == false)
					offset.X = 0;
				if (RelativePositionAxes.HasFlagFast(Axes.Y) == false)
					offset.Y = 0;
			}

			return ApplyRelativeAxes(RelativePositionAxes, Position - offset, FillMode.Stretch);
		}
	}

	private Axes _relativeSizeAxes;

	public virtual Axes RelativeSizeAxes
	{
		get => _relativeSizeAxes;
		set
		{
			if (_relativeSizeAxes == value) return;

			if (_fillMode != FillMode.Stretch && (value == Axes.Both || _relativeSizeAxes == Axes.Both))
				Invalidate(Invalidation.DrawSize);
			else
			{
				Vector2 conversion = _relativeToAbsoluteFactor;
				if ((value & Axes.X) > (_relativeSizeAxes & Axes.X))
					Width = Precision.AlmostEquals(conversion.X, 0) ? 0 : Width / conversion.X;
				else if ((_relativeSizeAxes & Axes.X) > (value & Axes.X))
					Width *= conversion.X;

				if ((value & Axes.Y) > (_relativeSizeAxes & Axes.Y))
					Height = Precision.AlmostEquals(conversion.Y, 0) ? 0 : Height / conversion.Y;
				else if ((_relativeSizeAxes & Axes.Y) > (value & Axes.Y))
					Height *= conversion.Y;
			}

			_relativeSizeAxes = value;

			if (_relativeSizeAxes.HasFlagFast(Axes.X) && Width == 0) Width = 1;
			if (_relativeSizeAxes.HasFlagFast(Axes.Y) && Height == 0) Height = 1;
		}
	}

	public Vector2 _negativeSize;
	public Vector2 NegativeSize
	{
		get => _negativeSize;
		set
		{
			if (value == _negativeSize) return;

			_negativeSize = value;

			invalidateParentSizeDependencies(Invalidation.DrawSize, Axes.Both);
		}
	}

	public Vector2 DrawSize => ApplyRelativeAxes(RelativeSizeAxes, Size, FillMode) - NegativeSize;

	public float DrawWidth => DrawSize.X;
	public float DrawHeight => DrawSize.Y;
	private Boundary _margin;

	public Boundary Margin
	{
		get => _margin;
		set
		{
			if (_margin.Equals(value)) return;

			_margin = value;
		}
	}

	public Vector2 LayoutSize => DrawSize + new Vector2(Margin.Horizontal, Margin.Vertical);

	public Rectangle DrawRectangle => new(0, 0, DrawSize.X, DrawSize.Y);

	public Rectangle LayoutRectangle => new(-Margin.Left, -Margin.Top, LayoutSize.X, LayoutSize.Y);

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

	private Vector2 _relativeToAbsoluteFactor => Parent?.RelativeToAbsoluteFactor ?? Vector2.One;

	public virtual Rectangle BoundingBox => ToParentSpace(LayoutRectangle).AABBFloat;

	protected virtual void OnSizingChanged()
	{

	}

	private Vector2 computeRequiredParentSizeToFit()
	{
		var ap = AnchorPosition;
		var rap = RelativeAnchorPosition;

		var ratio1 = new Vector2(
			rap.X <= 0 ? 0 : 1 / rap.X,
			rap.Y <= 0 ? 0 : 1 / rap.Y);

		var ratio2 = new Vector2(
			rap.X >= 1 ? 0 : 1 / (1 - rap.X),
			rap.Y >= 1 ? 0 : 1 / (1 - rap.Y));

		Rectangle bBox = BoundingBox;

		var topLeftOffset = ap - bBox.TopLeft;
		var topLeftSize1 = topLeftOffset * ratio1;
		var topLeftSize2 = -topLeftOffset * ratio2;

		var bottomRightOffset = ap - bBox.BottomRight;
		var bottomRightSize1 = bottomRightOffset * ratio1;
		var bottomRightSize2 = -bottomRightOffset * ratio2;

		return Vector2Extentions.ComponentMax(
			Vector2Extentions.ComponentMax(topLeftSize1, topLeftSize2),
			Vector2Extentions.ComponentMax(bottomRightSize1, bottomRightSize2));
	}

	internal Vector2 RequiredParentSizeToFit => computeRequiredParentSizeToFit();

	private float _fillAspectRatio = 1;

	[HideInInspector]
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

			_scale = value;

			Invalidate(Invalidation.MiscGeometry);
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

			Invalidate(Invalidation.DrawSize);
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

	public Vector2 RelativeOriginPosition => ComputeAnchorPosition(_origin);

	private Vector2 _customOrigin;

	public Vector2 OriginPosition
	{
		get
		{
			Vector2 result;
			if (Origin == Anchor.Custom)
				result = _customOrigin;
			else if (Origin == Anchor.TopLeft)
				result = Vector2.Zero;
			else
				result = DrawSize * RelativeOriginPosition;

			return result - new Vector2(Margin.Left, Margin.Top);
		}
		set
		{
			if (_customOrigin == value && Origin == Anchor.Custom)
				return;

			_customOrigin = value;
			Origin = Anchor.Custom;

			Invalidate(Invalidation.MiscGeometry);
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
			Invalidate(Invalidation.MiscGeometry);
		}
	}

	public Vector2 RelativeAnchorPosition => ComputeAnchorPosition(_anchor);

	public Vector2 AnchorPosition => RelativeAnchorPosition * Parent?.ChildSize ?? Vector2.Zero;

	internal static Vector2 ComputeAnchorPosition(Anchor anchor)
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

	private ColorQuad _color = ColorQuad.SolidColor(Palette.White);

	public ColorQuad Color
	{
		get => _color;
		set
		{
			if (_color == value) return;

			_color = value;
		}
	}

	private float _alpha = 1;

	public float Alpha
	{
		get => _alpha;
		set
		{
			if (_alpha == value) return;

			_alpha = Math.Clamp(value, 0, 1);

			Invalidate(Invalidation.Color);
		}
	}

	#endregion

	internal ulong ChildID { get; set; }

	internal bool IsPartOfComposite => ChildID != 0;

	private float depth;
	[HideInInspector]
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
				throw new Exception("Cannot add GameObject to multiple compositions.");

			parent = value;

			if (parent == null)
				RemoveComponentTreeFromScene();
			else
				AddComponentTreeToScene();

			Invalidate(Invalidation.Presence);
		}
	}

	private Axes _ignoredForAutoSizeAxes;
	public Axes IgnoredForAutoSizeAxes
	{
		get { return _ignoredForAutoSizeAxes; }
		set
		{
			if (value == _ignoredForAutoSizeAxes) return;

			_ignoredForAutoSizeAxes = value;

			Invalidate(Invalidation.RequiredParentSizeToFit);
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

			if (IsPresent != wasPresent)
				Invalidate(Invalidation.Presence);
		}
	}

	public virtual void Draw(IRenderer renderer)
	{

	}

	public DrawInfo DrawInfo
	{
		get
		{
			if (_drawInfoBacking.IsValid == false || AzaleaSettings.DontCacheDrawInfo == true)
			{
				_drawInfo = computeDrawInfo();
				_drawInfoBacking.Validate();
				OnDrawInfoInvalidated?.Invoke();
			}

			return _drawInfo;
		}
	}

	private DrawInfo _drawInfo;
	private readonly LayoutValue _drawInfoBacking = new LayoutValue(Invalidation.DrawInfo | Invalidation.RequiredParentSizeToFit | Invalidation.Presence);
	private DrawInfo computeDrawInfo()
	{
		DrawInfo di = Parent?.DrawInfo ?? new DrawInfo(null);

		Vector2 pos = DrawPosition + AnchorPosition;

		if (Parent != null)
			pos += Parent.ChildOffset;

		di.ApplyTransformations(pos, Scale, Rotation, Vector2.Zero, OriginPosition);

		return di;
	}

	internal Action? OnDrawInfoInvalidated;

	#region Components

	private List<Component> _components = new();

	public void AddComponent(Component component)
	{
		_components.Add(component);
		component.AttachParent(this);
	}

	public void RemoveComponent(Component component)
	{
		_components.Remove(component);
		component.DetachParent();
	}

	private void updateComponents() { foreach (var c in _components) c.Update(); }

	public T? GetComponent<T>()
		where T : class
	{
		foreach (var comp in _components)
		{
			if (comp is T castComp)
				return castComp;
		}

		return null;
	}

	public bool HasComponent<T>()
		where T : class
		=> GetComponent<T> != null;

	public IEnumerable<T> GetComponents<T>()
		where T : class
	{
		foreach (var comp in _components)
		{
			if (comp is T castComp)
				yield return castComp;
		}
	}

	internal virtual void AddComponentTreeToScene()
	{
		foreach (var component in _components)
			component.AddToScene();
	}

	internal virtual void RemoveComponentTreeFromScene()
	{
		foreach (var component in _components)
			component.RemoveFromScene();
	}

	#endregion

	private InvalidationList invalidationList = new(Invalidation.All);
	private LayoutMember? layoutList;

	protected void AddLayout(LayoutMember member)
	{
		if (layoutList == null)
			layoutList = member;
		else
		{
			member.Next = layoutList;
			layoutList = member;
		}

		member.Parent = this;
	}

	internal void ValidateSuperTree(Invalidation validationType)
	{
		if (invalidationList.Validate(validationType))
			Parent?.ValidateSuperTree(validationType);
	}

	public T? GetFirstParentOfType<T>()
		where T : GameObject
	{
		var parent = Parent;
		while (parent != null)
		{
			if (parent is T tParent)
				return tParent;

			parent = parent.Parent;
		}

		return null;
	}

	public long InvalidationID { get; private set; } = 1;

	public bool Invalidate(Invalidation invalidation = Invalidation.All, InvalidationSource source = InvalidationSource.Self)
		=> invalidate(invalidation, source);

	private bool invalidate(Invalidation invalidation = Invalidation.All, InvalidationSource source = InvalidationSource.Self, bool propagateToParent = true)
	{
		if (source != InvalidationSource.Child && source != InvalidationSource.Parent && source != InvalidationSource.Self)
			throw new InvalidOperationException($"A {nameof(GameObject)} can only be invalidated with a singular {nameof(source)} (child, parent, self).");

		if (source == InvalidationSource.Child)
			invalidation &= ~Invalidation.Color;

		if (invalidation == Invalidation.None)
			return false;

		if (propagateToParent && (source == InvalidationSource.Self || GetType() == typeof(CompositeGameObject)))
			Parent?.Invalidate(invalidation, InvalidationSource.Child);

		if (invalidationList.Invalidate(source, invalidation) == false)
			return false;

		bool anyInvalidated = (invalidation & Invalidation.DrawNode) > 0;

		LayoutMember? nextLayout = layoutList;

		while (nextLayout is not null)
		{
			if ((nextLayout.Source & source) == 0)
				goto NextLayoutIteration;

			Invalidation memberInvalidation = invalidation & nextLayout.Invalidation;
			if (memberInvalidation == 0)
				goto NextLayoutIteration;

			anyInvalidated |= nextLayout.Invalidate();

		NextLayoutIteration:
			nextLayout = nextLayout.Next;
		}

		anyInvalidated |= OnInvalidate(invalidation, source);

		if (anyInvalidated)
			InvalidationID++;

		Invalidated?.Invoke(this, invalidation);

		return anyInvalidated;
	}

	protected virtual bool OnInvalidate(Invalidation invalidation, InvalidationSource source) => false;

	public Invalidation InvalidationFromParentSize
	{
		get
		{
			Invalidation result = Invalidation.DrawInfo;
			if (RelativeSizeAxes != Axes.None)
				result |= Invalidation.DrawSize;
			if (RelativePositionAxes != Axes.None)
				result |= Invalidation.MiscGeometry;
			return result;
		}
	}

	private void invalidateParentSizeDependencies(Invalidation invalidation, Axes changedAxes)
	{
		invalidate(invalidation, InvalidationSource.Self, false);

		Parent?.InvalidateChildrenSizeDependencies(invalidation, changedAxes, this);
	}

	#region DrawInfo-based conversions

	public Vector2 ToSpaceOfOtherDrawable(Vector2 input, IGameObject other) => other == this ? input
		: Vector2Extentions.Transform(Vector2Extentions.Transform(input, DrawInfo.Matrix), other.DrawInfo.MatrixInverse);
	public Quad ToSpaceOfOtherDrawable(Rectangle input, IGameObject other) => other == this ? input
		: Quad.FromRectangle(input) * (DrawInfo.Matrix * other.DrawInfo.MatrixInverse);
	public Vector2 ToParentSpace(Vector2 input) => ToSpaceOfOtherDrawable(input, Parent
		?? throw new Exception($"This {nameof(GameObject)} doesn't have a {nameof(Parent)}"));
	public Quad ToParentSpace(Rectangle input) => ToSpaceOfOtherDrawable(input, Parent
		?? throw new Exception($"This {nameof(GameObject)} doesn't have a {nameof(Parent)}"));
	public Vector2 ToScreenSpace(Vector2 input) => Vector2Extentions.Transform(input, DrawInfo.Matrix);
	public Quad ToScreenSpace(Rectangle input) => Quad.FromRectangle(input) * DrawInfo.Matrix;
	public Vector2 ToLocalSpace(Vector2 screenSpacePosition) => Vector2Extentions.Transform(screenSpacePosition, DrawInfo.MatrixInverse);
	public Quad ToLocalSpace(Quad screenSpaceQuad) => screenSpaceQuad * DrawInfo.MatrixInverse;

	#endregion

	#region Input

	internal virtual bool BuildPositionalInputQueue(Vector2 screenSpacePos, List<GameObject> queue)
	{
		if (DrawRectangle.Contains(ToLocalSpace(screenSpacePos)) == false)
			return false;

		queue.Add(this);
		return true;
	}

	internal virtual bool BuildNonPositionalInputQueue(List<GameObject> queue)
	{
		queue.Add(this);

		return true;
	}

	/// <summary>
	/// Check if this <see cref="GameObject"/> has focus
	/// </summary>
	public bool HasFocus { get; internal set; }

	/// <summary>
	/// Controls if this <see cref="GameObject"/> should be able to be focused
	/// </summary>
	public virtual bool AcceptsFocus => false;

	/// <summary>
	/// Check if this <see cref="GameObject"/> is hovered
	/// </summary>
	public bool Hovered { get; internal set; }

	protected virtual bool Handle(InputEvent e) => false;

	public virtual bool TriggerEvent(InputEvent e)
	{
		e.Target = this;

		switch (e)
		{
			case MouseDownEvent mouseDown:
				var mdResult = OnMouseDown(mouseDown);
				MouseDown?.Invoke(mouseDown);
				return mdResult;
			case MouseUpEvent mouseUp:
				OnMouseUp(mouseUp);
				MouseUp?.Invoke(mouseUp);
				return false;
			case ClickEvent click:
				var cResult = OnClick(click);
				Click?.Invoke(click);
				return cResult;
			case HoverEvent hover:
				var hResult = OnHover(hover);
				Hover?.Invoke(hover);
				return hResult;
			case HoverLostEvent hoverLost:
				OnHoverLost(hoverLost);
				HoverLost?.Invoke(hoverLost);
				return false;
			case ScrollEvent scroll:
				OnScroll(scroll);
				Scroll?.Invoke(scroll);
				return false;
			case KeyDownEvent keyDown:
				var kdResult = OnKeyDown(keyDown);
				return kdResult;
			case KeyUpEvent keyUp:
				OnKeyUp(keyUp);
				KeyUp?.Invoke(keyUp);
				return false;
			case FocusEvent focus:
				OnFocus(focus);
				Focus?.Invoke(focus);
				return false;
			case FocusLostEvent focusLost:
				OnFocusLost(focusLost);
				FocusLost?.Invoke(focusLost);
				return false;
			default:
				return Handle(e);
		}
	}

	protected virtual bool OnMouseDown(MouseDownEvent e) => Handle(e);
	public event Action<MouseDownEvent>? MouseDown;
	protected virtual void OnMouseUp(MouseUpEvent e) => Handle(e);
	public event Action<MouseUpEvent>? MouseUp;
	protected virtual bool OnClick(ClickEvent e) => Handle(e);
	public Action<ClickEvent> ClickAction { set { Click += value; } }
	public event Action<ClickEvent>? Click;
	protected virtual bool OnHover(HoverEvent e) => Handle(e);
	public event Action<HoverEvent>? Hover;
	protected virtual void OnHoverLost(HoverLostEvent e) => Handle(e);
	public event Action<HoverLostEvent>? HoverLost;
	protected virtual void OnScroll(ScrollEvent e) => Handle(e);
	public event Action<ScrollEvent>? Scroll;
	protected virtual bool OnKeyDown(KeyDownEvent e) => Handle(e);
	public event Action<KeyDownEvent>? KeyDown;
	protected virtual void OnKeyUp(KeyUpEvent e) => Handle(e);
	public event Action<KeyUpEvent>? KeyUp;
	protected virtual void OnFocus(FocusEvent e) => Handle(e);
	public event Action<FocusEvent>? Focus;
	protected virtual void OnFocusLost(FocusLostEvent e) => Handle(e);
	public event Action<FocusLostEvent>? FocusLost;

	#endregion

	public virtual Quad ScreenSpaceDrawQuad => ToScreenSpace(DrawRectangle);

	public virtual DrawColorInfo DrawColorInfo => computeDrawColorInfo();

	private DrawColorInfo computeDrawColorInfo()
	{
		var info = Parent?.DrawColorInfo ?? new DrawColorInfo(null);

		var colorInfo = _color;

		if (Alpha != 1)
			colorInfo = colorInfo.MultiplyAlpha(Alpha);

		info.Color.ApplyChild(colorInfo);

		return info;
	}
}

[Flags]
public enum Invalidation
{
	DrawInfo = 1,
	DrawSize = 1 << 1,
	MiscGeometry = 1 << 2,
	Color = 1 << 3,
	DrawNode = 1 << 4,
	Presence = 1 << 5,
	Parent = 1 << 6,

	RequiredParentSizeToFit = MiscGeometry | DrawSize,
	All = DrawNode | RequiredParentSizeToFit | Color | DrawInfo | Presence,
	Layout = All & ~(DrawNode | Parent),

	None = 0
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

	Custom = 1 << 6,
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
