using Azalea.Graphics.OpenGL.Buffers;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using System;
using System.Collections.Generic;
using System.Numerics;
using AzaleaColor = Azalea.Graphics.Colors.Color;

namespace Azalea.Graphics.Rendering;

internal abstract class Renderer : IRenderer
{
	private IVertexBatch<TexturedVertex2D>? defaultQuadBatch;
	protected IVertexBatch? CurrentActiveBatch;

	private readonly INativeTexture[] lastBoundTexture = new INativeTexture[16];
	private int lastActiveTextureUnit = -1;

	private AzaleaColor _clearColor;
	public AzaleaColor ClearColor
	{
		get => _clearColor;
		set
		{
			if (_clearColor == value) return;
			_clearColor = value;
			SetClearColor(value);
		}
	}

	public bool AutomaticallyClear { get; set; } = true;

	protected IWindow Window { get; init; }

	public Renderer(IWindow window)
	{
		Window = window;

		Window.OnClientResized += UpdateViewport;
	}

	protected internal virtual void Initialize()
	{
		_uniformBuffer = new GLUniformBuffer();
		_uniformBuffer.BufferData((IntPtr)128, IntPtr.Zero, GLUsageHint.StaticDraw);
		_uniformBuffer.BindBufferBase(0);

		var defaultVertexShader = Assets.GetText("Shaders/quad_vertexShader.glsl")!;
		var defaultFragmentShader = Assets.GetText("Shaders/quad_fragmentShader.glsl")!;
		_quadShader = CreateShader(defaultVertexShader, defaultFragmentShader);

		var screenVertexShader = Assets.GetText("Shaders/screen_vertexShader.glsl")!;
		var screenFragmentShader = Assets.GetText("Shaders/screen_fragmentShader.glsl")!;
		_screenShader = CreateShader(screenVertexShader, screenFragmentShader);

		defaultQuadBatch = CreateQuadBatch(17000);
		CurrentActiveBatch = defaultQuadBatch;
	}

	private Texture? _whitePixel;
	public Texture WhitePixel => _whitePixel ??= generateWhitePixel();
	private Texture generateWhitePixel()
	{
		var whitePixel = new Image(1, 1,
			new byte[4] { byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue });

		return CreateTexture(whitePixel);
	}

	protected abstract bool SetTextureImplementation(INativeTexture? texture, int unit);

	public void SetViewport(Vector2Int size)
	{
		SetViewportImplementation(size);
	}

	protected virtual void UpdateViewport(Vector2Int size)
	{
		SetViewport(size);
	}

	protected abstract void SetViewportImplementation(Vector2Int size);

	protected abstract IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size);
	protected abstract INativeTexture CreateNativeTexture(int width, int height);
	public Texture CreateTexture(Image image)
	{
		var nativeTexture = CreateNativeTexture(image.Width, image.Height);
		nativeTexture.SetData(image);

		return new Texture(nativeTexture);
	}

	internal virtual void BeginFrame()
	{
		if (AutomaticallyClear) Clear();

		var clientSize = Window.ClientSize;
		var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, clientSize.X, clientSize.Y, 0, 0.1f, 100);

		unsafe
		{
			_uniformBuffer.BufferData((IntPtr)64, (IntPtr)(float*)&projectionMatrix, GLUsageHint.StaticDraw);
		}
	}
	internal virtual void FinishFrame()
	{
		FlushCurrentBatch();
	}

	protected internal virtual void SetClearColor(AzaleaColor value) { }

	public void Clear()
	{
		ClearImplementation(ClearColor);
	}

	protected abstract void ClearImplementation(AzaleaColor color);

	protected internal void FlushCurrentBatch()
	{
		CurrentActiveBatch?.Draw();
	}

	internal bool BindTexture(Texture texture, int unit = 0)
	{
		return BindTexture(texture.NativeTexture, unit);
	}

	internal bool BindTexture(INativeTexture nativeTexture, int unit = 0)
	{
		if (lastActiveTextureUnit == unit && lastBoundTexture[unit] == nativeTexture)
			return true;

		FlushCurrentBatch();
		if (SetTextureImplementation(nativeTexture, unit) == false)
			return false;

		lastBoundTexture[unit] = nativeTexture;
		lastActiveTextureUnit = unit;

		return true;
	}

	#region Shaders

	private IShader? _boundShader;
	private IShader _quadShader;
	private IShader _screenShader;

	public IShader? BoundShader => _boundShader;
	public IShader QuadShader => _quadShader;
	protected IShader ScreenShader => _screenShader;

	private GLUniformBuffer _uniformBuffer;

	public void BindShader(IShader shader)
	{
		if (_boundShader == shader)
			return;

		if (_boundShader != null)
			FlushCurrentBatch();

		BindShaderImplementation(shader);
		_boundShader = shader;
	}

	public void UnbindCurrentShader()
	{
		if (_boundShader == null)
			return;

		FlushCurrentBatch();

		UnbindCurrentShaderImplementation();
		_boundShader = null;
	}

	public IShader CreateShader(string vertexShaderCode, string fragmentShaderCode)
	{
		var shader = CreateShaderImplementation(vertexShaderCode, fragmentShaderCode);

		_uniformBuffer.SetShaderBinding(shader, "Matrices", 0);

		return shader;
	}

	public abstract IShader CreateShaderImplementation(string vertexShaderCode, string fragmentShaderCode);
	public abstract void BindShaderImplementation(IShader shader);
	public abstract void UnbindCurrentShaderImplementation();

	#endregion

	#region Scissor test

	private Stack<RectangleInt> _scissorRectangles = new();

	public void PushScissor(RectangleInt scissorRect)
	{
		var stackEmpty = _scissorRectangles.Count == 0;
		var rectSameAsLast = stackEmpty == false && _scissorRectangles.Peek() == scissorRect;

		if (rectSameAsLast == false)
			FlushCurrentBatch();

		if (stackEmpty)
			SetScissorTestState(enabled: true);

		_scissorRectangles.Push(scissorRect);

		if (rectSameAsLast == false)
			SetScissorTestRectangle(scissorRect);
	}

	public void PopScissor()
	{
		if (_scissorRectangles.Count <= 0) throw new InvalidOperationException("There are not scissor rectangles to be popped");

		var lastRect = _scissorRectangles.Peek();
		_scissorRectangles.Pop();

		var stackEmpty = _scissorRectangles.Count == 0;
		var rectSameAsLast = stackEmpty == false && _scissorRectangles.Peek() == lastRect;

		if (rectSameAsLast == false)
			FlushCurrentBatch();

		if (stackEmpty == false && rectSameAsLast == false)
			SetScissorTestRectangle(_scissorRectangles.Peek());

		if (stackEmpty)
			SetScissorTestState(enabled: false);
	}

	protected abstract void SetScissorTestState(bool enabled);
	protected abstract void SetScissorTestRectangle(RectangleInt scissorRectangle);

	#endregion

	void IRenderer.Initialize() => Initialize();
	void IRenderer.BeginFrame() => BeginFrame();
	void IRenderer.FinishFrame() => FinishFrame();
	IVertexBatch<TexturedVertex2D> IRenderer.DefaultQuadBatch => defaultQuadBatch ?? throw new Exception("Cannot call DefaultQuadBatch before Initialization");
	void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
	IVertexBatch IRenderer.CreateQuadBatch(int size) => CreateQuadBatch(size);
	bool IRenderer.BindTexture(Texture texture, int unit) => BindTexture(texture, unit);
}
