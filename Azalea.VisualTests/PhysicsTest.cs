using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Physics;
using Azalea.Physics.Colliders;
using System;
using System.Numerics;

namespace Azalea.VisualTests;
public class PhysicsTest : TestScene
{
	public Box box1;
	public Box box2;
	public Box box3;
	public Box box4;

	Box platform;
	Box ceiling;
	Box platform2;
	Box platform3;
	Box platform4;
	Box wallL;
	Box wallR;

	Sprite circle1;
	Sprite circle2;
	Sprite circle3;
	Sprite circle4;

	Sprite testCircle1;
	Sprite testCircle2;

	Line line;
	bool charging = false;
	private PhysicsGenerator _physics;
	public PhysicsTest()
	{
		_physics = AzaleaGame.Main.Host.Physics;
		_physics.DebugMode = true;
		_physics.UsesGravity = false;
		Add(line = new Line()
		{
			StartPoint = new(500, 40),
			EndPoint = new(200, 400),
			Alpha = 0f
		});

		Add(circle1 = new Sprite()
		{
			Position = new(1000, 100),
			Size = new(50, 50),
			Color = Palette.Blue,
			Origin = Graphics.Anchor.Center,
			Texture = Assets.GetTexture("Textures/Circle.png")
		});
		circle1.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
			Restitution = 0.6f,
		});
		circle1.AddComponent(new CircleCollider()
		{
			Radius = 25
		});
		/*
				Add(testCircle1 = new Sprite()
				{
					Position = new(200, 400),
					Size = new(30, 30),
					Color = Palette.Purple,
					Origin = Graphics.Anchor.Center,
					Texture = Assets.GetTexture("Textures/Circle.png")
				});
				testCircle1.AddComponent(new RigidBody()
				{
					Mass = 5,
					UsesGravity = true,
					Restitution = 0.5f
				});
				testCircle1.AddComponent(new CircleCollider()
				{
					Radius = 15
				});

				Add(testCircle2 = new Sprite()
				{
					Position = new(900, 400),
					Size = new(30, 30),
					Color = Palette.Silver,
					Origin = Graphics.Anchor.Center,
					Texture = Assets.GetTexture("Textures/Circle.png")
				});
				testCircle2.AddComponent(new RigidBody()
				{
					Mass = 5,
					UsesGravity = true,
					Restitution = 0.5f
				});
				testCircle2.AddComponent(new CircleCollider()
				{
					Radius = 15,

				});*/
		/*
		Add(circle2 = new Sprite()
		{
			Position = new(610, 200),
			Size = new(80, 80),
			Color = Palette.Red,
			Origin = Graphics.Anchor.Center,
			Texture = Assets.GetTexture("Textures/Circle.png")
		});
		circle2.AddComponent(new RigidBody()
		{
			Mass = 1500,
		//	UsesGravity = false,
		});
		circle2.AddComponent(new CircleCollider()
		{
			Radius = 40
		});

		Add(circle3 = new Sprite()
		{
			Position = new(400, 400),
			Size = new(30, 30),
			Color = Palette.Green,
			Origin = Graphics.Anchor.Center,
			Texture = Assets.GetTexture("Textures/Circle.png")
		});
		circle3.AddComponent(new RigidBody()
		{
			Mass = 5,
	//		UsesGravity = false,
		});
		circle3.AddComponent(new CircleCollider()
		{
			Radius = 15
		});
		*/
		Add(box1 = new Box()
		{
			Position = new(690, 356),
			Size = new(50, 50),
			Color = Palette.Yellow,
			Origin = Graphics.Anchor.Center,
		});
		box1.AddComponent(new RigidBody()
		{
			Mass = 10,
			//	UsesGravity = false,
			//AngularAcceleration = 0.0001f
		});
		box1.AddComponent(new RectCollider()
		{
			SideA = 50,
			SideB = 50
		});

		Add(box2 = new Box()
		{
			Position = new(790, 356),
			Size = new(50, 50),
			Color = Palette.Yellow,
			Origin = Graphics.Anchor.Center
		});
		box2.AddComponent(new RigidBody()
		{
			Mass = 10,
			IsDynamic = false,
			//	UsesGravity = false,
			//AngularAcceleration = 0.0001f
		});
		box2.AddComponent(new RectCollider()
		{
			SideA = 50,
			SideB = 50
		});

		Add(platform = new Box()
		{
			Position = new(600, 600),
			Size = new(1500, 100),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			//Rotation = 15
		});
		platform.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		platform.AddComponent(new RectCollider()
		{
			SideA = 1500,
			SideB = 100
		});

		Add(ceiling = new Box()
		{
			Position = new(600, -80),
			Size = new(1500, 100),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			//Rotation = 15
		});
		ceiling.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		ceiling.AddComponent(new RectCollider()
		{
			SideA = 1500,
			SideB = 100
		});

		Add(platform2 = new Box()
		{
			Position = new(600, 450),
			Size = new(200, 60),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			Rotation = 120
		});
		platform2.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		platform2.AddComponent(new RectCollider()
		{
			SideA = 200,
			SideB = 60
		});

		Add(platform3 = new Box()
		{
			Position = new(100, 150),
			Size = new(200, 60),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			Rotation = 15
		});
		platform3.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		platform3.AddComponent(new RectCollider()
		{
			SideA = 200,
			SideB = 60
		});

		Add(platform4 = new Box()
		{
			Position = new(700, 150),
			Size = new(400, 60),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			Rotation = 5
		});
		platform4.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		platform4.AddComponent(new RectCollider()
		{
			SideA = 400,
			SideB = 60
		});

		Add(wallL = new Box()
		{
			Position = new(-20, 450),
			Size = new(50, 900),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			//Rotation = 15
		});
		wallL.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		wallL.AddComponent(new RectCollider()
		{
			SideA = 50,
			SideB = 900
		});

		Add(wallR = new Box()
		{
			Position = new(1300, 450),
			Size = new(50, 900),
			Color = Palette.Brown,
			Origin = Graphics.Anchor.Center,
			//Rotation = 15
		});
		wallR.AddComponent(new RigidBody()
		{
			Mass = 1000000,
			UsesGravity = false,
			IsDynamic = false,
			//AngularAcceleration = 0.0001f
		});
		wallR.AddComponent(new RectCollider()
		{
			SideA = 50,
			SideB = 900
		});

	}

	protected override void Update()
	{

		if (Input.GetKey(Keys.Space).Down)
		{
			_physics.UsesGravity = true;
		}

		if (Input.GetKey(Keys.O).Down)
		{
			Add(testCircle1 = new Sprite()
			{
				Position = new(200, 400),
				Size = new(30, 30),
				Color = Palette.Purple,
				Origin = Graphics.Anchor.Center,
				Texture = Assets.GetTexture("Textures/Circle.png")
			});
			testCircle1.AddComponent(new RigidBody()
			{
				Mass = 5,
				UsesGravity = true,
			});
			testCircle1.AddComponent(new CircleCollider()
			{
				Radius = 15
			});

			Add(testCircle2 = new Sprite()
			{
				Position = new(600, 400),
				Size = new(30, 30),
				Color = Palette.Silver,
				Origin = Graphics.Anchor.Center,
				Texture = Assets.GetTexture("Textures/Circle.png")
			});
			testCircle2.AddComponent(new RigidBody()
			{
				Mass = 5,
				UsesGravity = true,
			});
			testCircle2.AddComponent(new CircleCollider()
			{
				Radius = 15
			});

		}

		if (Input.GetKey(Keys.P).Down)
		{
			Console.WriteLine($"Circle1 Position:{circle1.Position}");
		}
		/*
		Console.WriteLine($"Box1 Position: {box1.Position}");
		Console.WriteLine($"Circle1 Position: {circle1.Position}");
		Console.WriteLine($"Circle2 Position: {circle2.Position}");
		Console.WriteLine($"Circle3 Position: {circle3.Position}");
	   */
		if (Input.GetKey(Keys.W).Pressed)
		{
			circle1.Position += new Vector2(0, -2);

		}
		if (Input.GetKey(Keys.A).Pressed)
		{
			circle1.Position += new Vector2(-2, 0);
		}
		if (Input.GetKey(Keys.S).Pressed)
		{
			circle1.Position += new Vector2(0, 2);
		}
		if (Input.GetKey(Keys.D).Pressed)
		{
			circle1.Position += new Vector2(2, 0);
		}

		if (Input.GetKey(Keys.E).Pressed)
		{
			circle1.Rotation += 3f;
		}
		if (Input.GetKey(Keys.Q).Pressed)
		{
			circle1.Rotation -= 3f;
		}

		line.StartPoint = circle1.Position;
		line.EndPoint = Input.MousePosition;
		CircleCollider crCol = circle1.GetComponent<CircleCollider>();
		if (Input.MousePosition.X < circle1.Position.X + crCol.Radius && Input.MousePosition.X > circle1.Position.X - crCol.Radius
			&& Input.MousePosition.Y < circle1.Position.Y + crCol.Radius && Input.MousePosition.Y > circle1.Position.Y - crCol.Radius)
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
			Vector2 directionVector = Vector2.Normalize(circle1.Position - Input.MousePosition);

			float power = 2f;
			float distance = Vector2.Distance(Input.MousePosition, circle1.Position);
			power *= 1 + distance / 10;
			circle1.GetComponent<RigidBody>().ApplyForce(directionVector, power);
		}
	}
	protected override void FixedUpdate()
	{
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
}
