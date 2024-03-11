namespace Azalea.Graphics.Vulkan;

//Mapping from https://registry.khronos.org/vulkan/specs/1.3-extensions/html/chap3.html#VkStructureType
internal enum StructureType
{
	ApplicationInfo = 0,
	InstanceCreateInfo = 1,
	DeviceQueueCreateInfo = 2,
	DeviceCreateInfo = 3,
	SubmitInfo = 4,
	MemoryAllocateInfo = 5,
	//...
}
