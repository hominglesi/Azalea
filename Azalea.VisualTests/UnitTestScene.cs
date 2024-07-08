using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests;
public abstract class UnitTestScene : TestScene
{
	private List<Action> _stepActions = new();
	private StepSidebar _sidebar;

	public UnitTestScene()
	{
		Add(_sidebar = new StepSidebar()
		{
			RelativeSizeAxes = Axes.Y,
			Size = new(300, 1),
			BackgroundColor = new Color(0, 48, 73)
		});

		_sidebar.PlayButton.ClickAction = (_) =>
		{
			foreach (var action in _stepActions)
			{
				action.Invoke();
			}
		};
	}

	protected void AddStep(string name, Action action)
	{
		var step = new StepButton(name, action);
		_stepActions.Add(action);
		_sidebar.StepContainer.Add(step);
	}

	protected delegate bool TestAction();

	protected void AddTestStep(string name, TestAction action)
	{
		var step = new StepTestButton(name, action);
		_stepActions.Add(step.RunTest);
		_sidebar.StepContainer.Add(step);
	}

	private class StepButton : BasicButton
	{
		public StepButton(string text, Action action)
		{
			RelativeSizeAxes = Axes.X;
			ClickAction = (_) => action.Invoke();
			Size = new(1, 65);
			Text = text;
			BackgroundColor = new Color(252, 191, 73);
			TextColor = new Color(0, 48, 73);
			HoveredColor = new Color(247, 127, 0);
		}
	}

	private class StepTestButton : ContentContainer
	{
		private static float __buttonHeight = 65;

		private Sprite _resultSprite;
		private TestAction _action;

		public StepTestButton(string text, TestAction action)
		{
			_action = action;
			RelativeSizeAxes = Axes.X;
			Size = new(1, 65);

			AddInternal(_resultSprite = new Sprite()
			{
				Texture = Assets.GetTexture("Textures/Icons/checkbox-empty.png"),
				Size = new(__buttonHeight - 16),
				Position = new(-8, 0),
				Origin = Anchor.CenterRight,
				Anchor = Anchor.CenterRight
			});

			Add(new StepButton(text, RunTest));
		}

		public void RunTest()
		{
			var result = _action.Invoke();

			_resultSprite.Texture = Assets.GetTexture($"Textures/Icons/checkbox-{(result ? "passed" : "failed")}.png");
			_resultSprite.Color = result ? Palette.Green : new Color(214, 40, 40);
		}

		protected override void UpdateContentLayout()
		{
			ContentComposition.Width = DrawWidth - __buttonHeight;
		}
	}

	private class StepSidebar : ContentContainer
	{
		private static float __headerHeight = 70;

		public HeaderButton PlayButton;
		public HeaderButton StepButton;

		public FlexContainer HeaderContainer;
		public FlexContainer StepContainer;

		public StepSidebar()
		{
			ContentComposition.Y = __headerHeight;
			AddInternal(HeaderContainer = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				Size = new(1, __headerHeight),
				Justification = FlexJustification.Center,
				Alignment = FlexAlignment.Center,
				Wrapping = FlexWrapping.Wrap,
				Children = new[]
				{
					PlayButton = createButton("play"),
					StepButton = createButton("step")
				}
			});

			Add(new ScrollableContainer()
			{
				RelativeSizeAxes = Axes.Both,
				Child = StepContainer = new FlexContainer()
				{
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y,
					Direction = FlexDirection.Vertical
				}
			});
		}

		private HeaderButton createButton(string iconName)
			=> new(iconName)
			{
				Margin = new(15, 5, 15, 5),
				Size = new(__headerHeight - 30)
			};

		public class HeaderButton : Button
		{
			private Sprite _backgroundSprite;
			private Sprite _iconSprite;

			public HeaderButton(string iconName)
			{
				Add(_backgroundSprite = new Sprite()
				{
					RelativeSizeAxes = Axes.Both,
					Texture = Assets.GetTexture("Textures/Icons/background.png"),
					Color = new Color(234, 226, 183)
				});

				Add(_iconSprite = new Sprite()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.65f),
					Origin = Anchor.Center,
					Anchor = Anchor.Center,
					Texture = Assets.GetTexture($"Textures/Icons/{iconName}.png"),
					Color = new Color(214, 40, 40)
				});
			}

			protected override bool OnHover(HoverEvent e)
			{
				_backgroundSprite.Color = new Color(214, 40, 40);
				_iconSprite.Color = new Color(234, 226, 183);

				return true;
			}

			protected override void OnHoverLost(HoverLostEvent e)
			{
				_backgroundSprite.Color = new Color(234, 226, 183);
				_iconSprite.Color = new Color(214, 40, 40);
			}
		}

		protected override void UpdateContentLayout()
		{
			ContentComposition.Height = DrawHeight - __headerHeight;
			ContentComposition.Width = DrawWidth;
		}
	}
}
