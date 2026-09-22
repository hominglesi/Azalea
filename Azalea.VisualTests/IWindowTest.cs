using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Platform.Windowing;
using System;

namespace Azalea.VisualTests;
public class IWindowTest : TestScene
{
	private bool _preventsClosure;

	public IWindowTest()
	{
		Window.Closing += onWindowClosing;

		AddRange(new GameObject[] {
			CreateFullscreenVerticalFlex(new GameObject[]
			{
				CreateActionButton(
					"Create application window",
					() => GameHost.Main.CreateApplication(new VisualTests())),
				CreateActionButton(
					"Set size to 700, 700",
					() => App.Window.SetSize(new(700, 700))),
				CreateActionButton(
					"Set client size to 700, 700",
					() => App.Window.SetClientSize(new(700, 700))),
				CreateActionButton("Restore window", () => App.Window.Restore()),
				CreateActionButton("Minimize window", () => App.Window.Minimize()),
				CreateActionButton("Maximize window", () => App.Window.Maximize()),
				CreateActionButton("Fullscreen window", () => App.Window.Fullscreen()),
				CreateActionButton(
					"Set Resizable to 'true'",
					() => App.Window.SetResizable(true)),
				CreateActionButton(
					"Set Resizable to 'false'",
					() => App.Window.SetResizable(false)),
				CreateActionButton(
					"Set Title to 'Azalea Game'",
					() => App.Window.SetTitle("Azalea Game")),
				CreateActionButton(
					"Set Title to 'Ide Gas'",
					() => App.Window.SetTitle(Window.Title = "Ide Gas")),
				CreateActionButton(
					"Set Title to ''",
					() => App.Window.SetTitle(Window.Title = "")),
				CreateActionButton(
					"Set this test to prevent Closing",
					() => _preventsClosure = true),
				CreateActionButton(
					"Set this test to not prevent Closing",
					() => _preventsClosure = false),
				CreateActionButton(
					"Set icon to Azalea flower",
					() => App.Window.SetIcon(Assets.MainStore.GetImage("Textures/azalea-icon.png"))),
				CreateActionButton(
					"Set icon to Missing texture",
					() => App.Window.SetIcon(Assets.MainStore.GetImage("Textures/missing-texture.png"))),
				CreateActionButton(
					"Set icon to null",
					() => App.Window.SetIcon(null)),
				CreateActionButton(
					"Turn vsync on (BROKEN)",
					() => Window.VSync = true),
				CreateActionButton(
					"Turn vsync off (BROKEN)",
					() => Window.VSync = false),
				CreateActionButton(
					"Show cursor",
					() => App.Window.SetCursorVisible(true)),
				CreateActionButton(
					"Hide cursor",
					() => App.Window.SetCursorVisible(false)),
				CreateActionButton(
					"Set position to 0",
					() => App.Window.SetPosition(Vector2Int.Zero)),
				CreateActionButton(
					"Set client position to 0",
					() => App.Window.SetPosition(Vector2Int.Zero)),
				CreateActionButton(
					"Move window by (25, 25)",
					() => App.Window.SetPosition(App.Window.Position + new Vector2Int(25, 25))),
				CreateActionButton(
					"Enlarge window by (25, 25)",
					() => App.Window.SetClientSize(App.Window.ClientSize + new Vector2Int(25, 25))),
				CreateActionButton("Center window", () => App.Window.Center()),
				CreateActionButton(
					"Request Attention in 1.5 seconds",
					() => {_attentionTimer = 1.5f; }),
				CreateActionButton(
					"Focus in 1.5 seconds",
					() => _focusTimer = 1.5f),
				CreateActionButton("Close window", () => App.Window.Close())
			}),

			CreateObservedContainer(new GameObject[]
			{
				CreateObservedValue("WindowState",
					() => Window.State,
					(value) => $"Window state changed to {value}"),
				CreateObservedValue("Title",
					() => Window.Title,
					(value) => $"Window title changed to {value}"),
				CreateObservedValue("Resizable",
					() => Window.Resizable,
					(value) => $"Window resizable changed to {value}"),
				CreateObservedValue("Prevents Closure",
					() => _preventsClosure,
					(value) => value ? $"Test now prevents closure attempts" : $"Test no longer prevents closure attempts"),
				CreateObservedValue("Position",
					() => Window.Position,
					(value) => $"Window moved to {value}"),
				CreateObservedValue("ClientPosition",
					() => Window.ClientPosition),
				CreateObservedValue("Size",
					() => Window.Size),
				CreateObservedValue("ClientSize",
					() => Window.ClientSize,
					(value) => $"Window resized to {value}"),
				CreateObservedValue("VSync",
					() => Window.VSync),
				CreateObservedValue("CanChangeVSync",
					() => Window.CanChangeVSync),
				CreateObservedValue("Cursor Visible",
					() => Window.CursorVisible)
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
				App.Window.Focus();
		}

		if (_attentionTimer > 0)
		{
			_attentionTimer -= Time.DeltaTime;
			if (_attentionTimer <= 0)
				App.Window.RequestAttention();
		}
	}

	private void onWindowClosing()
	{
		if (_preventsClosure)
		{
			Console.WriteLine("The Window closure attempt was prevented by this Test");
			Window.PreventClosure();
		}
	}
}
