using Azalea.Graphics;
using Azalea.Numerics;
using System.Numerics;

namespace Azalea.Design.Containers;
public class CameraContainer : Composition
{
	private Vector2 _windowSize => AzaleaGame.Main.Host.Window.ClientSize;

	public void CenterOnObject(GameObject obj)
	{
		Position = -obj.Position;
		Position -= obj.Size / 2;
		Position += obj.OriginPosition;
		Position *= Scale;
		Position += _windowSize / 2;

		cointainWithinBoundaries();
	}

	private GameObject? _followedObject;
	private Vector2 _lastFollowedObjectPosition;
	private Vector2 _lastWindowSize;
	public void SetFollowedObject(GameObject obj)
	{
		_followedObject = obj;
		_lastFollowedObjectPosition = obj.Position;
	}

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		if (_lastWindowSize == Vector2.Zero)
			_lastWindowSize = _windowSize;

		if (_lastWindowSize != _windowSize)
		{
			var movement = (_windowSize - _lastWindowSize) / 2;
			Position += movement;
			cointainWithinBoundaries();
			_lastWindowSize = _windowSize;
		}

		if (_followedObject is not null && _followedObject.Position != _lastFollowedObjectPosition)
		{
			CenterOnObject(_followedObject);

			_lastFollowedObjectPosition = _followedObject.Position;
		}
	}

	private Rectangle? _boundaries;
	public void SetBoundaries(Rectangle boundaries)
		=> _boundaries = boundaries;

	private void cointainWithinBoundaries()
	{
		if (_boundaries is null) return;
		Rectangle boundaries = _boundaries.Value;

		if (Position.Y > boundaries.Y)
			Y = boundaries.Y;
		else if (Position.Y < -boundaries.Bottom * Scale.Y + _windowSize.Y)
			Y = -boundaries.Bottom * Scale.Y + _windowSize.Y;

		if (Position.X > boundaries.X)
			X = boundaries.X;
		else if (Position.X < -boundaries.Right * Scale.X + _windowSize.X)
			X = -boundaries.Right * Scale.X + _windowSize.X;
	}
}
