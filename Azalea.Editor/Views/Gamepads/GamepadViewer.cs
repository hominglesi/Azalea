using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.Inputs;
using Azalea.IO.Resources;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Editor.Views.Gamepads;

internal class GamepadViewer : Composition
{
	private readonly List<GamepadDisplay> _gamepadDisplays = [];

	public GamepadViewer()
	{
		RelativeSizeAxes = Axes.Both;

		Add(new SpritePattern()
		{
			Texture = Assets.GetTexture("Textures/background-pattern.png"),
			RelativeSizeAxes = Axes.Both,
		});
	}

	protected override void Update()
	{
		int currentIndex = 0;
		IGamepad? currentGamepad;

		while ((currentGamepad = App.Input.GetGamepad(currentIndex)) != null)
		{
			if (_gamepadDisplays.Count <= currentIndex)
			{
				_gamepadDisplays.Add(new GamepadDisplay());
				Add(_gamepadDisplays[currentIndex]);
			}

			_gamepadDisplays[currentIndex].UpdateData(currentGamepad);

			currentIndex++;
		}

		while (currentIndex < _gamepadDisplays.Count)
		{
			var lastDisplay = _gamepadDisplays[^1];
			Remove(lastDisplay);
			_gamepadDisplays.Remove(lastDisplay);
		}
	}

	private class GamepadDisplay : Composition
	{
		private readonly FlexContainer _inner;
		private readonly GamepadLayoutDisplay _layoutDisplay;

		public GamepadDisplay()
		{
			RelativeSizeAxes = Axes.X;
			Height = 420;

			Add(new ScrollableContainer()
			{
				RelativeSizeAxes = Axes.Both,
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				NegativeSize = new(20),
				BackgroundColor = Palette.Gray,
				Child = _inner = new FlexContainer()
				{
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y,
					Direction = FlexDirection.Vertical
				}
			});

			_inner.Add(_layoutDisplay = new GamepadLayoutDisplay());
		}

		public void UpdateData(IGamepad gamepad)
		{
			_layoutDisplay.UpdateData(gamepad);
		}

		private class GamepadLayoutDisplay : Composition
		{
			private readonly Sprite _xButtonPressed;
			private readonly Sprite _yButtonPressed;
			private readonly Sprite _aButtonPressed;
			private readonly Sprite _bButtonPressed;

			private readonly Sprite _upDPadPressed;
			private readonly Sprite _downDPadPressed;
			private readonly Sprite _leftDPadPressed;
			private readonly Sprite _rightDPadPressed;

			private readonly Sprite _backButtonPressed;
			private readonly Sprite _startButtonPressed;

			private readonly Sprite _leftBumperPressed;
			private readonly Sprite _rightBumperPressed;

			private readonly Sprite _leftTriggerPressed;
			private readonly Sprite _rightTriggerPressed;

			private readonly Composition _leftThumb;
			private readonly Composition _rightThumb;
			private readonly Vector2 _leftThumbPosition;
			private readonly Vector2 _rightThumbPosition;
			private readonly float _thumbMaxDelta = 20f;

			private readonly Sprite _leftThumbPressed;
			private readonly Sprite _rightThumbPressed;

			public GamepadLayoutDisplay()
			{
				Size = new(550, 400);

				Add(_xButtonPressed = createButtonPressed(new(-1, 0)));
				Add(_yButtonPressed = createButtonPressed(new(0, -1)));
				Add(_aButtonPressed = createButtonPressed(new(0, 1)));
				Add(_bButtonPressed = createButtonPressed(new(1, 0)));

				Add(_upDPadPressed = createDPadPressed(new(0, -1), 180));
				Add(_downDPadPressed = createDPadPressed(new(0, 1)));
				Add(_leftDPadPressed = createDPadPressed(new(-1, 0), 90));
				Add(_rightDPadPressed = createDPadPressed(new(1, 0), 270));

				Add(_backButtonPressed = createButtonSmallPressed(-1));
				Add(_startButtonPressed = createButtonSmallPressed(1));

				Add(_leftBumperPressed = createBumperPressed(-1));
				Add(_rightBumperPressed = createBumperPressed(1));

				Add(_leftTriggerPressed = createTriggerPressed(-1));
				Add(_rightTriggerPressed = createTriggerPressed(1));

				Add(new Sprite()
				{
					Anchor = Anchor.Center,
					Origin = Anchor.Center,
					Texture = getTexture("gamepad-layout.png")
				});

				Add(_leftThumb = createThumb(-1));
				_leftThumbPressed = (Sprite)_leftThumb.Children[0];
				_leftThumbPosition = _leftThumb.Position;

				Add(_rightThumb = createThumb(1));
				_rightThumbPressed = (Sprite)_rightThumb.Children[0];
				_rightThumbPosition = _rightThumb.Position;
			}

