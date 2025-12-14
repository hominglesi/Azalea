using Azalea.Amends;
using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{
	private const string __currentSceneKey = "currentScene";

	private List<string> _tests;
	private TestSelectScene _testSelectScene;

	public VisualTests()
	{
		Assets.AddToMainStore(new NamespacedResourceStore(new EmbeddedResourceStore(typeof(VisualTests).Assembly), "Resources"));
		Assets.AddFont("Fonts/TitanOne-Regular.fnt", "TitanOne-Regular");
		Assets.MainStore.AddMsdfFont("TitanOne-Regular", "Fonts/TitanOne-Regular.csv", "Fonts/TitanOne-Regular.bmp");
		_tests = ReflectionUtils.GetAllChildrenOf(typeof(TestScene)).Where(x => x.IsAbstract == false).Select(x => x.FullName).ToList()!;

		_testSelectScene = new TestSelectScene(_tests);

		Renderer.ClearColor = Palette.Flowers.Azalea;

		var selectedTest = Config.Get(__currentSceneKey);
		if (selectedTest is null || _tests.Contains(selectedTest) == false)
			goToSceneSelect();
		else
			_testSelectScene.ChangeTest(selectedTest);
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Escape).Down)
			goToSceneSelect();
	}

	private void goToSceneSelect()
	{
		var currentScene = SceneManager.CurrentScene;

		if (currentScene == null || currentScene != _testSelectScene)
		{
			SceneManager.ChangeScene(_testSelectScene);
			Renderer.ClearColor = new Color(40, 51, 60);
			Window.ClientSize = new(1600, 900);
			Window.Center();
		}
	}

	private class TestSelectScene : Scene
	{
		private FlexContainer _testContainer;

		public TestSelectScene(IEnumerable<string> tests)
		{
			Add(_testContainer = new FlexContainer()
			{
				RelativeSizeAxes = Axes.Both,
				Direction = FlexDirection.Vertical,
				Spacing = new(10),
				Wrapping = FlexWrapping.Wrap,
			});

			foreach (var test in tests)
			{
				var displayName = VisualTestUtils.GetTestDisplayName(test);

				_testContainer.Add(new TestButton()
				{
					Text = displayName,
					Action = () => { ChangeTest(test); }
				});
			}
		}

		public void ChangeTest(string testName)
		{
			var test = Activator.CreateInstance(Assembly.GetAssembly(typeof(VisualTests))!.FullName!, testName)!.Unwrap()
			as TestScene;

			SceneManager.ChangeScene(test!);
			Config.Set(__currentSceneKey, testName);
		}
	}

	private class TestButton : Button
	{
		private SpriteText _text;
		private Sprite _background;
		private Box _whiteOverlay;

		public TestButton()
		{
			Size = new(320, 65);

			BorderColor = new Color(59, 70, 81);
			BorderThickness = new(2);
			Margin = new(4);

			Add(_background = new()
			{
				RelativeSizeAxes = Axes.Both,
				Texture = Assets.GetTexture("Textures/TestSelect/test-button.png")
			});

			Add(_whiteOverlay = new()
			{
				RelativeSizeAxes = Axes.Both,
				Color = Palette.White,
				Alpha = 0
			});

			Add(_text = new()
			{
				Origin = Anchor.CenterLeft,
				Anchor = Anchor.CenterLeft,
				Margin = new(0, 0, 0, 10),
				Font = FontUsage.Default.With(size: 32, family: "TitanOne")
			});
		}

		protected override bool OnHover(HoverEvent e)
		{
			this.ResizeBy(new(8), 0.15f);
			this.ChangeBoundaryPropertyTo("Margin", Boundary.Zero, 0.15f);
			BorderObject!.RecolorTo(Palette.White, 0.15f);
			_whiteOverlay.ChangeAlphaTo(0.1f, 0.15f);
			return true;
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			RemoveAmends();
			BorderObject!.RemoveAmends();
			_whiteOverlay.RemoveAmends();

			this.ResizeTo(new(320, 65), 0.07f);
			this.ChangeBoundaryPropertyTo("Margin", new(4), 0.07f);
			BorderObject!.RecolorTo(new Color(59, 70, 81), 0.07f);
			_whiteOverlay.ChangeAlphaTo(0f, 0.15f);

		}

		public string Text
		{
			get => _text.Text;
			set => _text.Text = value;
		}
	}
}
