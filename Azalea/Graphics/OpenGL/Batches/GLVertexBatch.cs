using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform;
using Silk.NET.OpenGL;
using System;
using System.Numerics;

namespace Azalea.Graphics.OpenGL.Batches;

internal class GLVertexBatch<TVertex> : IVertexBatch<TVertex>
	where TVertex : unmanaged, IVertex
{
	private readonly GLRenderer _renderer;
	private readonly GL _gl;
	private readonly IWindow _window;

	private readonly uint _vao;
	private readonly uint _vbo;
	private readonly uint _ebo;
	private readonly uint _shader;

	public Action<TVertex> AddAction;

	private readonly float[] _vertices;
	private readonly uint[] _indices;

	private int _vertexCount;

	private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec2 vPos;
        layout (location = 1) in vec4 vCol;
        layout (location = 2) in vec2 vTex;
        
        uniform mat4 uProjection;

        out vec4 oCol;
        out vec2 oTex;

        void main()
        {
            gl_Position = uProjection * vec4(vPos.x, vPos.y, 1.0, 1.0);
            oCol = vCol;
            oTex = vTex;
        }
        ";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
        #version 330 core
        in vec4 oCol;
        in vec2 oTex;

        uniform sampler2D uTexture;

        out vec4 FragColor;

        void main()
        {
            FragColor = texture(uTexture, oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
        }
        ";

	public unsafe GLVertexBatch(GLRenderer renderer, GL gl, IWindow window, int size)
	{
		_renderer = renderer;
		_gl = gl;
		_window = window;

		AddAction = Add;

		_vertices = new float[size * IRenderer.VERTICES_PER_QUAD * 8];
		_indices = new uint[size * IRenderer.INDICES_PER_QUAD];

		for (uint i = 0, j = 0; i < size * IRenderer.VERTICES_PER_QUAD; i += IRenderer.VERTICES_PER_QUAD, j += IRenderer.INDICES_PER_QUAD)
		{
			_indices[j] = i;
			_indices[j + 1] = i + 1;
			_indices[j + 2] = i + 3;
			_indices[j + 3] = i + 2;
			_indices[j + 4] = i + 3;
			_indices[j + 5] = i + 1;
		}

		_vao = _gl.GenVertexArray();
		_gl.BindVertexArray(_vao);

		_vbo = _gl.GenBuffer();
		_gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

		_ebo = _gl.GenBuffer();
		_gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
		fixed (void* i = &_indices[0])
			_gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(_indices.Length * sizeof(uint)), i, BufferUsageARB.StreamDraw);

		uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
		_gl.ShaderSource(vertexShader, VertexShaderSource);
		_gl.CompileShader(vertexShader);

		var infoLog = _gl.GetShaderInfoLog(vertexShader);
		if (string.IsNullOrEmpty(infoLog) == false)
			Console.WriteLine(infoLog);

		uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
		_gl.ShaderSource(fragmentShader, FragmentShaderSource);
		_gl.CompileShader(fragmentShader);

		infoLog = _gl.GetShaderInfoLog(fragmentShader);
		if (string.IsNullOrEmpty(infoLog) == false)
			Console.WriteLine(infoLog);

		_shader = _gl.CreateProgram();
		_gl.AttachShader(_shader, vertexShader);
		_gl.AttachShader(_shader, fragmentShader);
		_gl.LinkProgram(_shader);

		_gl.GetProgram(_shader, GLEnum.LinkStatus, out var status);
		if (status == 0) Console.WriteLine($"Error linking shader {_gl.GetProgramInfoLog(_shader)}");

		_gl.DetachShader(_shader, vertexShader);
		_gl.DetachShader(_shader, fragmentShader);
		_gl.DeleteShader(vertexShader);
		_gl.DeleteShader(fragmentShader);

		_gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
		_gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(2 * sizeof(float)));
		_gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
		_gl.EnableVertexAttribArray(0);
		_gl.EnableVertexAttribArray(1);
		_gl.EnableVertexAttribArray(2);

		_gl.Enable(EnableCap.Blend);
		_gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	}

	public unsafe int Draw()
	{
		if (_vertexCount == 0)
			return 0;

		_gl.BindVertexArray(_vao);
		_gl.UseProgram(_shader);

		fixed (void* v = &_vertices[0])
			_gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_vertexCount * 8 * sizeof(float)), v, BufferUsageARB.StreamDraw);

		var windowSize = _window.ClientSize;
		var projection = Matrix4x4.CreateOrthographicOffCenter(0, windowSize.X, windowSize.Y, 0, 0.1f, 100);

		var projectionUniform = _gl.GetUniformLocation(_shader, "uProjection");
		if (projectionUniform == -1) throw new Exception($"uProjection uniform not found in shader");
		_gl.UniformMatrix4(projectionUniform, 1, false, (float*)&projection);

		int textureLocation = _gl.GetUniformLocation(_shader, "uTexture");
		if (textureLocation == -1) throw new Exception($"uTexture uniform not found in shader");
		_gl.Uniform1(textureLocation, 0);

		_gl.DrawElements(PrimitiveType.Triangles, (uint)((_vertexCount / 4) * 6), DrawElementsType.UnsignedInt, null);

		_vertexCount = 0;

		return _vertexCount / 2;
	}

	public void Add(TVertex vertex)
	{
		if (vertex is not TexturedVertex2D tVertex) throw new Exception("Only TexturedVertex2D is implemented");

		if (_vertexCount >= _vertices.Length / 8)
		{
			Draw();
		}

		_vertices[_vertexCount * 8] = tVertex.Position.X;
		_vertices[(_vertexCount * 8) + 1] = tVertex.Position.Y;
		_vertices[(_vertexCount * 8) + 2] = tVertex.Color.RNormalized;
		_vertices[(_vertexCount * 8) + 3] = tVertex.Color.GNormalized;
		_vertices[(_vertexCount * 8) + 4] = tVertex.Color.BNormalized;
		_vertices[(_vertexCount * 8) + 5] = tVertex.Color.ANormalized;
		_vertices[(_vertexCount * 8) + 6] = tVertex.TexturePosition.X;
		_vertices[(_vertexCount * 8) + 7] = tVertex.TexturePosition.Y;
		_vertexCount++;
	}

	Action<TVertex> IVertexBatch<TVertex>.AddAction => AddAction;
}