			public void UpdateData(IGamepad gamepad)
			{
				updateButtonAlpha(_aButtonPressed, GamepadButton.A, gamepad);
				updateButtonAlpha(_bButtonPressed, GamepadButton.B, gamepad);
				updateButtonAlpha(_xButtonPressed, GamepadButton.X, gamepad);
				updateButtonAlpha(_yButtonPressed, GamepadButton.Y, gamepad);
				updateButtonAlpha(_startButtonPressed, GamepadButton.Start, gamepad);
				updateButtonAlpha(_backButtonPressed, GamepadButton.Back, gamepad);
				updateButtonAlpha(_leftBumperPressed, GamepadButton.LeftShoulder, gamepad);
				updateButtonAlpha(_rightBumperPressed, GamepadButton.RightShoulder, gamepad);
				updateButtonAlpha(_leftTriggerPressed, GamepadButton.LeftTrigger, gamepad);
				updateButtonAlpha(_rightTriggerPressed, GamepadButton.RightTrigger, gamepad);
				updateButtonAlpha(_leftThumbPressed, GamepadButton.LeftStick, gamepad);
				updateButtonAlpha(_rightThumbPressed, GamepadButton.RightStick, gamepad);

				var dpad = gamepad.GetDPad();
				updateDPadAlpha(_upDPadPressed, dpad.Up);
				updateDPadAlpha(_downDPadPressed, dpad.Down);
				updateDPadAlpha(_leftDPadPressed, dpad.Left);
				updateDPadAlpha(_rightDPadPressed, dpad.Right);

				_leftThumb.Position = _leftThumbPosition
					+ (gamepad.GetLeftStick().GetVectorCircular() * _thumbMaxDelta);
				_rightThumb.Position = _rightThumbPosition
					+ (gamepad.GetRightStick().GetVectorCircular() * _thumbMaxDelta);
			}

			private static void updateButtonAlpha(Sprite sprite,
				GamepadButton button, IGamepad gamepad)
				=> sprite.Alpha = gamepad.GetButton(button).Pressed ? 1 : 0;

			private static void updateDPadAlpha(Sprite sprite, ButtonState button)
				=> sprite.Alpha = button.Pressed ? 1 : 0;

			private static Sprite createButtonPressed(Vector2 direction)
				=> new()
				{
					Texture = getTexture("gamepad-button-pressed.png"),
					Origin = Anchor.Center,
					X = 392 + (direction.X * 31),
					Y = 165 + (direction.Y * 30)
				};

			private static Sprite createDPadPressed(Vector2 direction, float rotation = 0)
				=> new()
				{
					Texture = getTexture("gamepad-dpad-pressed.png"),
					Rotation = rotation,
					Origin = Anchor.Center,
					X = 159 + (direction.X * 26),
					Y = 167 + (direction.Y * 26)
				};

			private static Sprite createButtonSmallPressed(float horizontalDirection)
				=> new()
				{
					Texture = getTexture("gamepad-button-small-pressed.png"),
					Origin = Anchor.Center,
					X = 274 + (horizontalDirection * 22),
					Y = 142
				};

			private static Sprite createBumperPressed(float horizontalDirection)
				=> new()
				{
					Texture = getTexture("gamepad-bumper-pressed.png"),
					Origin = Anchor.Center,
					X = 274 + (horizontalDirection * 90),
					Y = 42
				};

			private static Sprite createTriggerPressed(float horizontalDirection)
				=> new()
				{
					Texture = getTexture("gamepad-trigger-pressed.png"),
					Origin = Anchor.Center,
					X = 274 + (horizontalDirection * 135),
					Y = 45
				};

			private static Composition createThumb(float horizontalDirection)
				=> new()
				{
					Size = new(62),
					Origin = Anchor.Center,
					X = 276 + (horizontalDirection * 62),
					Y = 245,
					Children = [
						new Sprite()
						{
							Texture = getTexture("gamepad-thumb-pressed.png")
						},
						new Sprite()
						{
							Texture = getTexture("gamepad-thumb.png")
						}
					]
				};
		}
		private class ButtonDisplay : Composition
		{
			private bool _active;
			public ButtonDisplay()
			{
				Size = new(40);
				BackgroundColor = Palette.Black;
				_active = false;
			}

			public void SetActive(bool active)
			{
				if (_active == active)
					return;

				if (active)
					BackgroundColor = Palette.White;
				else
					BackgroundColor = Palette.Black;

				_active = active;
			}
		}

		private class ValueDisplay : Composition
		{
			private readonly SpriteText _text;
			public ValueDisplay()
			{
				Size = new(40);
				BackgroundColor = Palette.Black;

				Add(_text = new SpriteText()
				{
					Origin = Anchor.Center,
					Anchor = Anchor.Center
				});
			}

			public void SetValue(float value)
			{
				_text.Text = value.ToString();
			}
		}
	}

	private static Texture getTexture(string name)
		=> Assets.GetTexture("Textures/Editing/GamepadViewer/" + name);
}
