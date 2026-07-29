using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.IO.Resources;
using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
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

		using (var commandGroup = Renderer.BeginGroup())
		{
			Renderer.Enable(GL.BLEND, commandGroup);
			Renderer.Enable(GL.CULL_FACE, commandGroup);
			Renderer.Disable(GL.DEPTH_TEST, commandGroup);
			Renderer.BlendFunction(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA, commandGroup);

			var azaleaImage = Assets.MainStore.GetImage("Textures/azalea-icon.png")!;

			var azaleaTexture = Renderer.GenerateTexture(commandGroup);

			Renderer.BindTexture(GL.TEXTURE_2D, azaleaTexture, commandGroup);
			Renderer.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, azaleaImage.Width, azaleaImage.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, azaleaImage.Data, commandGroup);
			Renderer.GenerateMipmap(GL.TEXTURE_2D, commandGroup);

			var whiteImage = new Image(1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue]);
			var whiteTexture = Renderer.GenerateTexture(commandGroup);
			Renderer.BindTexture(GL.TEXTURE_2D, whiteTexture, commandGroup);
			Renderer.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, whiteImage.Width, whiteImage.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, whiteImage.Data, commandGroup);
		}

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
					renderQueue.Return();
					renderQueue = RenderCommandQueue.Borrow();
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
