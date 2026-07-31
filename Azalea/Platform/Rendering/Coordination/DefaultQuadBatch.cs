using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Native.OpenGL;
using Azalea.Utils;
using System.Buffers;
using System.Numerics;

namespace Azalea.Platform.Rendering.Coordination;
public class DefaultQuadBatch : RenderBatch<DefaultQuadBatchVertex>
{
	private readonly PlatformRenderer _renderer;

	private readonly UniformLocation _projectionUniform;
	private readonly UniformLocation _textureUniform;

	private readonly VertexArray _vertexArray;

	public const int MaxQuadCount = 1000;
	private readonly uint[] _indices;

	public DefaultQuadBatch(RenderCoordinator renderCoordinator)
		: base(renderCoordinator)
	{
		_renderer = renderCoordinator.Renderer;
		var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();

		_vertexArray = commandGroup.GenerateVertexArray();
		commandGroup.BindVertexArray(_vertexArray);

		var vertexBuffer = commandGroup.GenerateBuffer();
		commandGroup.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);

		var indexArray = commandGroup.GenerateBuffer();
		commandGroup.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, indexArray);

		_indices = new uint[MaxQuadCount * 6];
		for (uint i = 0, j = 0; i < MaxQuadCount * 4; i += 4, j += 6)
		{
			_indices[j] = i;
			_indices[j + 1] = i + 1;
			_indices[j + 2] = i + 3;
			_indices[j + 3] = i + 2;
			_indices[j + 4] = i + 3;
			_indices[j + 5] = i + 1;
		}
		commandGroup.BufferData(GL.ELEMENT_ARRAY_BUFFER, _indices.Length * sizeof(uint), _indices, GL.STATIC_DRAW, false);

		commandGroup.VertexAttribPointer(0, 2, GL.FLOAT, false, 8 * sizeof(float), 0);
		commandGroup.EnableVertexAttribArray(0);
		commandGroup.VertexAttribPointer(1, 4, GL.FLOAT, false, 8 * sizeof(float), 2 * sizeof(float));
		commandGroup.EnableVertexAttribArray(1);
		commandGroup.VertexAttribPointer(2, 2, GL.FLOAT, false, 8 * sizeof(float), 6 * sizeof(float));
		commandGroup.EnableVertexAttribArray(2);

		commandGroup.BindVertexArray(null);

		_renderer.Thread.SubmitCommandGroup(commandGroup);
		ObjectPool<RenderCommandGroup>.Return(commandGroup);
	}

	private float[]? _vertices;
	private int _nextVertex = 0;
	private const int _vertexSize = 8;

	public void Add(RenderCommandGroup commandQueue, Quad quad, ColorQuad colorQuad, Quad uvCoordinated)
	{
		Add(new DefaultQuadBatchVertex(quad.BottomLeft, colorQuad.BottomLeft, uvCoordinated.BottomLeft));
		Add(new DefaultQuadBatchVertex(quad.BottomRight, colorQuad.BottomRight, uvCoordinated.BottomRight));
		Add(new DefaultQuadBatchVertex(quad.TopRight, colorQuad.TopRight, uvCoordinated.TopRight));
		Add(new DefaultQuadBatchVertex(quad.TopLeft, colorQuad.TopLeft, uvCoordinated.TopLeft));

		if (_nextVertex == MaxQuadCount * 4)
			Draw(commandQueue);
	}

	public static readonly ArrayPool<float> ArrayPool = ArrayPool<float>.Create(
		maxArrayLength: 32768, maxArraysPerBucket: 600);

	internal override void AddImplementation(DefaultQuadBatchVertex vertex)
	{
		_vertices ??= ArrayPool.Rent(MaxQuadCount * 4 * _vertexSize);

		_vertices[_nextVertex * _vertexSize] = vertex.Position.X;
		_vertices[(_nextVertex * _vertexSize) + 1] = vertex.Position.Y;
		_vertices[(_nextVertex * _vertexSize) + 2] = vertex.Color.RNormalized;
		_vertices[(_nextVertex * _vertexSize) + 3] = vertex.Color.GNormalized;
		_vertices[(_nextVertex * _vertexSize) + 4] = vertex.Color.BNormalized;
		_vertices[(_nextVertex * _vertexSize) + 5] = vertex.Color.ANormalized;
		_vertices[(_nextVertex * _vertexSize) + 6] = vertex.TextureCoordinate.X;
		_vertices[(_nextVertex * _vertexSize) + 7] = vertex.TextureCoordinate.Y;

		_nextVertex++;
	}

	internal override void Draw(RenderCommandGroup commandQueue)
	{
		if (_nextVertex is 0)
			return;

		commandQueue.BindVertexArray(_vertexArray);

		commandQueue.BufferData(GL.ARRAY_BUFFER, _nextVertex * _vertexSize * sizeof(float), _vertices, GL.DYNAMIC_DRAW, true);

		commandQueue.DrawElements(GL.TRIANGLES, (_nextVertex / 4) * 6, GL.UNSIGNED_INT, 0);

		_vertices = null;
		_nextVertex = 0;
	}
}

public readonly struct DefaultQuadBatchVertex(Vector2 position, Color color, Vector2 textureCoordinate)
{
	public readonly Vector2 Position = position;
	public readonly Color Color = color;
	public readonly Vector2 TextureCoordinate = textureCoordinate;
}
