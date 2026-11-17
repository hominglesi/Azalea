using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Platform.Windowing;
using Azalea.Scenes;
using Azalea.Utils;
using System;

namespace Azalea.Platform;

public class Application
{
	private PlatformWindow? _window;
	private PlatformRenderer? _renderer;
	private PlatformScheduler? _scheduler;
	private InputProcessor? _input;
	public IScene? Scene
	{
		get;
		set
		{
			if (field == value) return;

			field?.SetApplication(null);
			_input?.Scene = null;

			field = value;
			field?.SetApplication(this);
			_input?.Scene = value;
		}
	}

	private float _accumulator = 0;
	private const float _fixedUpdateInterval = 1f / 60;

	internal void Start(PlatformWindow window, PlatformRenderer renderer,
		PlatformScheduler scheduler, InputProcessor input)
	{
		_window = window;
		_renderer = renderer;
		_scheduler = scheduler;
		_input = input;

		if (Scene is not null)
			_input.Scene = Scene;

		var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();

		commandGroup.Enable(GL.BLEND);
		commandGroup.Disable(GL.CULL_FACE);
		commandGroup.Disable(GL.DEPTH_TEST);
		commandGroup.BlendFunction(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);

		Renderer.Thread.SubmitCommandGroup(commandGroup);

		Renderer.PrintErrors();

		Scheduler.InjectProtocol(processUpdate);
	}

	private void processUpdate(PlatformWindow window, PlatformRenderer renderer)
	{
		if (Scene is null) return;

		var clientSize = window.ClientSize;

		if (clientSize == Vector2Int.Zero)
			return;

		Scene.UpdateSize(clientSize);

		_accumulator += Scheduler.DeltaTime;
		while (_accumulator > _fixedUpdateInterval)
		{
			Scene.RunFixedUpdate();
			_accumulator -= _fixedUpdateInterval;
		}

		Scene.RunUpdate();

		Input.Process(window.PendingInputEvents);

		var renderQueue = renderer.Coordinator.BeginCommandQueue();
		renderQueue.PrepareRendering(clientSize);

		Scene.RunDraw(Renderer.Coordinator);

		renderer.Coordinator.DefaultQuadBatch.Draw(renderQueue);

		renderQueue.PrintErrors();

		renderQueue.SwapBuffers();

		Renderer.StageQueue(renderer.Coordinator.EndCommandQueue());
	}

	public PlatformWindow Window => _window ?? throw new Exception("Application has not been started yet!");
	public PlatformRenderer Renderer => _renderer ?? throw new Exception("Application has not been started yet!");
	public PlatformScheduler Scheduler => _scheduler ?? throw new Exception("Application has not been started yet!");
	public InputProcessor Input => _input ?? throw new Exception("Application has not been started yet!");
}
