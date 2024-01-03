using Azalea.Design.Shapes;
using Azalea.Platform;
using System.Numerics;

namespace Azalea.VisualTests.Breakout;
public class BreakoutBall : Box
{
	public const float BallSpeed = 5f;

	public Vector2 Direction;

	public BreakoutBall(Vector2 direction)
	{
		Direction = direction;
	}

	protected override void Update()
	{
		var timeStep = Time.DeltaTime;
		if (timeStep > 0.1f)
			timeStep = 0.1f;

		var nextPosition = Position + (Direction * BallSpeed * timeStep * 60);

		if (nextPosition.X < 0)
			Direction.X *= -1;
		else if (nextPosition.X + Size.X > BreakoutTest.GameWidth)
			Direction.X *= -1;

		if (nextPosition.Y < 0)
			Direction.Y *= -1;
		else if (nextPosition.Y > BreakoutTest.GameHeight)
			Direction.Y *= -1;
		//GetFirstParentOfType<Composition>()!.Remove(this);


		Position += Direction * BallSpeed * timeStep * 60;
	}
}
