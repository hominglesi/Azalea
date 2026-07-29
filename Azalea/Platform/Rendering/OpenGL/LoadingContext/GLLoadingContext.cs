using Azalea.Threading;
using System.Threading;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;

/// <summary>
/// An OpenGL context used for loading assets which all other contexts
/// use as a shared context.
/// </summary>
internal partial class GLLoadingContext
{
	public readonly nint Handle;

	public GLLoadingContext()
	{
		Thread = new GLLoadingThread();
		Thread.Start();

		Thread.Initialized.WaitOne();
		Thread.Initialized.Dispose();

		Handle = Thread.Context!.Handle;
	}

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
			_window = Windowing.PlatformWindow.Create(initiallyVisible: false);
			Context = GLContext.Create(_window.BorrowDeviceContext());
			Context.MakeCurrent();

			Initialized.Set();
		}

		protected override void Update() { }
	}

	#endregion
}
