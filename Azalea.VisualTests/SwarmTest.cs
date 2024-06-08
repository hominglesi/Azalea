using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Physics;
using Azalea.Physics.Colliders;
using Azalea.Utils;

namespace Azalea.VisualTests;
internal class SwarmTest : TestScene
{
	public SwarmTest()
	{

	}

	protected override void FixedUpdate()
	{
		SpawnSeeker();
	}

	private void SpawnSeeker()
	{
		Add(new Seeker()
		{
			Position = Rng.Vector2(new(0, 1200))
		});
	}

	private class Seeker : Sprite
	{
		public Seeker()
		{
			Size = new(60, 100);
			Texture = Assets.GetTexture("Textures/baseSprite.png");

			AddComponent(new RigidBody());
			AddComponent(new RectCollider());
		}
	}
}
