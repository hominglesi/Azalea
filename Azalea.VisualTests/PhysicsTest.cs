using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using Azalea.Simulations;
using Azalea.Simulations.Colliders;
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
	public PhysicsTest()
	{
		Physics.DebugMode = true;
		Physics.UsesGravity = false;
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

		Add(new Composition()
		{
			Position = new(690, 356),
			Child = box1 = new Box()
			{
				Size = new(50, 50),
				Color = Palette.Yellow,
				Origin = Graphics.Anchor.CenterRight,
			}
		});
		box1.AddComponent(new RigidBody()
		{
			Mass = 10,
			//	UsesGravity = false,
			//AngularAcceleration = 0.0001f
		});
		box1.AddComponent(new RectCollider());



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
		box2.AddComponent(new RectCollider());

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
		platform.AddComponent(new RectCollider());

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
		ceiling.AddComponent(new RectCollider());

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
		platform2.AddComponent(new RectCollider());

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
		platform3.AddComponent(new RectCollider());

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
		platform4.AddComponent(new RectCollider());

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
		wallL.AddComponent(new RectCollider());

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
		wallR.AddComponent(new RectCollider());

	}

	protected override void Update()
	{
		circle1.Position += App.Input.State.GetDirectionalMovement() * 2;

		if (App.Input.State.KeyPressed(Keys.E)) circle1.Rotation += 3f;
		if (App.Input.State.KeyPressed(Keys.Q)) circle1.Rotation -= 3f;

		var mousePos = App.Input.State.MousePosition;

		line.StartPoint = circle1.Position;
		line.EndPoint = mousePos;
		CircleCollider crCol = circle1.GetComponent<CircleCollider>();
		if (mousePos.X < circle1.Position.X + crCol.Radius && mousePos.X > circle1.Position.X - crCol.Radius
			&& mousePos.Y < circle1.Position.Y + crCol.Radius && mousePos.Y > circle1.Position.Y - crCol.Radius)
		{
			if (App.Input.State.MouseButtonPressed(MouseButton.Left))
			{
				line.Alpha = 1;
				charging = true;
			}
		}
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.Space)
			Physics.UsesGravity = true;
		else if (e.Key == Keys.O)
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
		else if (e.Key == Keys.P)
			Console.WriteLine($"Circle1 Position:{circle1.Position}");
		else
			return false;

		return true; ;
	}

	protected override void OnMouseUp(MouseUpEvent e)
	{
		if (charging == false)
			return;

		charging = false;
		line.Alpha = 0;
		Vector2 directionVector = Vector2.Normalize(circle1.Position - e.State.MousePosition);

		float power = 2f;
		float distance = Vector2.Distance(e.State.MousePosition, circle1.Position);
		power *= 1 + distance / 10;
		circle1.GetComponent<RigidBody>().ApplyForce(directionVector, power);
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
