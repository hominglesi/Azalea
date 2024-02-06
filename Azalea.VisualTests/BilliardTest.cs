using Azalea.Design.Components;
using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Physics;
using Azalea.Physics.Colliders;
using Azalea.Platform;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Azalea.VisualTests;
public class BilliardTest : TestScene
{
	public Box box1;
	public Box box2;
	public Box box3;
	public Box box4;

	int tableWidth = 1200;
	int tableHeight = 600;
	int panelWidth = 1680;
	int panelHeight = 960;
	int edgeThickness = 50;

	Box topLeftEdge;
	Box topRightEdge;
	Box bottomLeftEdge;
	Box bottomRightEdge;
	Box leftEdge;
	Box rightEdge;

	Box topLeftHoleEdgeLeft;
	Box topLeftHoleEdgeRight;
	Box topMiddleHoleEdgeLeft;
	Box topMiddleHoleEdgeRight;
	Box topRightHoleEdgeLeft;
	Box topRightHoleEdgeRight;

	Box bottomLeftHoleEdgeLeft;
	Box bottomLeftHoleEdgeRight;
	Box bottomMiddleHoleEdgeLeft;
	Box bottomMiddleHoleEdgeRight;
	Box bottomRightHoleEdgeLeft;
	Box bottomRightHoleEdgeRight;

	SpriteText player1Text;
	SpriteText player2Text;
	SpriteText targetBallText;

	int holeOffset = 10;
	int middleOffset = 25;

	Box tableMat;

	Player player1;
	Player player2;
	Player currentPlayer;


	Box wallL;
	Box wallR;

	float holeRadius = 25;

	BallType targetBallType = BallType.Red;
	BallType firstHitBallType;
	List<SnookerBall> scoredBallsThisTurn = new List<SnookerBall>();
	SnookerBall whiteBall;
	SnookerBall[] redBalls = new SnookerBall[15];
	SnookerBall blueBall;
	SnookerBall yellowBall;
	SnookerBall blackBall;
	SnookerBall greenBall;
	SnookerBall brownBall;
	SnookerBall pinkBall;

	List<SnookerBall> AllBalls = new List<SnookerBall>();

	Sprite topLeftHole;
	Sprite topMiddleHole;
	Sprite topRightHole;
	Sprite bottomLeftHole;
	Sprite bottomMiddleHole;
	Sprite bottomRightHole;


	FlexContainer infoContainer;

	Vector2 topLeftHolePosition;
	Vector2 topMiddleHolePosition;
	Vector2 topRightHolePosition;
	Vector2 bottomLeftHolePosition;
	Vector2 bottomMiddleHolePosition;
	Vector2 bottomRightHolePosition;

	Vector2 whitePosition = new Vector2(1240, 480);
	Vector2 yellowPosition = new Vector2(1180, 380);
	Vector2 brownPosition = new Vector2(1180, 480);
	Vector2 greenPosition = new Vector2(1180, 580);
	Vector2 redPosition = new Vector2(540, 480);
	Vector2 pinkPosition = new Vector2(570, 480);
	Vector2 blackPosition = new Vector2(540 - 5 * 2 * 15 - 5, 480);
	Vector2 bluePosition = new Vector2(840, 480);

	Line line;
	Line aimLine = new Line()
	{
		Thickness = 1,
		Color = Palette.Gray
	};
	bool charging = false;
	bool waitingForBalls = false;
	bool scored = false;

