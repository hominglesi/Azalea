using Azalea.Inputs;
using Azalea.Web.Platform.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using nkast.Wasm.Canvas;
using nkast.Wasm.Canvas.WebGL;
using nkast.Wasm.Dom;
using System;
using System.Numerics;
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

	private Canvas? _canvas;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await Task.Delay(10);
		_main = this;
		_canvas = Window.Current.Document.GetElementById<Canvas>("mainCanvas");
		_gl = _canvas.GetContext<IWebGLRenderingContext>();

		OnCanvasInitialized?.Invoke();
	}

	public void UpdateRender()
	{
		FocusCanvas();
		StateHasChanged();
	}

	public void HandleMouseMove(MouseEventArgs e)
		=> Input.MOUSE_POSITION = new Vector2((float)e.ClientX, (float)e.ClientY);

	public void HandleMouseDown(MouseEventArgs e)
	{
		var button = e.Button;
		if (button == 1) button = 2;
		else if (button == 2) button = 1;
		Input.MOUSE_BUTTONS[button].SetDown();
	}
	public void HandleMouseUp(MouseEventArgs e)
	{
		var button = e.Button;
		if (button == 1) button = 2;
		else if (button == 2) button = 1;
		Input.MOUSE_BUTTONS[button].SetUp();
	}

	public void HandleKeyDown(KeyboardEventArgs e)
	{
		var button = BlazorExtentions.ToAzaleaKey(e.Code);
		Input.KEYBOARD_KEYS[(int)button].SetDown();
	}

	public void HandleKeyUp(KeyboardEventArgs e)
	{
		var button = BlazorExtentions.ToAzaleaKey(e.Code);
		Input.KEYBOARD_KEYS[(int)button].SetUp();
	}

	public async void SetTitle(string title)
		=> await JSRuntime.InvokeVoidAsync("setTitle", title);

	public async Task<string> GetTitle()
		=> await JSRuntime.InvokeAsync<string>("getTitle");

	public async void Log(object? text)
		=> await JSRuntime.InvokeVoidAsync("console.log", text?.ToString());

	private async void FocusCanvas()
		=> await JSRuntime.InvokeVoidAsync("focusCanvas");
}