using Azalea.Graphics.Vulkan.Enums;
using System;

namespace Azalea.Graphics.Vulkan.Structs;

//Mapping from https://registry.khronos.org/vulkan/specs/1.3-extensions/html/chap34.html#VkWin32SurfaceCreateInfoKHR
internal struct Win32SurfaceCreateInfo
{
	private readonly StructureType _sType;
	private readonly IntPtr _pNext;
	private readonly Win32SurfaceCreateFlags _flags;
	private readonly IntPtr _hInstance;
	private readonly IntPtr _hwnd;

	//...
}