	private IWindow _window;
	public BilliardTest()
	{

		_window = AzaleaGame.Main.Host.Window;
		_window.ClientSize = new(panelWidth, panelHeight);
		_window.Center();
		this.BackgroundColor = new Graphics.Colors.Color(48, 23, 8);
		var physics = AzaleaGame.Main.Host.Physics;
		physics.UsesGravity = false;
		physics.IsTopDown = true;
		Add(line = new Line()
		{
			StartPoint = new(500, 40),
			EndPoint = new(200, 400),
			Alpha = 0f
		});
		Add(aimLine);

		Add(tableMat = new Box()
		{
			Position = new(panelWidth / 2, panelHeight / 2),
			Size = new(tableWidth, tableHeight),
			Color = Palette.Green,
			Origin = Graphics.Anchor.Center,
			Depth = 25
			//Rotation = 15
		});

		GenerateBalls();

		topLeftHolePosition = new Vector2(panelWidth / 2 - tableWidth / 2 + holeRadius / 2, panelHeight / 2 - tableHeight / 2 + holeRadius / 2);
		topMiddleHolePosition = new Vector2(panelWidth / 2, panelHeight / 2 - tableHeight / 2);
		topRightHolePosition = new Vector2(panelWidth / 2 + tableWidth / 2 - holeRadius / 2, panelHeight / 2 - tableHeight / 2 + holeRadius / 2);
		bottomLeftHolePosition = new Vector2(panelWidth / 2 - tableWidth / 2 + holeRadius / 2, panelHeight / 2 + tableHeight / 2 - holeRadius / 2);
		bottomMiddleHolePosition = new Vector2(panelWidth / 2, panelHeight / 2 + tableHeight / 2);
		bottomRightHolePosition = new Vector2(panelWidth / 2 + tableWidth / 2 - holeRadius / 2, panelHeight / 2 + tableHeight / 2 - holeRadius / 2);
		GenerateHoles();

		player1 = new Player("Toma");
		player2 = new Player("Tod");
		currentPlayer = player1;

		GenerateHoleCorners();
		GenerateEdges();

		GenerateUI();

		GenerateInfo();
	}

	private void GenerateInfo()
	{
		Add(infoContainer = new FlexContainer()
		{
			Size = new(1400, 800),
			Origin = Graphics.Anchor.Center,
			Anchor = Graphics.Anchor.Center,
			BorderThickness = new Graphics.Boundary(3),
			BackgroundColor = Palette.Cyan,
			Alpha = 0,
		}); ;
		infoContainer.Add(new Sprite
		{
			RelativeSizeAxes = Graphics.Axes.Both,
			Texture = Assets.GetTexture("Textures/OpisRada.png")

		});
	}

