using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.IO.Resources;
using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Utils;
using System;

namespace Azalea.Platform;
public sealed class Application
{
	public readonly Windowing.PlatformWindow Window;
	public readonly PlatformRenderer Renderer;
	public readonly PlatformScheduler Scheduler;

	public Application()
	{
		Window = Windowing.PlatformWindow.Create();
		Renderer = PlatformRenderer.AttachRenderer(Window);
		Scheduler = PlatformScheduler.AttachScheduler(Window);

		Window.Closed.OnValueChanged += _ => OnClosed?.Invoke();

		var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();

		commandGroup.Enable(GL.BLEND);
		commandGroup.Enable(GL.CULL_FACE);
		commandGroup.Disable(GL.DEPTH_TEST);
		commandGroup.BlendFunction(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);

		var azaleaImage = Assets.MainStore.GetImage("Textures/azalea-icon.png")!;

		var azaleaTexture = commandGroup.GenerateTexture();
		commandGroup.TexImage2D(azaleaTexture, azaleaImage.Width, azaleaImage.Height, azaleaImage.Data, true);

		var whiteImage = new Image(1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue]);
		var whiteTexture = commandGroup.GenerateTexture();
		commandGroup.TexImage2D(whiteTexture, whiteImage.Width, whiteImage.Height, whiteImage.Data, false);

		commandGroup.BindTexture(GL.TEXTURE_2D, whiteTexture);

		Renderer.Thread.SubmitCommandGroup(commandGroup);

		var coordinator = Renderer.Coordinator;
		var quadBatch = coordinator.DefaultQuadBatch;

		Renderer.PrintErrors();

		Scheduler.InjectProtocol((win, rend) =>
		{
			var renderQueue = coordinator.BeginCommandQueue();
			renderQueue.PrepareRendering();
			renderQueue.Clear(Palette.Beige);

			if (AzaleaGame.RENDERED_GAME is not null)
			{
				try
				{
					AzaleaGame.RENDERED_GAME.Draw(null, Renderer.Coordinator);
				}
				catch (Exception)
				{
					ObjectPool<RenderCommandGroup>.Return(renderQueue);
					renderQueue = ObjectPool<RenderCommandGroup>.Borrow();
					renderQueue.Clear(Palette.Beige);
				}
			}

			quadBatch.Draw(renderQueue);

			renderQueue.PrintErrors();

			renderQueue.SwapBuffers();

			Renderer.StageQueue(coordinator.EndCommandQueue());
		});
	}

	public Action? OnClosed;
	public void Close() => Window.Close();
}
