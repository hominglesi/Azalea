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
using System;
using System.Linq;
using System.Numerics;

namespace Azalea.VisualTests;
public class BilliardTest : TestScene
{
	PhysicsGenerator PGen = new PhysicsGenerator();
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
	Box tableMat;


	Box wallL;
	Box wallR;

	float holeRadius = 25;

	Sprite whiteBall;
	Sprite[] redBalls = new Sprite[15];
	Sprite blueBall;
	Sprite yellowBall;
	Sprite blackBall;
	Sprite greenBall;
	Sprite brownBall;
	Sprite pinkBall;

	Sprite topLeftHole;
	Sprite topMiddleHole;
	Sprite topRightHole;
	Sprite bottomLeftHole;
	Sprite bottomMiddleHole;
	Sprite bottomRightHole;


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
	bool charging = false;

	private IWindow _window;
	public BilliardTest()
	{

		_window = AzaleaGame.Main.Host.Window;
		_window.ClientSize = new(panelWidth, panelHeight);
		_window.Center();
		this.BackgroundColor = new Graphics.Colors.Color(48, 23, 8);
		PGen.UsesGravity = false;
		PGen.IsTopDown = true;
		Add(line = new Line()
		{
			StartPoint = new(500, 40),
			EndPoint = new(200, 400),
			Alpha = 0f
		});

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


		Add(topLeftEdge = new Box()
		{
			Position = new(panelWidth / 2 - tableWidth / 4, panelHeight / 2 - tableHeight / 2),
			Size = new(tableWidth / 4 - holeRadius, edgeThickness),
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
		topLeftEdge.AddComponent(new RectCollider()
		{
			SideA = tableWidth / 2,
			SideB = edgeThickness
		});
		/*
		Add(topRightEdge = new Box()
		{
			Position = new(panelWidth / 2 + tableWidth / 4, panelHeight / 2 - tableHeight / 2),
			Size = new(tableWidth / 4 - -3 * holeRadius, edgeThickness),
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
		topRightEdge.AddComponent(new RectCollider()
		{
			SideA = tableWidth,
			SideB = edgeThickness
		});*/

		Add(bottomLeftEdge = new Box()
		{
			Position = new(panelWidth / 2, panelHeight / 2 + tableHeight / 2),
			Size = new(tableWidth, edgeThickness),
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
		bottomLeftEdge.AddComponent(new RectCollider()
		{
			SideA = tableWidth,
			SideB = edgeThickness
		});
		/*
		Add(bottomRightEdge = new Box()
		{
			Position = new(panelWidth / 2, panelHeight / 2 + tableHeight / 2),
			Size = new(tableWidth, edgeThickness),
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
		bottomRightEdge.AddComponent(new RectCollider()
		{
			SideA = tableWidth,
			SideB = edgeThickness
		});*/

		Add(leftEdge = new Box()
		{
			Position = new(panelWidth / 2 - tableWidth / 2, panelHeight / 2),
			Size = new(edgeThickness, tableHeight),
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
		leftEdge.AddComponent(new RectCollider()
		{
			SideA = edgeThickness,
			SideB = tableHeight
		});

		Add(rightEdge = new Box()
		{
			Position = new(panelWidth / 2 + tableWidth / 2, panelHeight / 2),
			Size = new(edgeThickness, tableHeight),
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
		rightEdge.AddComponent(new RectCollider()
		{
			SideA = edgeThickness,
			SideB = tableHeight
		});
		topLeftHolePosition = new Vector2(panelWidth / 2 - tableWidth / 2, panelHeight / 2 - tableHeight / 2);
		topMiddleHolePosition = new Vector2(panelWidth / 2, panelHeight / 2 - tableHeight / 2);
		topRightHolePosition = new Vector2(panelWidth / 2 + tableWidth / 2, panelHeight / 2 - tableHeight / 2);
		bottomLeftHolePosition = new Vector2(panelWidth / 2 - tableWidth / 2, panelHeight / 2 + tableHeight / 2);
		bottomMiddleHolePosition = new Vector2(panelWidth / 2, panelHeight / 2 + tableHeight / 2);
		bottomRightHolePosition = new Vector2(panelWidth / 2 + tableWidth / 2, panelHeight / 2 + tableHeight / 2);
		GenerateHoles();


	}

	private void GenerateBalls()
	{
		Add(whiteBall = new Sprite()
		{
			Position = whitePosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		whiteBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		whiteBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});

		Add(blueBall = new Sprite()
		{
			Position = bluePosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Color = Palette.Blue,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		blueBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		blueBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});
		int redRow = 1;
		int numInRow = 0;
		for (int i = 0; i < 15; i++)
		{
			Vector2 position = redPosition;
			Vector2 startPos = new Vector2(position.X - 15 * 2 * (redRow - 1), position.Y + 15 * (redRow - 1) - 15 * 2 * numInRow);
			Add(redBalls[i] = new Sprite()
			{

				Position = startPos,
				Size = new(30, 30),
				Origin = Graphics.Anchor.Center,
				Color = Palette.Red,
				Texture = Assets.GetTexture("Textures/Ball.png"),
				Depth = 5
			});
			redBalls[i].AddComponent(new RigidBody()
			{
				Mass = 10,
				//	IsDynamic=false,
				UsesGravity = true,
				Restitution = 0.9f,
			});
			redBalls[i].AddComponent(new CircleCollider()
			{
				Radius = 15
			});
			numInRow++;
			if (numInRow >= redRow)
			{
				redRow++;
				numInRow = 0;
			}
		}
		Add(blackBall = new Sprite()
		{
			Position = blackPosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Color = Palette.Black,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		blackBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		blackBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});

		Add(pinkBall = new Sprite()
		{
			Position = pinkPosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Color = Palette.Pink,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		pinkBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		pinkBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});

		Add(brownBall = new Sprite()
		{
			Position = brownPosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Color = Palette.Brown,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		brownBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		brownBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});

		Add(greenBall = new Sprite()
		{
			Position = greenPosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Color = Palette.Lime,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		greenBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		greenBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});

		Add(yellowBall = new Sprite()
		{
			Position = yellowPosition,
			Size = new(30, 30),
			Origin = Graphics.Anchor.Center,
			Color = Palette.Yellow,
			Texture = Assets.GetTexture("Textures/Ball.png"),
			Depth = 5
		});
		yellowBall.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.9f,
		});
		yellowBall.AddComponent(new CircleCollider()
		{
			Radius = 15
		});
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
		hole.AddComponent(new CircleCollider()
		{
			Radius = 10
		});
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
			if (Input.GetMouseButton(MouseButton.Left).Down)
			{
				line.Alpha = 1;
				charging = true;
			}
		}

		if (charging && Input.GetMouseButton(MouseButton.Left).Released)
		{
			charging = false;
			line.Alpha = 0;
			Vector2 directionVector = Vector2.Normalize(whiteBall.Position - Input.MousePosition);

			float power = 4f;
			float distance = Vector2.Distance(Input.MousePosition, whiteBall.Position);
			power *= 1 + distance / 10;
			whiteBall.GetComponent<RigidBody>().ApplyForce(directionVector, power);
		}
	}
	protected override void FixedUpdate()
	{
		if (Input.GetKey(Keys.G).Down)
			foreach (var ob in this.Children.Where(x => x.GetComponent<RigidBody>() != null).ToList())
				Console.WriteLine(ob.GetType());
		PGen.Update(ComponentStorage<RigidBody>.GetComponents());
		/*
		//Fake Floor
		if (box1.Y>600)
		{
			RigidBody rb = box1.GetComponent<RigidBody>();
			rb.Force = new(0,0);
			rb.Velocity = new(0, 0);
			rb.Position = new(rb.Position.X, 600);
		}
		if (box2.Y > 600)
		{
			RigidBody rb = box2.GetComponent<RigidBody>();
			rb.Force = new(0, 0);
			rb.Velocity = new(0, 0);
			rb.Position = new(rb.Position.X, 600);
		}*/
	}

	private class BilliardBall : Composition
	{

	}
}
