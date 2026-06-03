using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Buffers;
using System.Numerics;

namespace Azalea.Platform.Rendering.Coordination;
public class DefaultQuadBatch : RenderBatch<DefaultQuadBatchVertex>
{
	private readonly PlatformRenderer _renderer;

	private readonly Program _program;
	private readonly UniformLocation _outColorUniform;

	private readonly VertexArray _vertexArray;

	public const int MaxQuadCount = 1000;
	private readonly uint[] _indices;

	public DefaultQuadBatch(RenderCoordinator renderCoordinator)
		: base(renderCoordinator)
	{
		_renderer = renderCoordinator.Renderer;

		_program = renderCoordinator.CreateStandardProgram(_vertexShaderSource, _fragmentShaderSource);

		_outColorUniform = _renderer.GetUniformLocation(_program, "outColor");

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
			_indices[j + 3] = i + 1;
			_indices[j + 4] = i + 2;
			_indices[j + 5] = i + 3;
		}
		_renderer.BufferData(GL.ELEMENT_ARRAY_BUFFER, _indices.Length * sizeof(uint), _indices, GL.STATIC_DRAW, false);

		_renderer.VertexAttribPointer(0, 3, GL.FLOAT, false, 3 * sizeof(float), 0);
		_renderer.EnableVertexAttribArray(0);

		_renderer.BindVertexArray(null);
	}

	private float[]? _vertices;
	private int _nextVertex = 0;
	private const int _vertexSize = 3;

	public void Add(Rectangle rect)
	{
		Add(new DefaultQuadBatchVertex(new(rect.TopLeft, 0)));
		Add(new DefaultQuadBatchVertex(new(rect.TopRight, 0)));
		Add(new DefaultQuadBatchVertex(new(rect.BottomRight, 0)));
		Add(new DefaultQuadBatchVertex(new(rect.BottomLeft, 0)));
	}

	public override void Add(DefaultQuadBatchVertex vertex)
	{
		_vertices ??= ArrayPool<float>.Shared.Rent(MaxQuadCount * 4 * _vertexSize);

		_vertices[_nextVertex * _vertexSize] = vertex.Position.X;
		_vertices[(_nextVertex * _vertexSize) + 1] = vertex.Position.Y;
		_vertices[(_nextVertex * _vertexSize) + 2] = vertex.Position.Z;

		_nextVertex++;
	}

	internal override void Draw(RenderCommandQueue commandQueue)
	{
		if (_nextVertex is 0)
			return;

		commandQueue.UseProgram(_program);
		var blueValue = MathUtils.Map(MathF.Sin(Time.TimeSinceStart), -1, 1, 0, 1);
		commandQueue.Uniform4f(_outColorUniform, 0, blueValue, 1 - blueValue, 1);

		commandQueue.BindVertexArray(_vertexArray);

		commandQueue.BufferData(GL.ARRAY_BUFFER, _nextVertex * _vertexSize * sizeof(float), _vertices, GL.DYNAMIC_DRAW, true);
		commandQueue.DrawElements(GL.TRIANGLES, (_nextVertex / 4) * 6, GL.UNSIGNED_INT, 0);

		commandQueue.BindVertexArray(null);

		_vertices = null;
		_nextVertex = 0;
	}

	private const string _vertexShaderSource = """
		#version 330 core
		layout (location = 0) in vec3 aPos;

		void main()
		{
			gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
		}
	""";

	private const string _fragmentShaderSource = """
		#version 330 core
		out vec4 FragColor;

		uniform vec4 outColor;

		void main()
		{
			FragColor = outColor;
		} 
	""";
}


public readonly struct DefaultQuadBatchVertex(Vector3 position)
{
	public readonly Vector3 Position = position;
}
