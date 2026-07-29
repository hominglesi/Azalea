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

		commandGroup.BindTexture(GL.TEXTURE_2D, azaleaTexture);
		commandGroup.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, azaleaImage.Width, azaleaImage.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, azaleaImage.Data);
		commandGroup.GenerateMipmap(GL.TEXTURE_2D);

		var whiteImage = new Image(1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue]);
		var whiteTexture = commandGroup.GenerateTexture();
		commandGroup.BindTexture(GL.TEXTURE_2D, whiteTexture);
		commandGroup.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, whiteImage.Width, whiteImage.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, whiteImage.Data);

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
