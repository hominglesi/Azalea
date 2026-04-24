using Azalea.Graphics.Camera;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Rendering;

internal abstract class RendererBase : IRenderer
{
	private IVertexBatch<TexturedVertex2D>? defaultQuadBatch;
	private IVertexBatch? currentActiveBatch;

	private readonly INativeTexture[] lastBoundTexture = new INativeTexture[16];
	private int lastActiveTextureUnit = -1;

	internal Shader? _defaultQuadShader;
	public Shader DefaultQuadShader => _defaultQuadShader
		?? throw new Exception("Renderer has not been initialized yet!");
	public INativeShader? ActiveShader { get; private set; }

	private Color _clearColor;
	public Color ClearColor
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

	public RendererBase(IWindow window)
	{
		Window = window;

		Window.OnClientResized += SetViewport;
	}

	public void Initialize()
	{
		defaultQuadBatch = CreateQuadBatch(10000);
		currentActiveBatch = defaultQuadBatch;

		_defaultQuadShader = ShaderBuilder.FromShaderCode(
			Assets.GetText("Shaders/quad_vertex.glsl")!,
			Assets.GetText("Shaders/quad_fragment.glsl")!);
		ShaderLibrary.RegisterShader("QuadShader", _defaultQuadShader);

		BindShader(_defaultQuadShader);
	}

	private ITexture? _whitePixel;
	public ITexture WhitePixel => _whitePixel ??= generateWhitePixel();
	private Texture generateWhitePixel()
	{
		var whitePixel = new Image(1, 1,
			new byte[4] { byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue });

		return CreateTexture(whitePixel);
	}

	public void SetViewport(Vector2Int size)
	{
		SetViewportImplementation(size);
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

	protected abstract INativeShader CreateNativeShader(string vertexCode, string fragmentCode);
	public Shader CreateShader(string vertexCode, string fragmentCode)
	{
		var nativeShader = CreateNativeShader(vertexCode, fragmentCode);

		return new Shader(nativeShader);
	}

	internal virtual void BeginFrame()
	{

	}
	internal virtual void FinishFrame()
	{
		FlushCurrentBatch();
	}

	protected internal virtual void SetClearColor(Color value) { }

	public void Clear()
	{
		ClearImplementation();
	}

	protected abstract void ClearImplementation();

	protected internal void FlushCurrentBatch()
	{
		currentActiveBatch?.Draw();
	}

	protected abstract bool SetTextureImplementation(INativeTexture? texture, int unit);

	bool IRenderer.BindTexture(INativeTexture texture, int unit) => BindTexture(texture, unit);
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

	public void BindShader(Shader shader)
		=> BindShader(shader.NativeShader);

	protected abstract void BindNativeShaderImplementation(INativeShader shader);

	internal void BindShader(INativeShader shader)
	{
		if (ActiveShader == shader)
			return;

		FlushCurrentBatch();
		BindNativeShaderImplementation(shader);

		ActiveShader = shader;
	}

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
		{
			var screenRect = MainCamera.Instance.ToWorldSpace(scissorRect);
			SetScissorTestRectangle(screenRect);
		}

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
		{
			var screenRect = MainCamera.Instance.ToWorldSpace(_scissorRectangles.Peek());
			SetScissorTestRectangle(screenRect);
		}

		if (stackEmpty)
			SetScissorTestState(enabled: false);
	}

	protected abstract void SetScissorTestState(bool enabled);
	protected abstract void SetScissorTestRectangle(RectangleInt scissorRectangle);

	#endregion
	void IRenderer.BeginFrame() => BeginFrame();
	void IRenderer.FinishFrame() => FinishFrame();
	IVertexBatch<TexturedVertex2D> IRenderer.DefaultQuadBatch => defaultQuadBatch ?? throw new Exception("Cannot call DefaultQuadBatch before Initialization");
	void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
	IVertexBatch IRenderer.CreateQuadBatch(int size) => CreateQuadBatch(size);
}
