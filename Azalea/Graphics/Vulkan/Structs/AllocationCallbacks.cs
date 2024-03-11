using System;

namespace Azalea.Graphics.Vulkan;

//Mapping from https://registry.khronos.org/vulkan/specs/1.3-extensions/html/chap11.html#VkAllocationCallbacks
internal readonly struct AllocationCallbacks
{
	private readonly IntPtr _pUserData;
	private readonly IntPtr _pfnAllocation;
	private readonly IntPtr _pfnReallocation;
	private readonly IntPtr _pfnFree;
	private readonly IntPtr _pfnInternalAllocation;
	private readonly IntPtr _pfnInternalFree;

	//...
}
