using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Simulations;
using Azalea.Simulations.Colliders;
using System.Numerics;

namespace Azalea.VisualTests;
public class TriggerTest : TestScene
{
	public Box box1;

	Sprite circle1;

	Line line;
	bool charging = false;
	public TriggerTest()
	{
		PhysicsOld.DebugMode = false;
		PhysicsOld.UsesGravity = false;
		Add(line = new Line()
		{
			StartPoint = new(500, 40),
			EndPoint = new(200, 400),
			Alpha = 0f
		});

		Add(circle1 = new Sprite()
		{
			Position = new(1000, 100),
			Size = new(140, 140),
			Color = Palette.Blue,
			Origin = Graphics.Anchor.Center,
			Texture = Assets.GetTexture("Textures/Circle.png"),
			Alpha = 150f
		});
		circle1.AddComponent(new RigidBodyOld()
		{
			Mass = 10000,
			IsDynamic = false,
		});
		CircleColliderOld crcol;
		circle1.AddComponent(crcol = new CircleColliderOld()
		{
			Radius = 70,
			IsTrigger = true,
		});
		crcol.OnCollisionEnter += (other) =>
		{
			ChangeColorTo(Palette.Red, other.Parent);
		};
		crcol.OnCollisionExit += (other) =>
		{
			ChangeColorTo(Palette.Yellow, other.Parent);
		};

		Add(box1 = new Box()
		{
			Position = new(690, 356),
			Size = new(50, 50),
			Color = Palette.Yellow,
			Origin = Graphics.Anchor.Center,
		});
		box1.AddComponent(new RigidBodyOld()
		{
			Mass = 10,
			//	UsesGravity = false,
			//AngularAcceleration = 0.0001f
		});
		box1.AddComponent(new RectColliderOld());


	}

	public void ChangeColorTo(Azalea.Graphics.Colors.Color color, GameObject obj)
	{
		obj.Color = color;
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Space).Down)
		{
			PhysicsOld.UsesGravity = true;
		}

		if (Input.GetKey(Keys.W).Pressed)
		{
			box1.Position += new Vector2(0, -2);

		}
		if (Input.GetKey(Keys.A).Pressed)
		{
			box1.Position += new Vector2(-2, 0);
		}
		if (Input.GetKey(Keys.S).Pressed)
		{
			box1.Position += new Vector2(0, 2);
		}
		if (Input.GetKey(Keys.D).Pressed)
		{
			box1.Position += new Vector2(2, 0);
		}

		if (Input.GetKey(Keys.E).Pressed)
		{
			box1.Rotation += 3f;
		}
		if (Input.GetKey(Keys.Q).Pressed)
		{
			box1.Rotation -= 3f;
		}
		/*
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
		}*/
	}
}
