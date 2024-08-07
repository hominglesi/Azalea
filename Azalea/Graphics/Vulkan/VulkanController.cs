using Azalea.Extentions;
using Azalea.IO.Resources;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Graphics.Vulkan;
internal unsafe class VulkanController
{
	private IntPtr _handle;

	public VulkanController(IntPtr window)
	{
		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var fragShader = Assets.GetStream("Shaders/frag.spv");
		var fragShaderData = fragShader!.ReadBytesToArray((int)fragShader!.Length);
		var vertShader = Assets.GetStream("Shaders/vert.spv");
		var vertShaderData = vertShader!.ReadBytesToArray((int)vertShader!.Length);

		fixed (byte* fragShaderDataPointer = fragShaderData)
		fixed (byte* vertShaderDataPointer = vertShaderData)
		{
			_handle = createVulkanController(window, processHandle,
				vertShaderDataPointer, vertShaderData.Length,
				fragShaderDataPointer, fragShaderData.Length);
		}
	}

	public void Destroy() => destroyVulkanController(_handle);

	public uint CreateTexture(Image image)
	{
		fixed (void* imageData = image.Data)
		{
			return createTexture(_handle, image.Width, image.Height, image.ChannelCount, imageData);
		}
	}
	public void BeginFrame() => beginFrame(_handle);
	public void FinishFrame() => finishFrame(_handle);

	public void PushQuad(VulkanVertex topLeft, VulkanVertex topRight, VulkanVertex bottomRight, VulkanVertex bottomLeft)
		=> pushQuad(_handle, topLeft, topRight, bottomRight, bottomLeft);

	public void SetProjectionMatrix(Matrix4x4 matrix) => setProjectionMatrix(_handle, matrix);
	public void SetFramebufferResized() => setFramebufferResized(_handle);

	#region Imports

	[DllImport("AzaleaNative.dll", EntryPoint = "CreateVulkanController")]
	private static extern IntPtr createVulkanController(IntPtr hwnd, IntPtr hInstance, byte* vertShader, int vertShaderLength, byte* fragShader, int fragShaderLength);

	[DllImport("AzaleaNative.dll", EntryPoint = "DestroyVulkanController")]
	private static extern void destroyVulkanController(IntPtr vulkanController);

	[DllImport("AzaleaNative.dll", EntryPoint = "VulkanCreateTexture")]
	private static extern uint createTexture(IntPtr vulkanController, int width, int height, int channelCount, void* data);

	[DllImport("AzaleaNative.dll", EntryPoint = "VulkanBeginFrame")]
	private static extern void beginFrame(IntPtr vulkanController);

	[DllImport("AzaleaNative.dll", EntryPoint = "VulkanFinishFrame")]
	private static extern void finishFrame(IntPtr vulkanController);

	[DllImport("AzaleaNative.dll", EntryPoint = "VulkanPushQuad")]
	private static extern void pushQuad(IntPtr vulkanController, VulkanVertex topLeft, VulkanVertex topRight, VulkanVertex bottomRight, VulkanVertex bottomLeft);

	[DllImport("AzaleaNative.dll", EntryPoint = "VulkanSetProjectionMatrix")]
	private static extern void setProjectionMatrix(IntPtr vulkanController, Matrix4x4 projectionMatrix);

	[DllImport("AzaleaNative.dll", EntryPoint = "VulkanSetFramebufferResized")]
	private static extern void setFramebufferResized(IntPtr vulkanController);

	#endregion

}
