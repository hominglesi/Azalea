using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using System;
using System.Reflection;

namespace Azalea.VisualTests;
public class IWindowTest : TestScene
{
	private IWindow _window;

	private bool _preventsClosure;

	public IWindowTest()
	{
		_window = AzaleaGame.Main.Host.Window;
		_window.Closing += onWindowClosing;

		AddRange(new GameObject[] {
			CreateFullscreenVerticalFlex(new GameObject[]
			{
				CreateActionButton(
					"Set size to 700, 700",
					() => _window.Size = new(700, 700)),
				CreateActionButton(
					"Set client size to 700, 700",
					() => _window.ClientSize = new(700, 700)),
				CreateActionButton(
					"Set WindowState to 'Normal'",
					() => _window.State = WindowState.Normal),
				CreateActionButton(
					"Set WindowState to 'Minimized'",
					() => _window.State = WindowState.Minimized),
				CreateActionButton(
					"Set WindowState to 'Maximized'",
					() => _window.State = WindowState.Maximized),
				CreateActionButton(
					"Set WindowState to 'Fullscreen'",
					() => _window.State = WindowState.Fullscreen),
				CreateActionButton(
					"Set Resizable to 'true'",
					() => _window.Resizable = true),
				CreateActionButton(
					"Set Resizable to 'false'",
					() => _window.Resizable = false),
				CreateActionButton(
					"Set Title to 'Azalea Game'",
					() => _window.Title = "Azalea Game"),
				CreateActionButton(
					"Set Title to 'Ide Gas'",
					() => _window.Title = "Ide Gas"),
				CreateActionButton(
					"Set Title to ''",
					() => _window.Title = ""),
				CreateActionButton(
					"Set this test to prevent Closing",
					() => _preventsClosure = true),
				CreateActionButton(
					"Set this test to not prevent Closing",
					() => _preventsClosure = false),
				CreateActionButton(
					"Set icon to Azalea flower",
					() => _window.SetIconFromStream(Assets.GetStream("Textures/azalea-icon.png")!)),
				CreateActionButton(
					"Set icon to Missing texture",
					() => _window.SetIconFromStream(Assets.GetStream("Textures/missing-texture.png")!)),
				CreateActionButton(
					"Set icon to null",
					() => _window.SetIconFromStream(null)),
				CreateActionButton(
					"Turn vsync on",
					() => _window.VSync = true),
				CreateActionButton(
					"Turn vsync off",
					() => _window.VSync = false),
				CreateActionButton(
					"Set position to 0",
					() => _window.Position = Vector2Int.Zero),
				CreateActionButton(
					"Set client position to 0",
					() => _window.ClientPosition = Vector2Int.Zero),
				CreateActionButton(
					"Move window by (25, 25)",
					() => _window.Position += new Vector2Int(25, 25)),
				CreateActionButton(
					"Enlarge window by (25, 25)",
					() => _window.ClientSize += new Vector2Int(25, 25)),
				CreateActionButton(
					"Center window",
					() => _window.Center()),
				CreateActionButton(
					"Request Attention in 1.5 seconds",
					() => {_attentionTimer = 1.5f; }),
				CreateActionButton(
					"Focus in 1.5 seconds",
					() => _focusTimer = 1.5f),
				CreateActionButton(
					"Close window",
					() => _window.Close())
			}),

			CreateObservedContainer(new GameObject[]
			{
				CreateObservedValue("WindowState",
						() => _window.State,
						(value) => $"Window state changed to {value}"),
					CreateObservedValue("Title",
						() => _window.Title,
						(value) => $"Window title changed to {value}"),
					CreateObservedValue("Resizable",
						() => _window.Resizable,
						(value) => $"Window resizable changed to {value}"),
					CreateObservedValue("Prevents Closure",
						() => _preventsClosure,
						(value) => value ? $"Test now prevents closure attempts" : $"Test no longer prevents closure attempts"),
					CreateObservedValue("Position",
						() => _window.Position,
						(value) => $"Window moved to {value}"),
					CreateObservedValue("ClientPosition",
						() => _window.ClientPosition),
					CreateObservedValue("Size",
						() => _window.Size),
					CreateObservedValue("ClientSize",
						() => _window.ClientSize,
						(value) => $"Window resized to {value}"),
					/*
					CreateObservedValue("_fullscreen",
						() => getField<bool>(_window, "_fullscreen")),
					CreateObservedValue("_maximized",
						() => getField<bool>(_window, "_maximized")),
					CreateObservedValue("_minimized",
						() => getField<bool>(_window, "_minimized")),
					CreateObservedValue("_lastPosition",
						() => getField<Vector2Int>(_window, "_lastPosition")),
					CreateObservedValue("_lastSize",
						() => getField<Vector2Int>(_window, "_lastSize")),
					CreateObservedValue("_preMinimizedMaximized",
						() => getField<bool>(_window, "_preMinimizedMaximized")),
					CreateObservedValue("_preMinimizedFullscreen",
						() => getField<bool>(_window, "_preMinimizedFullscreen")),*/
			})
		});
	}

	private float _attentionTimer = -1f;
	private float _focusTimer = -1f;

	protected override void Update()
	{
		if (_focusTimer > 0)
		{
			_focusTimer -= Time.DeltaTime;
			if (_focusTimer <= 0)
			{
				_window.Focus();
			}
		}

		if (_attentionTimer > 0)
		{
			_attentionTimer -= Time.DeltaTime;
			if (_attentionTimer <= 0)
			{
				_window.RequestAttention();
			}
		}
	}

	private T getField<T>(object? obj, string name)
	{
		var type = typeof(AzaleaGame).Assembly.GetType("Azalea.Platform.Glfw.GLFWWindow")!;
		return (T)type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(obj)!;
	}

	private void onWindowClosing()
	{
		if (_preventsClosure)
		{
			Console.WriteLine("The Window closure attempt was prevented by this Test");
			_window.PreventClosure();
		}
	}
}
