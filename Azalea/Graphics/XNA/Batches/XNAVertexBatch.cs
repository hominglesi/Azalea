using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Azalea.Graphics.XNA.Batches;

internal class XNAVertexBatch<TVertex> : IVertexBatch<TVertex>
    where TVertex : unmanaged, IVertex
{
    private readonly XNARenderer _renderer;
    private readonly GameWrapper _gameWrapper;
    private readonly BasicEffect _effect;

    public Action<TVertex> AddAction;

    private VertexPositionColor[] _vertices;
    private int[] _indices;

    private int _vertexCount;

    public XNAVertexBatch(XNARenderer renderer, GameWrapper gameWrapper, int size)
    {
        _renderer = renderer;
        _gameWrapper = gameWrapper;

        _vertices = new VertexPositionColor[size * IRenderer.VERTICES_PER_QUAD];
        _indices = new int[size * IRenderer.INDICES_PER_QUAD];

        for (int i = 0, j = 0; i < _vertices.Length; i += IRenderer.VERTICES_PER_QUAD, j += IRenderer.INDICES_PER_QUAD)
        {
            _indices[j] = i;
            _indices[j + 1] = i + 1;
            _indices[j + 2] = i + 3;
            _indices[j + 3] = i + 2;
            _indices[j + 4] = i + 3;
            _indices[j + 5] = i + 1;
        }

        var viewport = _gameWrapper.GraphicsDevice.Viewport;
        _effect = new BasicEffect(_gameWrapper.GraphicsDevice)
        {
            TextureEnabled = false,
            FogEnabled = false,
            LightingEnabled = false,
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1)
        };

        AddAction = Add;
    }

    public int Draw()
    {
        if (_vertexCount == 0)
            return 0;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _gameWrapper.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                _vertices,
                0,
                _vertexCount,
                _indices,
                0,
                _vertexCount / 2);
        }

        _vertexCount = 0;

        return _vertexCount / 2;
    }

    public void Add(TVertex vertex)
    {
        if (vertex is not PositionColorVertex pcVertex) throw new Exception("Only position color vertex is implemented");

        if (_vertexCount >= _vertices.Length)
        {
            Draw();
        }

        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(pcVertex.Position, 0f), pcVertex.Color.ToXNAColor());
    }

    Action<TVertex> IVertexBatch<TVertex>.AddAction => AddAction;
}
