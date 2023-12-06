using Azalea.Graphics.Textures;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class Sprite : GameObject
{
	private Texture? texture;

	public virtual Texture? Texture
	{
		get => texture;
		set
		{
			if (texture == value) return;

			texture = value;
			_time = 0;

			if (Size == Vector2.Zero)
				Size = new Vector2(texture?.Width ?? 0, texture?.Height ?? 0);
		}
	}

	private float _time = 0;

	internal float Time => _time;

	protected override void Update()
	{
		base.Update();

		_time += Azalea.Platform.Time.DeltaTime;
	}

	protected override DrawNode CreateDrawNode() => new SpriteDrawNode(this);
}
