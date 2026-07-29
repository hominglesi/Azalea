using Azalea.Graphics.Camera;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Utils;
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
		var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();

		_program = RenderCoordinator.CreateStandardProgram(commandGroup,
			_vertexShaderSource, _fragmentShaderSource);

		commandGroup.UseProgram(_program);

		_projectionUniform = commandGroup.GetUniformLocation(_program, "u_Projection");
		_textureUniform = commandGroup.GetUniformLocation(_program, "u_Texture");

		commandGroup.Uniform1i(_textureUniform, 0);

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

	public void Add(RenderCommandGroup commandQueue, Quad quad, ColorQuad colorQuad)
	{
		Add(new DefaultQuadBatchVertex(quad.BottomLeft, colorQuad.BottomLeft, Rectangle.One.BottomLeft));
		Add(new DefaultQuadBatchVertex(quad.BottomRight, colorQuad.BottomRight, Rectangle.One.BottomRight));
		Add(new DefaultQuadBatchVertex(quad.TopRight, colorQuad.TopRight, Rectangle.One.TopRight));
		Add(new DefaultQuadBatchVertex(quad.TopLeft, colorQuad.TopLeft, Rectangle.One.TopLeft));

		if (_nextVertex == MaxQuadCount * 4)
			Draw(commandQueue);
	}

	internal override void AddImplementation(DefaultQuadBatchVertex vertex)
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

	internal override void Draw(RenderCommandGroup commandQueue)
	{
		if (_nextVertex is 0)
			return;

		if (_lastScreenSize != new Vector2Int(800, 600))
		{
			_lastScreenSize = new Vector2Int(800, 600);

			var projectionMatrix = MainCamera.Instance.CreateProjectionMatrix(_lastScreenSize);

			var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();
			commandGroup.UseProgram(_program);
			commandGroup.UniformMatrix4fv(_projectionUniform, 1, false, projectionMatrix);
			_renderer.Thread.SubmitCommandGroup(commandGroup);
			ObjectPool<RenderCommandGroup>.Return(commandGroup);
		}

		commandQueue.BindVertexArray(_vertexArray);

		commandQueue.BufferData(GL.ARRAY_BUFFER, _nextVertex * _vertexSize * sizeof(float), _vertices, GL.DYNAMIC_DRAW, true);

		commandQueue.UseProgram(_program);

		commandQueue.DrawElements(GL.TRIANGLES, (_nextVertex / 4) * 6, GL.UNSIGNED_INT, 0);

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
