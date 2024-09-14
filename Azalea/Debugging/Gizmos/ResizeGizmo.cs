using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System.Numerics;

namespace Azalea.Debugging.Gizmos;
public class ResizeGizmo : Composition
{
	private const float __lineWidth = 4;
	private static Color __lineColor = new(115, 144, 252);

	private GameObject? _targetObject;

	private Box _topLine;
	private Box _bottomLine;
	private Box _leftLine;
	private Box _rightLine;

	private ResizeNode _topLeftNode;
	private ResizeNode _topRightNode;
	private ResizeNode _bottomLeftNode;
	private ResizeNode _bottomRightNode;

	public ResizeGizmo()
	{
		Depth = -1000;

		Add(_topLine = new Box()
		{
			Origin = Anchor.CenterLeft,
			Height = __lineWidth,
			Color = __lineColor
		});
		Add(_bottomLine = new Box()
		{
			Origin = Anchor.CenterRight,
			Height = __lineWidth,
			Color = __lineColor
		});
		Add(_leftLine = new Box()
		{
			Origin = Anchor.TopCenter,
			Width = __lineWidth,
			Color = __lineColor
		});
		Add(_rightLine = new Box()
		{
			Origin = Anchor.BottomCenter,
			Width = __lineWidth,
			Color = __lineColor
		});

		Add(_topLeftNode = new ResizeNode());
		Add(_topRightNode = new ResizeNode());
		Add(_bottomLeftNode = new ResizeNode());
		Add(_bottomRightNode = new ResizeNode());

		_topLeftNode.PositionChanged += () => onResized(Anchor.TopLeft, _topLeftNode);
		_topRightNode.PositionChanged += () => onResized(Anchor.TopRight, _topRightNode);
		_bottomLeftNode.PositionChanged += () => onResized(Anchor.BottomLeft, _bottomLeftNode);
		_bottomRightNode.PositionChanged += () => onResized(Anchor.BottomRight, _bottomRightNode);
	}

	public void SetTarget(GameObject target)
	{
		_targetObject = target;
	}

	private Vector2 _lastPosition;
	private Vector2 _lastSize;
	protected override void Update()
	{
		if (_targetObject is null) return;

		var targetPosition = _targetObject.ToSpaceOfOtherDrawable(Vector2.Zero, this);
		var targetSize = _targetObject.Size;

		if (targetPosition == _lastPosition && targetSize == _lastSize)
			return;

		_topLine.Position = _leftLine.Position = targetPosition;
		_bottomLine.Position = _rightLine.Position = targetPosition + targetSize;
		_topLine.Width = _bottomLine.Width = targetSize.X;
		_leftLine.Height = _rightLine.Height = targetSize.Y;

		_topLeftNode.Position = targetPosition;
		_topRightNode.Position = targetPosition + new Vector2(targetSize.X, 0);
		_bottomLeftNode.Position = targetPosition + new Vector2(0, targetSize.Y);
		_bottomRightNode.Position = targetPosition + targetSize;

		_lastPosition = targetPosition;
		_lastSize = targetSize;
	}

	private void onResized(Anchor anchor, ResizeNode node)
	{
		if (_targetObject is null)
			return;

		var difference = _lastPosition;
		if (anchor.HasFlag(Anchor.x2)) difference.X += _lastSize.X;
		if (anchor.HasFlag(Anchor.y2)) difference.Y += _lastSize.Y;

		difference -= node.Position;

		if (anchor.HasFlag(Anchor.x0))
		{
			_targetObject.X -= difference.X;
			_targetObject.Width += difference.X;
		}
		else
		{
			_targetObject.Width -= difference.X;
		}

		if (anchor.HasFlag(Anchor.y0))
		{
			_targetObject.Y -= difference.Y;
			_targetObject.Height += difference.Y;
		}
		else
		{
			_targetObject.Height -= difference.Y;
		}
	}

	private class ResizeNode : DraggableContainer
	{
		private Sprite _sprite;
		public ResizeNode()
		{
			Add(_sprite = new Sprite()
			{
				Texture = Assets.GetTexture("Textures/resizeNode.png")
			});

			Size = _sprite.Size;
			Origin = Anchor.Center;
		}
	}
}
