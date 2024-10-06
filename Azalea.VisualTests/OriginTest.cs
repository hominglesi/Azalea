using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.VisualTests;
internal class OriginTest : TestScene
{
	private Sprite _player;
	private Sprite _arm;
	private Vector2 _windowCenter => GameHost.Main.Window.ClientSize / 2;
	private Vector2 _leftHandPosition => _windowCenter - new Vector2(22, 5);
	private Vector2 _rightHandPosition => _windowCenter + new Vector2(22, -5);
	public OriginTest()
	{
		Add(_player = new Sprite()
		{
			Size = new(64, 96),
			Texture = Assets.GetTexture("Textures/baseSprite.png"),
			Position = _windowCenter,
			Origin = Graphics.Anchor.Center,
		});

		Add(_arm = new Sprite()
		{
			Size = new(128, 64),
			Origin = Graphics.Anchor.Custom,
			OriginPosition = new(120, 40),
			Position = _rightHandPosition,
			Texture = Assets.GetTexture("Textures/Bolter.png"),

		});
	}

	protected override void Update()
	{
		var angle = MathUtils.GetAngleTowards(_windowCenter, Input.MousePosition);
		var direction = MathUtils.GetDirectionFromAngle(angle);
		var rotation = MathUtils.RadiansToDegrees(angle) - 180;

		if (direction.X < 0)
		{
			_arm.Rotation = rotation;
			_arm.Position = _rightHandPosition;
			_arm.Scale = Vector2.One;
		}
		else
		{
			_arm.Rotation = rotation;
			_arm.Position = _leftHandPosition;
			_arm.Scale = new(1, -1);
		}
	}
}
