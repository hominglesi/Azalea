using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Utils;

namespace Azalea.Graphics.Vulkan;
internal class VulkanTexture : Disposable, INativeTexture
{
	private uint _textureIndex;
	private int _width;
	private int _height;
	private VulkanRenderer _renderer;

	public VulkanTexture(VulkanRenderer renderer, Image image)
	{
		_renderer = renderer;
		_width = image.Width;
		_height = image.Height;

		_textureIndex = renderer.Controller.CreateTexture(image);
	}

	public IRenderer Renderer => _renderer;

	public int Width => _width;

	public int Height => _height;

	public uint TextureIndex => _textureIndex;

	// Not Implemented
	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter) { }

	// Not Implemented
	protected override void OnDispose() { }
}
