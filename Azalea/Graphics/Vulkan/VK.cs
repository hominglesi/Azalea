using Azalea.Graphics.Vulkan.Structs;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Graphics.Vulkan;
internal static class VK
{
	private const string VulkanPath = "vulkan-1";

	[DllImport(VulkanPath, EntryPoint = "vkCreateInstance")]
	public static extern Result CreateInsance(ref InstanceCreateInfo createInfo, ref AllocationCallbacks allocator, ref IntPtr instance);

	[DllImport(VulkanPath, EntryPoint = "vkCreateWin32SurfaceKHR")]
	public static extern Result CreateWin32Surface(IntPtr instance, ref Win32SurfaceCreateInfo createInfo, ref AllocationCallbacks allocator, ref IntPtr surface);
}
