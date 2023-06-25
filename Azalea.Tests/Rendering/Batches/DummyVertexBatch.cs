using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
