using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Veldrid.Batches;
using Azalea.Graphics.Veldrid.Textures;
using Azalea.Numerics;
using Azalea.Platform;
using System;
using System.Collections.Generic;
using Veldrid;

using TextureVeldrid = Veldrid.Texture;

namespace Azalea.Graphics.Veldrid;

internal class VeldridRenderer : Renderer
{
	public readonly GraphicsDevice GraphicsDevice;

	public readonly ResourceFactory Factory;
	public CommandList CommandList;
	public CommandList TextureUploadCommandList;

	private readonly IWindow _window;
	private readonly Dictionary<int, VeldridTextureResources> boundTextureUnits = new();

	public VeldridRenderer(GraphicsDevice graphicsDevice, IWindow window)
	{
		GraphicsDevice = graphicsDevice;
		_window = window;

		Factory = GraphicsDevice.ResourceFactory;

		CommandList = Factory.CreateCommandList();
		TextureUploadCommandList = Factory.CreateCommandList();
	}

	internal override void BeginFrame()
	{
		CommandList.Begin();
		CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);

		base.BeginFrame();
	}

	internal override void FinishFrame()
	{
		base.FinishFrame();

		CommandList.End();
		GraphicsDevice.SubmitCommands(CommandList);
		GraphicsDevice.SwapBuffers();
	}

	internal Dictionary<int, VeldridTextureResources> GetBoundTextureResources() => boundTextureUnits;

	internal void UpdateTexture<T>(TextureVeldrid texture, uint x, uint y, int width, int height, ReadOnlySpan<T> data)
		where T : unmanaged
	{
		GraphicsDevice.UpdateTexture(texture, data, x, y, 0, (uint)width, (uint)height, 1, 0, 0);
	}

	protected override void ClearImplementation(Color color)
	{
		CommandList.ClearColorTarget(0, ClearColor.ToRgbaFloat());
	}

	protected override INativeTexture CreateNativeTexture(int width, int height)
		=> new VeldridTexture(this, width, height);

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new VeldridVertexBatch<TexturedVertex2D>(this, _window, size);

	protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
	{
		if (texture is not VeldridTexture vTexture)
			return false;

		var resources = vTexture.GetResourceList();

		for (int i = 0; i < resources.Count; i++)
			boundTextureUnits[unit++] = resources[i];

		return true;
	}

	protected override void SetScissorTestState(bool enabled)
	{
		//Scissor state is always enabled

		if (enabled == false)
			SetScissorTestRectangle(new RectangleInt(0, 0, 9999, 9999));
	}

	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		CommandList.SetScissorRect(0, (uint)scissorRectangle.X, (uint)scissorRectangle.Y,
			(uint)scissorRectangle.Width, (uint)scissorRectangle.Height);
	}
}
