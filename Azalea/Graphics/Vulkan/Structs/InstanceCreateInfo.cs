using System;

namespace Azalea.Graphics.Vulkan;

//Mapping from https://registry.khronos.org/vulkan/specs/1.3-extensions/html/chap4.html#VkInstanceCreateInfo
internal readonly struct InstanceCreateInfo
{
	private readonly StructureType _sType;
	private readonly IntPtr _pNext;
	private readonly InstanceCreateFlags _flags;
	private readonly ApplicationInfo _pApplicationInfo;
	private readonly uint _enabledLayerCount;
	private readonly string _ppEnabledLayerNames;
	private readonly uint _enabledExtentionCount;
	private readonly string _ppEnabledExtentionNames;

	//...
}
