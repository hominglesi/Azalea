using Azalea.Editor.Design.Gui;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.IO.Resources;
using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Platform.Scheduling;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Editor.DebugWindows;
internal class WindowingWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static readonly Dictionary<PlatformWindow, GUIGroup> _windowGroups = [];

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Windows", new(400, 400));

			PlatformWindow.OnWindowCreated += window =>
			{
				var group = _window.AddGroup(window.Title);
				_window.AddLabel("Window Type: " + window.PlatformType);

				if (window is WindowsWindow win)
				{
					// A bit hacky but it's for debugging purposes so it should be fine
					while (window.Initialized == false)
						Thread.Sleep(1);

					_window.AddLabel("Class Atom: " + win.ClassAtom);
					_window.AddLabel("Handle: " + win.Handle);
				}

				_window.AddButton("Close", () => window.Close());
				_window.FinishGroup();
				_windowGroups.Add(window, group);
			};
			PlatformWindow.OnWindowClosed += window =>
			{
				_window.RemoveElement(_windowGroups[window]);
				_windowGroups.Remove(window);
			};

			_window.AddLabel("Process Architecture: " + RuntimeInformation.ProcessArchitecture);
			_window.AddButton("Create new Window", () => PlatformWindow.Create());
			_window.AddButton("Create renderable new Window", () =>
			{
				var window = PlatformWindow.Create();
				var renderer = PlatformRenderer.AttachRenderer(window);
				var scheduler = PlatformScheduler.AttachScheduler(window);

				renderer.BeginCommandGroup();

				renderer.Enable(GL.BLEND);
				renderer.Enable(GL.CULL_FACE);
				renderer.Disable(GL.DEPTH_TEST);
				renderer.BlendFunction(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);

				var azaleaImage = Assets.MainStore.GetImage("Textures/azalea-icon.png")!;

				var azaleaTexture = renderer.GenerateTexture();
				renderer.BindTexture(GL.TEXTURE_2D, azaleaTexture);
				renderer.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, azaleaImage.Width, azaleaImage.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, azaleaImage.Data);
				renderer.GenerateMipmap(GL.TEXTURE_2D);

				var whiteImage = new Image(1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue]);
				var whiteTexture = renderer.GenerateTexture();
				renderer.BindTexture(GL.TEXTURE_2D, whiteTexture);
				renderer.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, whiteImage.Width, whiteImage.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, whiteImage.Data);

				var defaultQuadBatch = new DefaultQuadBatch(renderer.GetCoordinator());

				renderer.PrintErrors();

				renderer.SubmitCommandGroup();

				scheduler.InjectProtocol((win, rend) =>
				{
					var renderQueue = RenderCommandQueue.Borrow();
					renderQueue.Clear(Palette.Beige);

					defaultQuadBatch.Add(new Rectangle(new(100, 100), new(50)), Palette.Blue);
					defaultQuadBatch.Add(new Rectangle(new(400, 150), new(150)), Palette.Aqua);

					renderQueue.BindTexture(GL.TEXTURE_2D, whiteTexture);
					defaultQuadBatch.Draw(renderQueue);

					defaultQuadBatch.Add(new Rectangle(new(50, 300), new(200)), Palette.White);

					renderQueue.BindTexture(GL.TEXTURE_2D, azaleaTexture);
					defaultQuadBatch.Draw(renderQueue);

					renderQueue.PrintErrors();

					renderQueue.SwapBuffers();

					renderer.StageQueue(renderQueue);
				});
			});
		}

		_window.Show();
	}

	private static void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
