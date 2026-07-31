using Azalea.Threading;
using System.Threading;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;

/// <summary>
/// An OpenGL context used for loading assets which all other contexts
/// use as a shared context.
/// </summary>
internal partial class GLLoadingContext : ICommandHandler<LoadingCommand>
{
	public readonly nint Handle;

	public readonly Texture WhitePixel;

	public GLLoadingContext()
	{
		Thread = new GLLoadingThread();
		Thread.Start();

		Thread.Initialized.WaitOne();
		Thread.Initialized.Dispose();

		WhitePixel = new Texture();
		this.GenerateTexture(WhitePixel);
		this.TexImage2D(WhitePixel, 1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue], false);

		Handle = Thread.Context!.Handle;
	}

	public ICommandAwaitable? Enqueue(LoadingCommand command) => Thread.Enqueue(command);

	#region LoadingThread

	internal readonly GLLoadingThread Thread;

	internal partial class GLLoadingThread() : GameThread<LoadingCommand>(1)
	{
		private Windowing.PlatformWindow? _window;
		internal GLContext? Context;

		public override string DisplayName => "GL Loading Thread";
		internal EventWaitHandle Initialized = new(false, EventResetMode.ManualReset);

		protected override void Initialize()
		{
			_window = Windowing.PlatformWindow.Create(Vector2Int.Zero, initiallyVisible: false);
			Context = GLContext.Create(_window.BorrowDeviceContext());
			Context.MakeCurrent();

			Initialized.Set();
		}

		protected override void Update() { }
	}

	#endregion
}
