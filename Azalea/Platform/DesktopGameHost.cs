using Azalea.Graphics;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Platform.Windows;
using Azalea.Sounds.FFmpeg;
using Azalea.Threading;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Azalea.Platform;

internal class DesktopGameHost : GameHost
{
	private readonly Vector2Int _defaultWindowSize = new(1280, 720);

	internal DesktopGameHost(HostPreferences prefs)
		: base(prefs)
	{
		if (prefs.PersistentDirectory is not null)
			Assets.SetupPersistentStore(prefs.PersistentDirectory);

		if (prefs.ReflectedDirectory is not null)
			Assets.SetupReflectedStore(prefs.ReflectedDirectory);

		if (prefs.ConfigName is not null)
			ConfigProvider = new FileConfigProvider(prefs.ConfigName);
	}

	protected override void Prepare()
	{
		NativeLibrary.SetDllImportResolver(typeof(AzaleaGame).Assembly,
			(libraryName, assembly, searchPath) =>
			{
				var path = libraryName switch
				{
					"avcodec" => createPath("avcodec-62.dll"),
					"avdevice" => createPath("avdevice-62.dll"),
					"avfilter" => createPath("avfilter-11.dll"),
					"avformat" => createPath("avformat-62.dll"),
					"avutil" => createPath("avutil-60.dll"),
					"soft_oal" => createPath("soft_oal.dll"),
					"swresample" => createPath("swresample-6.dll"),
					"swscale" => createPath("swscale-9.dll"),
					_ => null
				};

				if (path is null)
					return nint.Zero;

				return NativeLibrary.Load(path);
			});

		if (NativeLibrary.TryLoad(createPath("soft_oal"), out var _) == false)
		{
			throw new Exception("Native binaries could not be loaded!\n" +
				"If you are a developer make sure to specify a RuntimeIdentifier in the project. " +
				"Valid runtimes are: 'win-x64'.\n" +
				"If you are a user and have moved the executable file " +
				"make sure to move all the other files with it.");
		}

		Scheduler.Run(() =>
		{
			NativeLibrary.Load(createPath("avcodec-62.dll"));
			NativeLibrary.Load(createPath("avdevice-62.dll"));
			NativeLibrary.Load(createPath("avfilter-11.dll"));
			NativeLibrary.Load(createPath("avformat-62.dll"));
			NativeLibrary.Load(createPath("avutil-60.dll"));
			NativeLibrary.Load(createPath("swresample-6.dll"));
			NativeLibrary.Load(createPath("swscale-9.dll"));

			FFmpegStreamReader.Preload();
			return Task.CompletedTask;
		});

		static string createPath(string file)
					=> Path.Combine(AppContext.BaseDirectory, file);
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);

		ConfigProvider?.Save();
	}

	protected override void RunGameLoop()
	{
		ProcessGameLoop();
	}

	internal override IClipboard CreateClipboard()
		=> new WindowsClipboard();

	public override ITrayIcon CreateTrayIcon(string iconName, Image icon)
	{
		ArgumentNullException.ThrowIfNull(icon, nameof(icon));

		return null;
	}
}