	private void GenerateUI()
	{
		Composition uiComp;
		Add(uiComp = new Composition()
		{
			Origin = Graphics.Anchor.TopCenter,
			Anchor = Graphics.Anchor.TopCenter,
			Size = new Vector2(panelWidth * 0.9f, panelHeight),
			Position = new Vector2(0, 50),
		});
		uiComp.Add(player1Text = new SpriteText()
		{
			Origin = Graphics.Anchor.TopLeft,
			Anchor = Graphics.Anchor.TopLeft,
			Font = FontUsage.Default.With(size: 50),
			Text = player1.ToString(),
		});
		uiComp.Add(player2Text = new SpriteText()
		{
			Origin = Graphics.Anchor.TopRight,
			Anchor = Graphics.Anchor.TopRight,
			Font = FontUsage.Default.With(size: 50),
			Text = player2.ToString(),
		});
		uiComp.Add(targetBallText = new SpriteText()
		{
			Origin = Graphics.Anchor.TopCenter,
			Anchor = Graphics.Anchor.TopCenter,
			Font = FontUsage.Default.With(size: 50),
			Text = $"Target Color: {BallType.Red.ToString()}",
		});
		highlightPlayers();
	}
	private void GenerateEdges()
	{
		Add(topLeftEdge = new Box()
		{
			Position = new(panelWidth / 2 - tableWidth / 4 + holeRadius / 2, panelHeight / 2 - tableHeight / 2),
			Size = new(tableWidth / 2 - 3 * holeRadius - holeOffset - middleOffset + 3, edgeThickness),
			Color = new Graphics.Colors.Color(20, 9, 3),
			Origin = Graphics.Anchor.Center,
			Depth = 20,
			//Rotation = 15
		});
		topLeftEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		topLeftEdge.AddComponent(new RectCollider());

		Add(topRightEdge = new Box()
		{
			Position = new(panelWidth / 2 + tableWidth / 4 - holeRadius / 2, panelHeight / 2 - tableHeight / 2),
			Size = new(tableWidth / 2 - 3 * holeRadius - holeOffset - middleOffset + 3, edgeThickness),
			Color = new Graphics.Colors.Color(20, 9, 3),
			Origin = Graphics.Anchor.Center,
			Depth = 20,
			//Rotation = 15
		});
		topRightEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		topRightEdge.AddComponent(new RectCollider());

		Add(bottomLeftEdge = new Box()
		{
			Position = new(panelWidth / 2 - tableWidth / 4 + holeRadius / 2, panelHeight / 2 + tableHeight / 2),
			Size = new(tableWidth / 2 - 3 * holeRadius - holeOffset - middleOffset + 3, edgeThickness),
			Color = new Graphics.Colors.Color(20, 9, 3),
			Origin = Graphics.Anchor.Center,
			Depth = 20,
			//Rotation = 15
		});
		bottomLeftEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		bottomLeftEdge.AddComponent(new RectCollider());

		Add(bottomRightEdge = new Box()
		{
			Position = new(panelWidth / 2 + tableWidth / 4 - holeRadius / 2, panelHeight / 2 + tableHeight / 2),
			Size = new(tableWidth / 2 - 3 * holeRadius - holeOffset - middleOffset + 3, edgeThickness),
			Color = new Graphics.Colors.Color(20, 9, 3),
			Origin = Graphics.Anchor.Center,
			Depth = 20,
			//Rotation = 15
		});
		bottomRightEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		bottomRightEdge.AddComponent(new RectCollider());

		Add(leftEdge = new Box()
		{
			Position = new(panelWidth / 2 - tableWidth / 2, panelHeight / 2),
			Size = new(edgeThickness, tableHeight - holeRadius * 3 - holeOffset - middleOffset - holeOffset - 10),
			Color = new Graphics.Colors.Color(20, 9, 3),
			Origin = Graphics.Anchor.Center,
			Depth = 20,
			//Rotation = 15
		});
		leftEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		leftEdge.AddComponent(new RectCollider());

		Add(rightEdge = new Box()
		{
			Position = new(panelWidth / 2 + tableWidth / 2, panelHeight / 2),
			Size = new(edgeThickness, tableHeight - holeRadius * 3 - holeOffset - middleOffset - holeOffset - 10),
			Color = new Graphics.Colors.Color(20, 9, 3),
			Origin = Graphics.Anchor.Center,
			Depth = 20,
			//Rotation = 15
		});
		rightEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		rightEdge.AddComponent(new RectCollider());
	}
	private void GenerateHoleCorners()
	{
		topLeftHoleEdgeLeft = GenerateHoleCorner(new Vector2(topLeftHolePosition.X - holeRadius / 2 + 5, topLeftHolePosition.Y + holeRadius * 1.5f), new(holeRadius / 2, holeRadius * 2), -40);
		topRightHoleEdgeRight = GenerateHoleCorner(new Vector2(topRightHolePosition.X + holeRadius / 2 - 5, topRightHolePosition.Y + holeRadius * 1.5f), new(holeRadius / 2, holeRadius * 2), 40);

		topLeftHoleEdgeRight = GenerateHoleCorner(new Vector2(topLeftHolePosition.X + holeRadius * 1.5f, topLeftHolePosition.Y - 0.5f * holeRadius + 5), new(holeRadius / 2, holeRadius * 2), -50);
		topRightHoleEdgeLeft = GenerateHoleCorner(new Vector2(topRightHolePosition.X - holeRadius * 1.5f, topRightHolePosition.Y - 0.5f * holeRadius + 5), new(holeRadius / 2, holeRadius * 2), 50);

		topMiddleHoleEdgeLeft = GenerateHoleCorner(new Vector2(topMiddleHolePosition.X - 1.5f * holeRadius, topMiddleHolePosition.Y + holeRadius / 2 - 1), new(holeRadius / 2, holeRadius), 35);
		topMiddleHoleEdgeRight = GenerateHoleCorner(new Vector2(topMiddleHolePosition.X + 1.5f * holeRadius, topMiddleHolePosition.Y + holeRadius / 2 - 1), new(holeRadius / 2, holeRadius), -35);

		bottomMiddleHoleEdgeLeft = GenerateHoleCorner(new Vector2(bottomMiddleHolePosition.X - 1.5f * holeRadius, bottomMiddleHolePosition.Y - holeRadius / 2 + 1), new(holeRadius / 2, holeRadius), -35);
		bottomMiddleHoleEdgeRight = GenerateHoleCorner(new Vector2(bottomMiddleHolePosition.X + 1.5f * holeRadius, bottomMiddleHolePosition.Y - holeRadius / 2 + 1), new(holeRadius / 2, holeRadius), +35);

		bottomLeftHoleEdgeRight = GenerateHoleCorner(new Vector2(bottomLeftHolePosition.X + holeRadius * 1.5f, bottomLeftHolePosition.Y + 0.5f * holeRadius - 5), new(holeRadius / 2, holeRadius * 2), 50);
		bottomRightHoleEdgeLeft = GenerateHoleCorner(new Vector2(bottomRightHolePosition.X - holeRadius * 1.5f, bottomRightHolePosition.Y + 0.5f * holeRadius - 5), new(holeRadius / 2, holeRadius * 2), -50);

		bottomLeftHoleEdgeLeft = GenerateHoleCorner(new Vector2(bottomLeftHolePosition.X - holeRadius / 2 + 5, bottomLeftHolePosition.Y - holeRadius * 1.5f), new(holeRadius / 2, holeRadius * 2), 40);
		bottomRightHoleEdgeRight = GenerateHoleCorner(new Vector2(bottomRightHolePosition.X + holeRadius / 2 - 5, bottomRightHolePosition.Y - holeRadius * 1.5f), new(holeRadius / 2, holeRadius * 2), -40);
	}

