using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.VisualTests.Breakout;
public class BreakoutTest : TestScene
{
	public const int GameWidth = 1024;
	public const int GameHeight = 700;
	public const int BlocksHeight = 256;
	public const int BlockWidth = 8;
	public const int BlockHeight = 8;
	public const int PaddleWidth = 100;
	public const int PaddleHeight = 20;
	public const int PaddleOffset = 10;

	private BreakoutChunk _chunkRoot;
	private List<BreakoutBall> _balls = new();
	private List<Box> _tempBoxes = new();

	public BreakoutTest()
	{
		AzaleaGame.Main.Host.Renderer.ClearColor = Palette.Black;
		var window = AzaleaGame.Main.Host.Window;
		window.ClientSize = new(GameWidth, GameHeight);
		window.Resizable = false;
		window.Center();

		_chunkRoot = new BreakoutChunk(new Rectangle(Vector2.Zero,
									   new Vector2(GameWidth, BlocksHeight)));

		//Create blocks
		for (int j = 0; j < BlocksHeight; j += BlockHeight)
		{
			for (int i = 0; i < GameWidth; i += BlockWidth)
			{
				var brick = new Box()
				{
					X = i,
					Y = j,
					Size = new(BlockWidth, BlockHeight),
					Color = new ColorQuad(
						getColorFromPosition(i, j),
						getColorFromPosition(i, j + BlockHeight),
						getColorFromPosition(i + BlockWidth, j + BlockHeight),
						getColorFromPosition(i + BlockWidth, j))
				};
				_chunkRoot.AddBrick(brick);
				_tempBoxes.Add(brick);
				Add(brick);
			}
		}

		_chunkRoot.SubdivideHorizontaly();
		_chunkRoot.SubdivideHorizontaly();
		_chunkRoot.Subdivide(5);

		//Create ball
		var initialBallDirection = Rng.Direction();
		initialBallDirection.Y = -Math.Abs(initialBallDirection.Y);
		var ball = new BreakoutBall(initialBallDirection)
		{
			Y = GameHeight - PaddleOffset - PaddleHeight - BlockHeight,
			X = (GameWidth / 2) - (PaddleWidth / 2),
			Size = new(BlockWidth, BlockHeight)
		};
		_balls.Add(ball);

		Add(ball);
	}

	private Color getColorFromPosition(int x, int y)
	{
		var hue = MathUtils.Map(x, 0, GameWidth, 0, 350);
		var luminance = MathUtils.Map(y, 0, BlocksHeight, 0.25f, 0.85f);

		return Graphics.Colors.Color.FromHSL(new(hue, 0.95f, luminance, 1));
	}

	private List<BreakoutBall> _createdBalls = new();
	protected override void UpdateAfterChildren()
	{
		foreach (var ball in _balls)
		{
			if (_chunkRoot.SubChunks.Count == 0) return;

			//if (ball.Direction.Y > 0) continue;

			Box? collision = null;

			foreach (var intersectingChunk in _chunkRoot.GetChunksIntersecting(ball.BoundingBox))
			{
				if (collision is not null) break;
				foreach (var brick in intersectingChunk.Bricks)
				{
					if (brick.BoundingBox.Intersects(ball.BoundingBox))
					{
						collision = brick;
						break;
					}
				}
			}

			if (collision is not null)
			{
				_chunkRoot.RemoveBrick(collision);

				ball.Direction.Y *= -1;
				Remove(collision);

				var newDirection = Rng.Direction();
				newDirection.Y = Math.Abs(newDirection.Y);

				_createdBalls.Add(new BreakoutBall(newDirection)
				{
					Position = collision.Position + new Vector2(0, BlockHeight * 1.5f),
					Size = collision.Size,
					Color = collision.Color,
				});
			}
		}

		if (_createdBalls.Count > 0)
		{
			foreach (var created in _createdBalls)
			{
				_balls.Add(created);
				Add(created);
			}

			_createdBalls.Clear();
		}
	}
}
