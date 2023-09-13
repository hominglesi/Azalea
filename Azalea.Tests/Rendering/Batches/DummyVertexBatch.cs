using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using System;

namespace Azalea.Tests.Rendering.Batches;

internal class DummyVertexBatch<TVertex> : IVertexBatch<TVertex>
	where TVertex : unmanaged, IVertex
{
	public Action<TVertex> AddAction;

	public DummyVertexBatch()
	{
		AddAction = Add;
	}

	public int Draw()
	{
		return 0;
	}

	public void Add(TVertex vertex)
	{

	}

	Action<TVertex> IVertexBatch<TVertex>.AddAction => AddAction;
}