	private Box GenerateHoleCorner(Vector2 position, Vector2 size, float angle)
	{
		Box box;
		Add(box = topLeftEdge = new Box()
		{
			Position = position,
			Size = size,
			Color = new Graphics.Colors.Color(67, 31, 21),
			Origin = Graphics.Anchor.Center,
			Depth = 19,
			Rotation = angle
		});
		topLeftEdge.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		topLeftEdge.AddComponent(new RectCollider());
		return box;
	}


	private void GenerateBalls()
	{
		Add(whiteBall = new SnookerBall(whitePosition, Palette.White, 0, BallType.White));
		Add(blackBall = new SnookerBall(blackPosition, Palette.Black, 7, BallType.Color));
		Add(blueBall = new SnookerBall(bluePosition, Palette.Blue, 5, BallType.Color));
		Add(yellowBall = new SnookerBall(yellowPosition, Palette.Yellow, 2, BallType.Color));
		Add(greenBall = new SnookerBall(greenPosition, Palette.Lime, 3, BallType.Color));
		Add(brownBall = new SnookerBall(brownPosition, Palette.Brown, 4, BallType.Color));
		Add(pinkBall = new SnookerBall(pinkPosition, Palette.Pink, 6, BallType.Color));
		int redRow = 1;
		int numInRow = 0;
		for (int i = 0; i < 15; i++)
		{
			Vector2 position = redPosition;
			Vector2 startPos = new Vector2(position.X - 10 * 2 * (redRow - 1), position.Y + 10 * (redRow - 1) - 10 * 2 * numInRow);
			Add(redBalls[i] = new SnookerBall(startPos, Palette.Red, 1, BallType.Red));
			AllBalls.Add(redBalls[i]);
			numInRow++;
			if (numInRow >= redRow)
			{
				redRow++;
				numInRow = 0;
			}

		}
		AllBalls.Add(whiteBall);
		AllBalls.Add(blackBall);
		AllBalls.Add(blueBall);
		AllBalls.Add(yellowBall);
		AllBalls.Add(greenBall);
		AllBalls.Add(brownBall);
		AllBalls.Add(pinkBall);
	}

