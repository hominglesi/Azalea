using Azalea;
using Azalea.Editor;
using Azalea.VisualTests;
using System;
using System.Threading;

internal class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		var host = new HostBuilder()
			.EnableEditor()
			.SetTitle("Azalea Visual Tests")
			.SetGameSize(new Vector2Int(1600, 900))
			.SetResizable(true)
			.SetVSync(false)
			.SetupPersistentDirectory("Azalea.VisualTests")
			.SetupReflectedDirectory("../../../../../../Azalea.VisualTests/")
			//.EnableTracing()
			.SetupConfig()
			.Create();

		/*
		host.CreateApplication(new VisualTests());
		Thread.Sleep(int.MaxValue);*/

		/*
		AzaleaGame.RENDERED_GAME = EditorWrapper.Wrap(new VisualTests());
		host.Run(AzaleaGame.RENDERED_GAME);*/

		host.CreateApplication(new VisualTests());
		Thread.Sleep(int.MaxValue);
	}
}

