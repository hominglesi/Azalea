using Azalea.Design.Containers;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Scenes;
using Azalea.Scenes.Drawables2D;
using Azalea.Scenes.Physics;
using Azalea.Utils;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Scenes.Physics;

public class InteractiveScenePhysicsTests : UnitTestSuite
{
	public class RandomCreation : UnitTest
	{
		private CustomScene2DContainer? _container;
		private RigidBody2D? _moveable;

		public override void Setup(UnitTestContainer scene)
		{
			scene.Add(_container = new CustomScene2DContainer());

			for (int i = 0; i < 15; i++)
			{
				var rigidBody = new RigidBody2D();

				if (Rng.Int(0, 2) == 0)
				{
					rigidBody.Width = Rng.Float(50, 150);
					rigidBody.Height = rigidBody.Width;
					rigidBody.Add(new CircleCollider2D() { Size = rigidBody.Size });
					rigidBody.Add(new Sprite()
					{
						Size = rigidBody.Size,
						Texture = Assets.GetTexture("Textures/resize-node.png")
					});
				}
				else
				{
					rigidBody.Size = Rng.Vector2(new(50, 200));
					rigidBody.Add(new RectangleCollider2D() { Size = rigidBody.Size });
					rigidBody.Add(new Sprite()
					{
						Size = rigidBody.Size
					});

					_container.Rotating.Add(rigidBody);
				}

				rigidBody.Position = Rng.Vector2(new Vector2(500, 1400), new Vector2(100, 750));
				_container.Scene.Add(rigidBody);
			}

			_moveable = new RigidBody2D
			{
				Position = new(1000, 450),
				Size = new(100),
				Children = [

					new CircleCollider2D() { Size = new(100, 100) },
					new Sprite()
					{
						Size = new(100, 100),
						Texture = Assets.GetTexture("Textures/resize-node.png")
					}
				]
			};

			_container.Scene.Add(_moveable);
			_container.Moveable = _moveable;
		}

		public override void TearDown(UnitTestContainer scene) => scene.Remove(_container!);

		class CustomScene2DContainer : Scene2DContainer
		{
			public Leaf? Moveable;
			public List<Leaf> Rotating = [];

			protected override void Update()
			{
				Moveable?.Position += App.Scheduler.DeltaTime * 250 * App.Input.State.GetDirectionalMovement();

				if (App.Input.State.KeyPressed(Keys.Space))
				{
					foreach (var rotating in Rotating)
						rotating.Rotation += App.Scheduler.DeltaTime * 180;
				}

				base.Update();
			}
		}
	}
}
