using Azalea.Platform.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Azalea.Graphics.Rendering.XNA.Batches;

internal class XNAVertexBatch : IVertexBatch
{
    private readonly XNARenderer _renderer;
    private readonly GameWrapper _gameWrapper;
    private readonly BasicEffect _effect;

    private VertexPositionColor[] _vertices;
    private int[] _indices;

    private int _vertexCount;
    private int _indexCount;

    private const int MaxVertexCount = 1024;

    public XNAVertexBatch(XNARenderer renderer, GameWrapper gameWrapper)
    {
        _renderer = renderer;
        _gameWrapper = gameWrapper;

        _vertices = new VertexPositionColor[MaxVertexCount];
        _indices = new int[MaxVertexCount];

        _gameWrapper.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

        var viewport = _gameWrapper.GraphicsDevice.Viewport;
        _effect = new BasicEffect(_gameWrapper.GraphicsDevice)
        {
            TextureEnabled = false,
            FogEnabled = false,
            LightingEnabled = false,
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0, 1)
        };
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
                _indexCount / 3);
        }

        _vertexCount = 0;
        _indexCount = 0;

        return _indexCount / 3;
    }

    public void DrawRectangle(float x, float y, float width, float height, Color color)
    {
        ensureSpace(4, 6);

        float left = x;
        float right = x + width;
        float top = y;
        float bottom = y + height;

        Vector2 a = new Vector2(left, top);
        Vector2 b = new Vector2(right, top);
        Vector2 c = new Vector2(right, bottom);
        Vector2 d = new Vector2(left, bottom);

        _indices[_indexCount++] = 0 + _vertexCount;
        _indices[_indexCount++] = 1 + _vertexCount;
        _indices[_indexCount++] = 2 + _vertexCount;
        _indices[_indexCount++] = 0 + _vertexCount;
        _indices[_indexCount++] = 2 + _vertexCount;
        _indices[_indexCount++] = 3 + _vertexCount;

        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color.ToXNAColor());
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color.ToXNAColor());
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color.ToXNAColor());
        _vertices[_vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color.ToXNAColor());
    }

    private void ensureSpace(int newVertexCount, int newIndexCount)
    {
        if (_vertexCount + newVertexCount > _vertices.Length ||
            _indexCount + newIndexCount > _indices.Length)
        {
            Draw();
        }
    }
}
