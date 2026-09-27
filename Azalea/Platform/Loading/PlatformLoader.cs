// SHIPPABLE
using Azalea.Lists;
using Azalea.Platform.Rendering;
using Azalea.Threading;
using System.Diagnostics;
using System.Threading;

namespace Azalea.Platform.Loading;

public abstract partial class PlatformLoader : ICommandHandler<LoadingCommand>
{
	internal PlatformLoader()
	{
		Thread = new InitializationThread(this);

		Thread.Start();
		Thread.InitializedEvent.WaitOne();

		Debug.Assert(WhitePixel is not null);
	}

	protected abstract void InitializationLogic();
	protected abstract void HandleCommandLogic(LoadingCommand command);

	public Texture WhitePixel { get; protected set; }

	internal ObservableList<Texture> LoadedTextures { get; } = [];
	internal ObservableList<Program> LoadedPrograms { get; } = [];

	public ICommandAwaitable? Enqueue(LoadingCommand command) => Thread.Enqueue(command);
	internal readonly InitializationThread Thread;

	internal class InitializationThread(PlatformLoader loader) : GameThread<LoadingCommand>(1)
	{
		private readonly PlatformLoader _loader = loader;

		public override string DisplayName => "Loading Thread";

		public ManualResetEvent InitializedEvent = new(false);
		protected override void Initialize()
		{
			_loader.InitializationLogic();
			InitializedEvent.Set();
		}

		protected override void Update() { }

		protected override void HandleCommand(LoadingCommand command)
			=> _loader.HandleCommandLogic(command);
	}
}
