using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Utils;

namespace Azalea.Graphics.Vulkan;
internal class VulkanTexture : Disposable, INativeTexture
{
	private int _width;
	private int _height;
	private VulkanRenderer _renderer;

	public VulkanTexture(VulkanRenderer renderer, int width, int height)
	{
		_renderer = renderer;
		_width = width;
		_height = height;
	}

	public IRenderer Renderer => _renderer;

	public int Width => _width;

	public int Height => _height;

	// Not Implemented
	public void SetData(Image upload) { }

	// Not Implemented
	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter) { }

	// Not Implemented
	protected override void OnDispose() { }
}
