using System;

namespace Azalea.Graphics.Vulkan;

//Mapping from https://registry.khronos.org/vulkan/specs/1.3-extensions/html/chap4.html#VkInstanceCreateFlagBits

[Flags]
internal enum InstanceCreateFlags
{
	EnumeratePortabilityBit = 0x00000001
}
