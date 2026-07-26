using Azalea.Native.OpenGL;
using Azalea.Platform.Windowing;
using Azalea.Threading;
using System.Threading;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer
{
	private static object _initializationLock = new();
	private static bool _initialzed = false;
	private static void assureGLInitialized()
	{
		lock (_initializationLock)
		{
			if (_initialzed == true)
				return;

			var dummyWindow = Windowing.PlatformWindow.Create(initiallyVisible: false);
			var dummyDeviceContext = dummyWindow.BorrowDeviceContext();

			var dummyThread = new InitializationThread(dummyDeviceContext);
			dummyThread.Start();

			dummyThread.Initialized.WaitOne();
			dummyThread.Initialized.Dispose();

			dummyWindow.Close();
			dummyThread.Stop();

			_initialzed = true;
		}
	}

	class InitializationThread(PlatformDeviceContext deviceContext) : GameThread(1)
	{
		private readonly PlatformDeviceContext _deviceContext = deviceContext;

		public override string DisplayName => "OpenGL Initialization Thread";

		internal EventWaitHandle Initialized = new(false, EventResetMode.ManualReset);

		protected override void Initialize()
		{
			var context = GLContext.CreateSimple(_deviceContext);
			context.MakeCurrent();

			GL.LoadDynamicFunctions(context.GetProcAddress);

			Initialized.Set();
		}

		protected override void Update() { }
	}
}
