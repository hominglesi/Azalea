using Azalea.Design.Containers;
using Azalea.Design.Schemes;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Platform;
public sealed class Application
{
	public readonly AzaleaGame Game;
	public readonly Windowing.PlatformWindow Window;
	public readonly PlatformRenderer Renderer;
	public readonly PlatformScheduler Scheduler;
	public readonly InputProcessor Input;

	internal Application(AzaleaGame game)
	{
		Game = game;
		Game.App = this;

		Window = Windowing.PlatformWindow.Create("Azalea App", new(800, 600));
		Renderer = PlatformRenderer.AttachRenderer(Window);
		Scheduler = PlatformScheduler.AttachScheduler(Window);
		Input = new InputProcessor(Game);

		Window.OnClosed += () => OnClosed?.Invoke();

		var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();

		commandGroup.Enable(GL.BLEND);
		commandGroup.Enable(GL.CULL_FACE);
		commandGroup.Disable(GL.DEPTH_TEST);
		commandGroup.BlendFunction(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);

		Renderer.Thread.SubmitCommandGroup(commandGroup);

		var coordinator = Renderer.Coordinator;
		var quadBatch = coordinator.DefaultQuadBatch;

		Renderer.PrintErrors();

		Scheduler.InjectProtocol((win, rend) =>
		{
			var clientSize = win.ClientSize;

			if (clientSize == Vector2Int.Zero)
				return;

			Game.Size = clientSize;
			Game.UpdateSubTree();

			Input.Process(win.PendingInputEvents);
			
			var renderQueue = coordinator.BeginCommandQueue();
			renderQueue.PrepareRendering(clientSize);
			renderQueue.Clear(Palette.Flowers.Azalea);

			Game.Draw(null, Renderer.Coordinator);

			quadBatch.Draw(renderQueue);

			renderQueue.PrintErrors();

			renderQueue.SwapBuffers();

			Renderer.StageQueue(coordinator.EndCommandQueue());
		});
	}

	public Action? OnClosed;
	public void Close() => Window.Close();
}
