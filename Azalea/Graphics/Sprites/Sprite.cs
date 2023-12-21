using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class Sprite : GameObject
{
	private Texture _texture = Assets.MissingTexture;
	public virtual Texture Texture
	{
		get => _texture;
		set
		{
			if (_texture == value) return;
			_texture = value;
			_time = 0;

			if (Size == Vector2.Zero)
				Size = new Vector2(_texture?.Width ?? 0, _texture?.Height ?? 0);
		}
	}

	private float _time = 0;
	internal float Time => _time;

	protected override void Update()
	{
		base.Update();

		_time += Platform.Time.DeltaTime;
	}

	protected override DrawNode CreateDrawNode() => new SpriteDrawNode(this);
}
