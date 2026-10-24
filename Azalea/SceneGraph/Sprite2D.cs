using Azalea.Graphics.Textures;
using Azalea.Platform.Rendering;

namespace Azalea.SceneGraph;

public class Sprite2D : Entity, IHasSpriteComponent
{
	internal SpriteComponent _spriteComponent;
	SpriteComponent IHasSpriteComponent.SpriteComponent => _spriteComponent;
}

internal struct SpriteComponent
{
	public ITexture? Texture;
	public IShader? Shader;
}

internal interface IHasSpriteComponent
{
	public SpriteComponent SpriteComponent { get; }
}
