using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Shaders;
using Azalea.Numerics;
using Azalea.Platform;
using System;

namespace Azalea.Graphics.Vulkan;
internal class VulkanRenderer : Renderer
{
	public VulkanRenderer(IWindow window)
		: base(window) { }

	protected internal override void Initialize()
	{
		Console.WriteLine("Ide gas");
		Console.ReadLine();
	}

	public override void BindShaderImplementation(IShader shader)
	{
		throw new NotImplementedException();
	}

	public override IShader CreateShaderImplementation(string vertexShaderCode, string fragmentShaderCode)
	{
		throw new NotImplementedException();
	}

	public override void UnbindCurrentShaderImplementation()
	{
		throw new NotImplementedException();
	}

	protected override void ClearImplementation(Color color)
	{
		throw new NotImplementedException();
	}

	protected override INativeTexture CreateNativeTexture(int width, int height)
	{
		throw new NotImplementedException();
	}

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
	{
		throw new NotImplementedException();
	}

	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		throw new NotImplementedException();
	}

	protected override void SetScissorTestState(bool enabled)
	{
		throw new NotImplementedException();
	}

	protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
	{
		throw new NotImplementedException();
	}

	protected override void SetViewportImplementation(Vector2Int size)
	{
		throw new NotImplementedException();
	}
}
