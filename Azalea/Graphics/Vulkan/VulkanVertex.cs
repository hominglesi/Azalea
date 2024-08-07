using System.Numerics;

namespace Azalea.Graphics.Vulkan;
internal struct VulkanVertex
{
	public Vector2 Position;
	public Vector3 Color;
	public uint TextureIndex;
	public Vector2 TextureCoordinate;
}
