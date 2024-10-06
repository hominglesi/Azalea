using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;
using Azalea.Numerics;
using Azalea.Platform;
using System;
using System.Collections.Generic;
using AzaleaColor = Azalea.Graphics.Colors.Color;

namespace Azalea.Graphics.Rendering;

internal abstract class Renderer : IRenderer
{
	private IVertexBatch<TexturedVertex2D>? defaultQuadBatch;
	private IVertexBatch? currentActiveBatch;

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

		Window.OnClientResized += SetViewport;

		defaultQuadBatch = CreateQuadBatch(10000);
		currentActiveBatch = defaultQuadBatch;
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
		currentActiveBatch?.Draw();
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
	void IRenderer.BeginFrame() => BeginFrame();
	void IRenderer.FinishFrame() => FinishFrame();
	IVertexBatch<TexturedVertex2D> IRenderer.DefaultQuadBatch => defaultQuadBatch ?? throw new Exception("Cannot call DefaultQuadBatch before Initialization");
	void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
	IVertexBatch IRenderer.CreateQuadBatch(int size) => CreateQuadBatch(size);
	bool IRenderer.BindTexture(Texture texture, int unit) => BindTexture(texture, unit);
}
