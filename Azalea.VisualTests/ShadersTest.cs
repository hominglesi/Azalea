using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System.Numerics;

namespace Azalea.VisualTests;
internal class ShadersTest : TestScene
{
	IShader _octagonsShader;
	Sprite _shadedSprite;
	Vector2 _lastShadedSpriteSize;

	public ShadersTest()
	{
		AzaleaGame.Main.Host.Renderer.ClearColor = Palette.Gray;

		var quadVertexShader = Assets.GetText("Shaders/quad_vertexShader.glsl")!;
		var octagramsFragmentShader = Assets.GetText("Shaders/octagrams_fragmentShader.glsl")!;
		_octagonsShader = AzaleaGame.Main.Host.Renderer.CreateShader(quadVertexShader, octagramsFragmentShader);

		Add(_shadedSprite = new Sprite()
		{
			Anchor = Anchor.Center,
			Origin = Anchor.Center,
			RelativeSizeAxes = Axes.Both,
			Shader = _octagonsShader
		});
		_octagonsShader.SetUniform("iResolution", _shadedSprite.Size);
		_lastShadedSpriteSize = _shadedSprite.Size;
	}

	protected override void Update()
	{
		if (_shadedSprite.Size != _lastShadedSpriteSize)
		{
			_octagonsShader.SetUniform("iResolution", _shadedSprite.Size);
			_lastShadedSpriteSize = _shadedSprite.Size;
		}
	}
}
