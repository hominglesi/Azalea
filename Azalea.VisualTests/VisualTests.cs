//using Azalea.Audios;
using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{
	private List<string> _tests = new();

	private FlexContainer _testSelectScene = new();

	protected override void OnInitialize()
	{
		Assets.AddToMainStore(new NamespacedResourceStore(new AssemblyResourceStore(typeof(VisualTests).Assembly), "Resources"));
		var selectedTest = Config.GetValue("currentScene");
		if (selectedTest is null)
		{
			selectedTest = "TestingTestScene";
			Config.SetValue("currentScene", "TestingTestScene");
		}


		Host.Renderer.ClearColor = Palette.Flowers.Azalea;

		var scene = Activator.CreateInstance(Assembly.GetAssembly(typeof(VisualTests))!.FullName!, $"Azalea.VisualTests.{selectedTest}")!.Unwrap()
			as GameObject;

		if (scene is null)
		{
			scene = new TestingTestScene();
			Config.SetValue("currentScene", "TestingTestScene");
		}
		Child = scene!;

		_tests = ReflectionUtils.GetAllChildrenOfAsStrings(typeof(TestScene)).ToList();

		_testSelectScene.RelativeSizeAxes = Axes.Both;
		_testSelectScene.Direction = FlexDirection.VerticalReverse;
		_testSelectScene.Spacing = new(7);
		_testSelectScene.Wrapping = FlexWrapping.Wrap;
		foreach (var test in _tests)
		{
			_testSelectScene.Add(new BasicButton()
			{
				Text = test,
				Action = () =>
				{
					ChangeTest(test);
				}
			});
		}

		//Add(new DefaultUserInputTest());
		//Add(new TestingTestScene());
		//Add(new FlexTest());
		//Add(new TextContainerTest());
		//Add(new BilliardTest());
		//Add(new PhysicsTest());
		//Add(new TriggerTest());
		//Add(new SliderTests());
		//Add(new AutoSizeTest());
		//Add(new InputTest());
		//Add(new PanningTest());
		//Add(new IWindowTest());
		//Add(new AudioTest());
		//Add(new BreakoutTest());
		//Add(new BoundingBoxTreeTest());
		//Add(new CameraTest());
	}

	private void ChangeTest(string testName)
	{
		var test = Activator.CreateInstance(Assembly.GetAssembly(typeof(VisualTests))!.FullName!, $"Azalea.VisualTests.{testName}")!.Unwrap()
			as TestScene;
		Child = test!;
		Config.SetValue("currentScene", testName);
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Escape).Down)
		{
			if (Child != _testSelectScene)
				Child = _testSelectScene;
		}

		/*
		if (Input.GetKey(Keys.Escape).Down) Host.Window.Close();

		if (Input.GetKey(Keys.K).Down)
		{
			if (_instance is not null)
			{
				_instance.Stop();
				_instance = null;
			}
			else
			{
				_instance = Audio.Play(_sound);
			}
		}

		if (Input.GetKey(Keys.L).Down) Audio.Play(_sound2);*/
	}
}