	public Sprite GenerateHole(Vector2 position)
	{
		Sprite hole;
		Add(hole = new Sprite()
		{
			Position = position,
			Size = new(holeRadius * 2, holeRadius * 2),
			Origin = Graphics.Anchor.Center,
			Color = new Graphics.Colors.Color(27, 27, 27),
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 6
		});
		hole.AddComponent(new RigidBody()
		{
			Mass = 10,
			IsDynamic = false,
		});
		Collider holeCollider;
		hole.AddComponent(holeCollider = new CircleCollider()
		{
			Radius = 8,
			IsTrigger = true,
		});
		holeCollider.OnCollision += (other) =>
		{
			if (other.Parent.Alpha > 0.5f)
				Score(holeCollider, other);
		};
		return hole;
	}
	public void GenerateHoles()
	{
		topLeftHole = GenerateHole(topLeftHolePosition);
		topMiddleHole = GenerateHole(topMiddleHolePosition);
		topRightHole = GenerateHole(topRightHolePosition);
		bottomLeftHole = GenerateHole(bottomLeftHolePosition);
		bottomMiddleHole = GenerateHole(bottomMiddleHolePosition);
		bottomRightHole = GenerateHole(bottomRightHolePosition);
	}
	protected override void Update()
	{
		if (Input.GetKey(Keys.P).Down)
		{
			Console.WriteLine($"Circle1 Position:{whiteBall.Position}");
		}

		if (Input.GetKey(Keys.Tilde).Down)
		{
			infoContainer.Alpha = infoContainer.Alpha != 1f ? 1f : 0f;
		}

		/*
		Console.WriteLine($"Box1 Position: {box1.Position}");
		Console.WriteLine($"Circle1 Position: {circle1.Position}");
		Console.WriteLine($"Circle2 Position: {circle2.Position}");
		Console.WriteLine($"Circle3 Position: {circle3.Position}");
	   */
		if (Input.GetKey(Keys.W).Pressed)
		{
			whiteBall.Position += new Vector2(0, -2);
		}
		if (Input.GetKey(Keys.A).Pressed)
		{
			whiteBall.Position += new Vector2(-2, 0);
		}
		if (Input.GetKey(Keys.S).Pressed)
		{
			whiteBall.Position += new Vector2(0, 2);
		}
		if (Input.GetKey(Keys.D).Pressed)
		{
			whiteBall.Position += new Vector2(2, 0);
		}

		if (Input.GetKey(Keys.E).Pressed)
		{
			whiteBall.Rotation += 3f;
		}
		if (Input.GetKey(Keys.Q).Pressed)
		{
			whiteBall.Rotation -= 3f;
		}

		line.StartPoint = whiteBall.Position;
		line.EndPoint = Input.MousePosition;
		CircleCollider crCol = whiteBall.GetComponent<CircleCollider>();
		if (Input.MousePosition.X < whiteBall.Position.X + crCol.Radius && Input.MousePosition.X > whiteBall.Position.X - crCol.Radius
			&& Input.MousePosition.Y < whiteBall.Position.Y + crCol.Radius && Input.MousePosition.Y > whiteBall.Position.Y - crCol.Radius)
		{
			if (Input.GetMouseButton(MouseButton.Left).Down && waitingForBalls == false)
			{
				line.Alpha = 1;
				charging = true;
			}
		}

		Vector2 directionVector = Vector2.Normalize(whiteBall.Position - Input.MousePosition);
		if (charging && Input.GetMouseButton(MouseButton.Left).Released)
		{
			charging = false;
			line.Alpha = 0;


			float power = 4f;
			float distance = Vector2.Distance(Input.MousePosition, whiteBall.Position);
			power *= 1 + distance / 10;
			whiteBall.GetComponent<RigidBody>().ApplyForce(directionVector, power);
			waitingForBalls = true;
		}

		if (waitingForBalls == false)
		{
			aimLine.Alpha = 1;
			float angle = MathF.Atan2(directionVector.Y, directionVector.X);
			//	Console.WriteLine($"Angle: {angle}");
			Ray ray = new Ray(whiteBall.Position, angle, 1500)
			{
				MinimumRange = 13,
			};
			ray = RayCast.Cast(ray);
			//		if (ray.Hit)
			//			Console.WriteLine("HIT");
			aimLine.StartPoint = whiteBall.Position;
			aimLine.EndPoint = aimLine.StartPoint + MathUtils.GetDirectionFromAngle(angle) * ray.Distance;
		}
		else
			aimLine.Alpha = 0;
	}
	protected override void FixedUpdate()
	{
		if (waitingForBalls)
		{
			bool allStopped = true;
			foreach (RigidBody body in ComponentStorage<RigidBody>.GetComponents())
			{
				if (body.Parent is SnookerBall ball && body.Velocity.Length() > 0.01 && ball.Alpha > 0.5)
				{
					allStopped = false;
				}
			}

			if (allStopped)
			{
				waitingForBalls = false;
				FinishTurn();
			}
		}

	}

