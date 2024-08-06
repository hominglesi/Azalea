using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Utils;
using System;

namespace Azalea.Graphics.Vulkan;
internal class VulkanVertexBatch<TVertex> : Disposable, IVertexBatch<TVertex>
	where TVertex : unmanaged, IVertex
{
	public Action<TVertex> AddAction => Add;

	// Not Implemented
	public void Add(TVertex vertex) { }

	// Not Implemented
	public int Draw() => 0;

	// Not Implemented
	protected override void OnDispose() { }
}
