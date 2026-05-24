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

			// TODO: Remove declaration
			var dummyWindow = Windowing.PlatformWindow.Create(visible: false);
			var dummyDeviceContext = dummyWindow.BorrowDeviceContext();

			var dummyThread = new InitializationThread(dummyDeviceContext);
			dummyThread.Start();

			while (dummyThread.Initialized == false)
				Thread.Sleep(1);

			dummyWindow.Close();
			dummyThread.Stop();

			_initialzed = true;
		}
	}

	class InitializationThread(PlatformDeviceContext deviceContext) : GameThread(1)
	{
		public override string DisplayName => "OpenGL Initialization Thread";

		public bool Initialized { get; private set; } = false;

		private readonly PlatformDeviceContext _deviceContext = deviceContext;

		protected override void Initialize()
		{
			var context = GLContext.CreateSimple(_deviceContext);
			context.MakeCurrent();

			context.LoadDynamicFunctions();

			Initialized = true;
		}

		protected override void Update() { }
	}
}