	private void Score(Collider holeCollider, Collider other)
	{
		if (other.Parent is SnookerBall ball)
		{
			ball.Alpha = 0.3f;

			//if not foul	if()
			//	currentPlayer.Score += ball.PointValue;
			ball.GetComponent<RigidBody>().Velocity = new Vector2(0, 0);
			ball.Position = new(-500, -500);
			player1Text.Text = player1.ToString();
			player2Text.Text = player2.ToString();
			scoredBallsThisTurn.Add(ball);
			scored = true;

		}
	}

	private void Foul(Player player)
	{
		player.Score -= 4;
		player1Text.Text = player1.ToString();
		player2Text.Text = player2.ToString();

		foreach (SnookerBall ball in scoredBallsThisTurn)
		{
			ball.Alpha = 1;
		}

		foreach (var ball in AllBalls.Where(x => x.Alpha > 0.5))
		{
			ball.GetComponent<RigidBody>().Velocity = new(0, 0);
			ball.GetComponent<RigidBody>().IsDynamic = false;
		}
		foreach (var ball in AllBalls.Where(x => x.Alpha > 0.5))
		{
			ball.Position = ball.PreviousPosition;
		}
		foreach (var ball in AllBalls.Where(x => x.Alpha > 0.5))
		{
			ball.GetComponent<RigidBody>().IsDynamic = true;
		}


	}
	private void FinishTurn()
	{
		bool fouled = false;
		Console.WriteLine($"Scored Balls: {scoredBallsThisTurn.Count()}");
		if (scoredBallsThisTurn.Count() != 0)
		{
			if (scoredBallsThisTurn.Where(x => x.Type != targetBallType).Count() > 0)
			{
				fouled = true;
				Foul(currentPlayer);
				currentPlayer = currentPlayer != player1 ? player1 : player2;
				if (AllBalls.Where(x => x.Alpha > 0.5 && x.Type == BallType.Red).Count() > 0)
					targetBallType = BallType.Red;
				else
				{
					targetBallType = BallType.Color;
				}
			}
			else if (scoredBallsThisTurn.Where(x => x.Type == targetBallType).Count() > 0)
			{
				if (firstHitBallType == targetBallType)
				{
					if (AllBalls.Where(x => x.Alpha > 0.5 && x.Type == BallType.Red).Count() > 0)
						targetBallType = targetBallType != BallType.Red ? BallType.Red : BallType.Color;
					else
					{
						targetBallType = BallType.Color;
					}
					currentPlayer.Score += scoredBallsThisTurn.Sum(x => x.PointValue);
					player1Text.Text = player1.ToString();
					player2Text.Text = player2.ToString();

					foreach (var ball in scoredBallsThisTurn.Where(x => x.Type == BallType.Color))
					{
						resetBall(ball);
					}
				}
				else
				{
					fouled = true;
					Foul(currentPlayer);
					currentPlayer = currentPlayer != player1 ? player1 : player2;
					if (AllBalls.Where(x => x.Alpha > 0.5 && x.Type == BallType.Red).Count() > 0)
						targetBallType = BallType.Red;
					else
					{
						targetBallType = BallType.Color;
					}
				}
			}
		}
		else
		{
			if (firstHitBallType == targetBallType)
			{
				currentPlayer = currentPlayer != player1 ? player1 : player2;
				if (AllBalls.Where(x => x.Alpha > 0.5 && x.Type == BallType.Red).Count() > 0)
					targetBallType = BallType.Red;
				else
				{
					targetBallType = BallType.Color;
				}
			}
			else
			{
				fouled = true;
				Foul(currentPlayer);
				currentPlayer = currentPlayer != player1 ? player1 : player2;
				if (AllBalls.Where(x => x.Alpha > 0.5 && x.Type == BallType.Red).Count() > 0)
					targetBallType = BallType.Red;
				else
				{
					targetBallType = BallType.Color;
				}
			}

		}


		if (fouled == false)
		{
			foreach (SnookerBall ball in AllBalls)
			{
				ball.PreviousPosition = ball.Position;
			}
			foreach (SnookerBall ball in scoredBallsThisTurn)
			{
				resetBall(ball);
				ball.PreviousPosition = ball.Position;
			}
		}
		if (firstHitBallType == BallType.None)
			targetBallType = BallType.Red;

		firstHitBallType = BallType.None;
		targetBallText.Text = $"Target Color: {targetBallType}";
		highlightPlayers();



		scored = false;
		scoredBallsThisTurn.Clear();
	}

