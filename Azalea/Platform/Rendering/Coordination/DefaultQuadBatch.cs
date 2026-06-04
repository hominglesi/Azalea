using Azalea.Graphics.Camera;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Native.OpenGL;
using Azalea.Numerics;
using System.Buffers;
using System.Numerics;

namespace Azalea.Platform.Rendering.Coordination;
public class DefaultQuadBatch : RenderBatch<DefaultQuadBatchVertex>
{
	private readonly PlatformRenderer _renderer;

	private readonly Program _program;
	private readonly UniformLocation _projectionUniform;
	private readonly UniformLocation _textureUniform;

	private readonly VertexArray _vertexArray;

	public const int MaxQuadCount = 1000;
	private readonly uint[] _indices;

	public DefaultQuadBatch(RenderCoordinator renderCoordinator)
		: base(renderCoordinator)
	{
		_renderer = renderCoordinator.Renderer;

		_program = renderCoordinator.CreateStandardProgram(_vertexShaderSource, _fragmentShaderSource);

		_renderer.UseProgram(_program);

		_projectionUniform = _renderer.GetUniformLocation(_program, "u_Projection");
		_textureUniform = _renderer.GetUniformLocation(_program, "u_Texture");

		_renderer.Uniform1i(_textureUniform, 0);

		_vertexArray = _renderer.GenerateVertexArray();
		_renderer.BindVertexArray(_vertexArray);

		var vertexBuffer = _renderer.GenerateBuffer();
		_renderer.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);

		var indexArray = _renderer.GenerateBuffer();
		_renderer.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, indexArray);

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
		_renderer.BufferData(GL.ELEMENT_ARRAY_BUFFER, _indices.Length * sizeof(uint), _indices, GL.STATIC_DRAW, false);

		_renderer.VertexAttribPointer(0, 2, GL.FLOAT, false, 8 * sizeof(float), 0);
		_renderer.EnableVertexAttribArray(0);
		_renderer.VertexAttribPointer(1, 4, GL.FLOAT, false, 8 * sizeof(float), 2 * sizeof(float));
		_renderer.EnableVertexAttribArray(1);
		_renderer.VertexAttribPointer(2, 2, GL.FLOAT, false, 8 * sizeof(float), 6 * sizeof(float));
		_renderer.EnableVertexAttribArray(2);

		_renderer.BindVertexArray(null);
	}

	private float[]? _vertices;
	private int _nextVertex = 0;
	private const int _vertexSize = 8;

	public void Add(RenderCommandQueue commandQueue, Quad quad, ColorQuad colorQuad)
	{
		Add(new DefaultQuadBatchVertex(quad.BottomLeft, colorQuad.BottomLeft, Rectangle.One.BottomLeft));
		Add(new DefaultQuadBatchVertex(quad.BottomRight, colorQuad.BottomRight, Rectangle.One.BottomRight));
		Add(new DefaultQuadBatchVertex(quad.TopRight, colorQuad.TopRight, Rectangle.One.TopRight));
		Add(new DefaultQuadBatchVertex(quad.TopLeft, colorQuad.TopLeft, Rectangle.One.TopLeft));

		if (_nextVertex == MaxQuadCount * 4)
			Draw(commandQueue);
	}

	public override void Add(DefaultQuadBatchVertex vertex)
	{
		_vertices ??= ArrayPool<float>.Shared.Rent(MaxQuadCount * 4 * _vertexSize);

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

	private Vector2Int _lastScreenSize = Vector2Int.Zero;

	internal override void Draw(RenderCommandQueue commandQueue)
	{
		if (_nextVertex is 0)
			return;

		if (_lastScreenSize != new Vector2Int(800, 600))
		{
			_lastScreenSize = new Vector2Int(800, 600);

			var projectionMatrix = MainCamera.Instance.CreateProjectionMatrix(_lastScreenSize);
			_renderer.BeginCommandGroup();

			_renderer.UseProgram(_program);
			_renderer.UniformMatrix4fv(_projectionUniform, 1, false, projectionMatrix);

			_renderer.SubmitCommandGroup();
		}

		commandQueue.BindVertexArray(_vertexArray);

		commandQueue.BufferData(GL.ARRAY_BUFFER, _nextVertex * _vertexSize * sizeof(float), _vertices, GL.DYNAMIC_DRAW, true);

		commandQueue.UseProgram(_program);

		commandQueue.DrawElements(GL.TRIANGLES, (_nextVertex / 4) * 6, GL.UNSIGNED_INT, 0);

		commandQueue.BindVertexArray(null);

		_vertices = null;
		_nextVertex = 0;
	}

	private const string _vertexShaderSource = """
		#version 330 core
		layout (location = 0) in vec2 vPos;
		layout (location = 1) in vec4 vCol;
		layout (location = 2) in vec2 vTex;

		uniform mat4 u_Projection;

		out vec4 oCol;
		out vec2 oTex;

		void main()
		{
			gl_Position = u_Projection * vec4(vPos.x, vPos.y, 1.0, 1.0);
			oCol = vCol;
			oTex = vTex;
		}
	""";

	private const string _fragmentShaderSource = """
		#version 330 core
		in vec4 oCol;
		in vec2 oTex;

		uniform sampler2D u_Texture;

		out vec4 FragColor;

		void main()
		{
			FragColor = texture(u_Texture, oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
		}
	""";
}


public readonly struct DefaultQuadBatchVertex(Vector2 position, Color color, Vector2 textureCoordinate)
{
	public readonly Vector2 Position = position;
	public readonly Color Color = color;
	public readonly Vector2 TextureCoordinate = textureCoordinate;
}
