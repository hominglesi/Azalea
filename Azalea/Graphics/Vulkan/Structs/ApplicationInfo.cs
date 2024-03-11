using System;

namespace Azalea.Graphics.Vulkan;

//Mapping from https://registry.khronos.org/vulkan/specs/1.3-extensions/html/chap4.html#VkApplicationInfo
internal struct ApplicationInfo
{
	private readonly StructureType _sType;
	private readonly IntPtr _pNext;
	private readonly string _pApplicationName;
	private readonly uint _applicationVersion;
	private readonly string _engineName;
	private readonly uint _engineVersion;
	private readonly uint _apiVersion;

	//...
}
