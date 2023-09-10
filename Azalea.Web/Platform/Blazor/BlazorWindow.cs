using Azalea.Platform;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using nkast.Wasm.Canvas.WebGL;
using System;
using System.IO;
using System.Net.Http;
using System.Timers;
using IndexPage = Azalea.Web.Pages.Index;

namespace Azalea.Web.Platform.Blazor;

internal class BlazorWindow : IWindow
{
	private readonly WebAssemblyHostBuilder _builder;
	private IndexPage _main => IndexPage.MAIN;

	public IWebGLRenderingContext GL => _gl ?? throw new Exception("GL has not yet been initialized");
	private IWebGLRenderingContext? _gl;

	public Action? OnInitialized;
	public Action? OnUpdate;
	public Action? OnRender;

	private Timer _clock;

	public BlazorWindow()
	{
		IndexPage.OnCanvasInitialized += onInitialized;

		_builder = WebAssemblyHostBuilder.CreateDefault();
		_builder.RootComponents.Add<App>("#app");
		_builder.RootComponents.Add<HeadOutlet>("head::after");

		_builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress) });

		_clock = new Timer(1000 / 60)
		{
			AutoReset = true,
			Enabled = true
		};
		_clock.Elapsed += onClockElapsed;
		_title = IWindow.DefaultTitle;
	}

	private void onInitialized()
	{
		_gl = _main.GL;
		OnInitialized?.Invoke();
	}

	private void onClockElapsed(object? sender, ElapsedEventArgs e)
	{
		OnUpdate?.Invoke();
		OnRender?.Invoke();
		_main.UpdateRender();

	}

	private string _title;
	public string Title
	{
		get => _title;
		set
		{
			if (_title == value) return;

			_main.SetTitle(value);
			_title = value;
		}
	}
	public Vector2Int ClientSize { get => new(1280, 720); set => throw new NotImplementedException(); }
	public WindowState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public bool Resizable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public bool CursorVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public void Run()
	{
		_builder.Build().RunAsync();
	}

	public void SetIconFromStream(Stream imageStream)
	{
		//throw new NotImplementedException();
	}

	public void Close()
	{
		//Blazor does not support closing the window
	}
}