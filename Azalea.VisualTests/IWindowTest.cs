using Azalea.Graphics;
using Azalea.IO.Resources;
using Azalea.Platform;
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
					"Set size to 700, 700",
					() => Window.Size = new(700, 700)),
				CreateActionButton(
					"Set client size to 700, 700",
					() => Window.ClientSize = new(700, 700)),
				CreateActionButton(
					"Set WindowState to 'Normal'",
					() => Window.State = WindowState.Normal),
				CreateActionButton(
					"Set WindowState to 'Minimized'",
					() => Window.State = WindowState.Minimized),
				CreateActionButton(
					"Set WindowState to 'Maximized'",
					() => Window.State = WindowState.Maximized),
				CreateActionButton(
					"Set WindowState to 'Fullscreen'",
					() => Window.State = WindowState.Fullscreen),
				CreateActionButton(
					"Set Resizable to 'true'",
					() => Window.Resizable = true),
				CreateActionButton(
					"Set Resizable to 'false'",
					() => Window.Resizable = false),
				CreateActionButton(
					"Set Title to 'Azalea Game'",
					() => Window.Title = "Azalea Game"),
				CreateActionButton(
					"Set Title to 'Ide Gas'",
					() => Window.Title = "Ide Gas"),
				CreateActionButton(
					"Set Title to ''",
					() => Window.Title = ""),
				CreateActionButton(
					"Set this test to prevent Closing",
					() => _preventsClosure = true),
				CreateActionButton(
					"Set this test to not prevent Closing",
					() => _preventsClosure = false),
				CreateActionButton(
					"Set icon to Azalea flower",
					() => Window.SetIconFromStream(Assets.GetStream("Textures/azalea-icon.png")!)),
				CreateActionButton(
					"Set icon to Missing texture",
					() => Window.SetIconFromStream(Assets.GetStream("Textures/missing-texture.png")!)),
				CreateActionButton(
					"Set icon to null",
					() => Window.SetIconFromStream(null)),
				CreateActionButton(
					"Turn vsync on",
					() => Window.VSync = true),
				CreateActionButton(
					"Turn vsync off",
					() => Window.VSync = false),
				CreateActionButton(
					"Show cursor",
					() => Window.CursorVisible = true),
				CreateActionButton(
					"Hide cursor",
					() => Window.CursorVisible = false),
				CreateActionButton(
					"Set position to 0",
					() => Window.Position = Vector2Int.Zero),
				CreateActionButton(
					"Set client position to 0",
					() => Window.ClientPosition = Vector2Int.Zero),
				CreateActionButton(
					"Move window by (25, 25)",
					() => Window.Position += new Vector2Int(25, 25)),
				CreateActionButton(
					"Enlarge window by (25, 25)",
					() => Window.ClientSize += new Vector2Int(25, 25)),
				CreateActionButton(
					"Center window",
					() => Window.Center()),
				CreateActionButton(
					"Request Attention in 1.5 seconds",
					() => {_attentionTimer = 1.5f; }),
				CreateActionButton(
					"Focus in 1.5 seconds",
					() => _focusTimer = 1.5f),
				CreateActionButton(
					"Close window",
					() => Window.Close())
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
			{
				Window.Focus();
			}
		}

		if (_attentionTimer > 0)
		{
			_attentionTimer -= Time.DeltaTime;
			if (_attentionTimer <= 0)
			{
				Window.RequestAttention();
			}
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
