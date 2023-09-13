using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Azalea.Graphics.Rendering;

internal abstract class Renderer : IRenderer
{
	private IVertexBatch<TexturedVertex2D>? defaultQuadBatch;
	private IVertexBatch? currentActiveBatch;

	private readonly INativeTexture[] lastBoundTexture = new INativeTexture[16];
	private int lastActiveTextureUnit = -1;

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

	protected internal virtual void Initialize()
	{
		defaultQuadBatch = CreateQuadBatch(100);
		currentActiveBatch = defaultQuadBatch;
	}

	private Texture? whitePixel;
	public Texture WhitePixel => whitePixel ??= generateWhitePixel();
	private Texture generateWhitePixel()
	{
		var whitePixel = CreateTexture(1, 1);
		var whitePixelData = new Image<Rgba32>(1, 1, new Rgba32 { R = 255, G = 255, B = 255, A = 255 });
		whitePixel.SetData(new TextureUpload(whitePixelData));
		return whitePixel;
	}

	protected abstract bool SetTextureImplementation(INativeTexture? texture, int unit);

	protected abstract IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size);
	protected abstract INativeTexture CreateNativeTexture(int width, int height);
	public Texture CreateTexture(int width, int height)
		=> CreateTexture(CreateNativeTexture(width, height));

	internal Texture CreateTexture(INativeTexture nativeTexture)
		=> new(nativeTexture);

	internal virtual void BeginFrame() { }
	internal virtual void FinishFrame()
	{
		FlushCurrentBatch();
	}

	protected internal virtual void SetClearColor(Color value) { }

	public void Clear()
	{
		ClearImplementation(ClearColor);
	}

	protected abstract void ClearImplementation(Color color);

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

	void IRenderer.Initialize() => Initialize();
	void IRenderer.BeginFrame() => BeginFrame();
	void IRenderer.FinishFrame() => FinishFrame();
	IVertexBatch<TexturedVertex2D> IRenderer.DefaultQuadBatch => defaultQuadBatch ?? throw new Exception("Cannot call DefaultQuadBatch before Initialization");
	void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
	IVertexBatch IRenderer.CreateQuadBatch(int size) => CreateQuadBatch(size);
	bool IRenderer.BindTexture(Texture texture, int unit) => BindTexture(texture, unit);
}
