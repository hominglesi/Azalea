using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Physics;
using Azalea.Physics.Colliders;
using System.Data;
using System.Linq;
using System.Numerics;

namespace Azalea.VisualTests;
public class PhysicsTest : TestScene
{
	PhysicsGenerator PGen = new PhysicsGenerator();
	public Box box1;
	public Box box2;
	public Box box3;
	public Box box4;

	Sprite circle1;
	Sprite circle2;
	Sprite circle3;
	Sprite circle4;

	bool charging = false;
	public PhysicsTest()
	{
		Add(new Line()
		{
			StartPoint = new(500, 40),
			EndPoint = new(200, 400)
		});

		/*
		Add(box1 = new Box()
		{
			Position = new(310, 200),
			Size = new(50, 50),
			Color = Palette.Blue,
			Origin= Graphics.Anchor.Center
		});
		box1.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = true,
		}) ;
		box1.AddComponent(new RectCollider()
		{
			SideA=50,
			SideB=50
		});

		Add(box2 = new Box()
		{
			Position = new(352, 400),
			Size = new(50, 50),
			Color = Palette.Green,
			Origin = Graphics.Anchor.Center
		});
		box2.AddComponent(new RectCollider()
		{
			SideA = 50,
			SideB = 50
		});
		box2.AddComponent(new RigidBody()
		{
			Mass = 30,
			UsesGravity = true,
			
		});

		Add(box3 = new Box()
		{
			Position = new(600, 200),
			Size = new(50, 50),
			Color = Palette.Red,
			Origin = Graphics.Anchor.Center
		});
		box3.AddComponent(new RigidBody()
		{
			Mass = 30,
			UsesGravity = false
		});
		box3.AddComponent(new RectCollider()
		{
			SideA = 50,
			SideB = 50
		});

		Add(box4 = new Box()
		{
			Position = new(200, 600),
			Size = new(1200, 100),
			Color = Palette.Orange,
		//	Origin = Graphics.Anchor.Center
		});
		box4.AddComponent(new RigidBody()
		{
			IsDynamic = false,
			Mass = 3000,
			UsesGravity = false
		}) ;
		box4.AddComponent(new RectCollider()
		{
			SideA = 1200,
			SideB = 100
		});*/

		Add(circle1 = new Sprite()
		{
			Position = new(310, 200),
			Size = new(50, 50),
			Color = Palette.Blue,
			Origin = Graphics.Anchor.Center,
			Texture = Assets.GetTexture("Textures/Circle.png")
		});
		circle1.AddComponent(new RigidBody()
		{
			Mass = 10,
			UsesGravity = false,
		});
		circle1.AddComponent(new CircleCollider()
		{
			Radius = 25
		});

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
			Mass = 20,
			UsesGravity = false,
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
			UsesGravity = false,
		});
		circle3.AddComponent(new CircleCollider()
		{
			Radius = 15
		});

	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Space).Down)
		{
			circle1.GetComponent<RigidBody>().AddForce(new(0f, -1f), 50);
			//	box1.Rotation += 5f;
			//	box2.GetComponent<RigidBody>().AddForce(new(0f, -1f), 100);
			//	box3.GetComponent<RigidBody>().AddForce(new(0f, -1f), 10);
		}

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

		CircleCollider crCol = circle1.GetComponent<CircleCollider>();
		if (Input.MousePosition.X < circle1.Position.X + crCol.Radius && Input.MousePosition.X > circle1.Position.X - crCol.Radius
			&& Input.MousePosition.Y < circle1.Position.Y + crCol.Radius && Input.MousePosition.Y > circle1.Position.Y - crCol.Radius)
		{
			if (Input.GetMouseButton(MouseButton.Left).Down)
			{
				charging = true;
			}
		}

		if (charging && Input.GetMouseButton(MouseButton.Left).Released)
		{
			charging = false;
			//	directionVector = new Vector2(mouseX - whiteBallPosition.X, mouseY - whiteBallPosition.Y);
			Vector2 directionVector = Vector2.Normalize(circle1.Position - Input.MousePosition);
			float power = 1f;
			float distance = Vector2.Distance(Input.MousePosition, circle1.Position);
			power *= distance / 10;
			circle1.GetComponent<RigidBody>().AddForce(directionVector, power);
		}
	}
	protected override void FixedUpdate()
	{

		PGen.Update(this.Children.Where(x => x.GetComponent<RigidBody>() != null).ToList());
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
