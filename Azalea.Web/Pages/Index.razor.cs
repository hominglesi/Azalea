using Microsoft.JSInterop;
using nkast.Wasm.Canvas;
using nkast.Wasm.Canvas.WebGL;
using nkast.Wasm.Dom;
using System;
using System.Threading.Tasks;

namespace Azalea.Web.Pages;

public partial class Index
{
	public static Action? OnCanvasInitialized;
	public static Action? OnUpdate;
	public static Action? OnRender;

	public static Index MAIN => _main ?? throw new Exception("A MAIN Index has not been initialized");
	private static Index? _main;

	public IWebGLRenderingContext GL => _gl ?? throw new Exception("GL has not yet been initialized");
	private IWebGLRenderingContext? _gl;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await Task.Delay(10);
		_main = this;
		var canvas = Window.Current.Document.GetElementById<Canvas>("mainCanvas");
		_gl = canvas.GetContext<IWebGLRenderingContext>();

		OnCanvasInitialized?.Invoke();
	}

	public void UpdateRender()
	{
		StateHasChanged();
	}

	public async void SetTitle(string title)
		=> await JSRuntime.InvokeVoidAsync("setTitle", title);

	public async Task<string> GetTitle()
		=> await JSRuntime.InvokeAsync<string>("getTitle");

	public async void Log(string text)
		=> await JSRuntime.InvokeVoidAsync("console.log", text);
}