	private void highlightPlayers()
	{
		if (currentPlayer == player1)
		{
			player1Text.Color = Palette.Gold;
			player2Text.Color = Palette.White;
		}

		else
		{
			player1Text.Color = Palette.White;
			player2Text.Color = Palette.Gold;
		}
	}

	private void resetBall(SnookerBall ball)
	{
		if (ball.Type == BallType.Color)
		{
			if (AllBalls.Where(x => x.Alpha > 0.5f && x.Type == BallType.Red).ToList().Count() > 0)
			{
				ball.Alpha = 1;
				if (greenBall == ball)
					ball.Position = greenPosition;
				if (brownBall == ball)
					ball.Position = brownPosition;
				if (yellowBall == ball)
					ball.Position = yellowPosition;
				if (blackBall == ball)
					ball.Position = blackPosition;
				if (blueBall == ball)
					ball.Position = bluePosition;
				if (pinkBall == ball)
					ball.Position = pinkPosition;
			}
		}
	}

	private enum BallType { None, White, Color, Red };
	private class SnookerBall : Composition
	{
		public SnookerBall(Vector2 position, Color color, int pointValue, BallType type)
		{
			PreviousPosition = position;
			Position = position;
			Size = new(20, 20);
			Origin = Graphics.Anchor.Center;
			Color = color;
			Depth = 5;
			Type = type;
			PointValue = pointValue;

			Add(BallSprite = new Sprite()
			{
				RelativeSizeAxes = Graphics.Axes.Both,

				Texture = Assets.GetTexture("Textures/Ball.png"),
			});
			AddComponent(new RigidBody()
			{
				Mass = 10,
				UsesGravity = true,
				Restitution = 0.9f,
			});
			Collider collider;
			AddComponent(collider = new CircleCollider()
			{
				Radius = 10
			});
			if (Type == BallType.White)
			{
				collider.OnCollision += (other) =>
				{
					if (other.Parent is SnookerBall ball)
					{
						if (ball.GetFirstParentOfType<BilliardTest>().firstHitBallType == BallType.None)
							ball.GetFirstParentOfType<BilliardTest>().firstHitBallType = ball.Type;
					}
				};
			}

		}

		public Sprite BallSprite;
		public int PointValue;
		public BallType Type;
		public Vector2 PreviousPosition;

	}

	private class Player
	{
		public Player(string name)
		{
			Name = name;

		}

		public string Name;
		public int Score;
		public override string ToString()
		{
			return $"{Name}: {Score}";
		}
	}
}
