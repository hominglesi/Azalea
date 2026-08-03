using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;
using Azalea.Platform.Windowing;
using Azalea.Threading;
using Azalea.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer
{
	internal static GLLoadingContext? LoadingContext;
	internal static ReadOnlyObservable<bool> LoadingContextCreated = new(false);
	private static object _initializationLock = new();
	[MemberNotNull(nameof(LoadingContext))]
	private static void assureGLInitialized()
	{
		lock (_initializationLock)
		{
			if (LoadingContext is not null)
				return;

			var dummyWindow = Windowing.PlatformWindow.Create("", Vector2Int.Zero, initiallyVisible: false);
			var dummyDeviceContext = dummyWindow.BorrowDeviceContext();

			var dummyThread = new InitializationThread(dummyDeviceContext);
			dummyThread.Start();

			dummyThread.Initialized.WaitOne();
			dummyThread.Initialized.Dispose();

			dummyWindow.Close();
			dummyThread.Stop();

			LoadingContext = new GLLoadingContext();
			LoadingContextCreated.Value = true;
		}
	}

	public static void TempAssureGLInitialized() => assureGLInitialized();

	class InitializationThread(IPlatformDeviceContext deviceContext) : GameThread(1)
	{
		private readonly IPlatformDeviceContext _deviceContext = deviceContext;

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
