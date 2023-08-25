using Azalea.Extentions.MatrixExtentions;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform;
using nkast.Wasm.Canvas.WebGL;
using System;
using System.Numerics;

namespace Azalea.Web.Graphics.Blazor.Batches;

internal class BlazorVertexBatch<TVertex> : IVertexBatch<TVertex>
	where TVertex : unmanaged, IVertex
{
	private readonly IWebGLRenderingContext _gl;
	private readonly IWindow _window;

	private WebGLBuffer _vertexBuffer;
	private WebGLBuffer _indexBuffer;
	private WebGLProgram _shader;

	public Action<TVertex> AddAction;

	private readonly float[] _vertices;
	private readonly ushort[] _indices;

	private int _vertexCount;

	private static readonly string VertexShaderSource = @"
        attribute vec2 vPos;
        attribute vec3 vCol;
        attribute vec2 vTex;
        
        uniform mat4 uProjection;

        varying lowp vec3 oCol;
        varying lowp vec2 oTex;

        void main()
        {
            gl_Position = uProjection * vec4(vPos.x, vPos.y, 1.0, 1.0);
            oCol = vCol;
            oTex = vTex;
        }
        ";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
        varying lowp vec3 oCol;
        varying lowp vec2 oTex;

        uniform sampler2D uTexture;

        void main()
        {
            gl_FragColor = texture2D(uTexture, oTex) * vec4(oCol.x, oCol.y, oCol.z, 1.0);
        }
        ";

	public BlazorVertexBatch(IWebGLRenderingContext gl, IWindow window, int size)
	{
		_gl = gl;
		_window = window;

		_vertices = new float[size * IRenderer.VERTICES_PER_QUAD * 7];
		_indices = new ushort[size * IRenderer.INDICES_PER_QUAD];

		for (ushort i = 0, j = 0; i < size * IRenderer.VERTICES_PER_QUAD; i += IRenderer.VERTICES_PER_QUAD, j += IRenderer.INDICES_PER_QUAD)
		{
			_indices[j] = i;
			_indices[j + 1] = (ushort)(i + 1);
			_indices[j + 2] = (ushort)(i + 3);
			_indices[j + 3] = (ushort)(i + 2);
			_indices[j + 4] = (ushort)(i + 3);
			_indices[j + 5] = (ushort)(i + 1);
		}

		AddAction = Add;

		_vertexBuffer = _gl.CreateBuffer();
		_gl.BindBuffer(WebGLBufferType.ARRAY, _vertexBuffer);

		_indexBuffer = _gl.CreateBuffer();
		_gl.BindBuffer(WebGLBufferType.ELEMENT_ARRAY, _indexBuffer);
		_gl.BufferData(WebGLBufferType.ELEMENT_ARRAY, _indices, WebGLBufferUsageHint.STATIC_DRAW);

		var vertexShader = _gl.CreateShader(WebGLShaderType.VERTEX);
		_gl.ShaderSource(vertexShader, VertexShaderSource);
		_gl.CompileShader(vertexShader);

		var infoLog = _gl.GetShaderInfoLog(vertexShader);
		if (string.IsNullOrEmpty(infoLog) == false)
			Pages.Index.MAIN.Log(infoLog);

		var fragmentShader = _gl.CreateShader(WebGLShaderType.FRAGMENT);
		_gl.ShaderSource(fragmentShader, FragmentShaderSource);
		_gl.CompileShader(fragmentShader);

		infoLog = _gl.GetShaderInfoLog(fragmentShader);
		if (string.IsNullOrEmpty(infoLog) == false)
			Pages.Index.MAIN.Log(infoLog);

		_shader = _gl.CreateProgram();
		_gl.AttachShader(_shader, vertexShader);
		_gl.AttachShader(_shader, fragmentShader);
		_gl.LinkProgram(_shader);

		infoLog = _gl.GetProgramInfoLog(_shader);
		if (string.IsNullOrEmpty(infoLog) == false)
			Pages.Index.MAIN.Log(infoLog);

		_gl.VertexAttribPointer(0, 2, WebGLDataType.FLOAT, false, 7 * sizeof(float), 0);
		_gl.VertexAttribPointer(1, 3, WebGLDataType.FLOAT, false, 7 * sizeof(float), 2 * sizeof(float));
		_gl.VertexAttribPointer(2, 2, WebGLDataType.FLOAT, false, 7 * sizeof(float), 5 * sizeof(float));
		_gl.EnableVertexAttribArray(0);
		_gl.EnableVertexAttribArray(1);
		_gl.EnableVertexAttribArray(2);

		_gl.Enable(WebGLCapability.BLEND);
		_gl.BlendFuncSeparate(WebGLBlendFunc.SRC_ALPHA, WebGLBlendFunc.ONE_MINUS_SRC_ALPHA,
			WebGLBlendFunc.ONE, WebGLBlendFunc.ONE_MINUS_CONSTANT_ALPHA);
	}

	public int Draw()
	{
		if (_vertexCount == 0)
			return 0;

		_gl.BindBuffer(WebGLBufferType.ARRAY, _vertexBuffer);
		_gl.UseProgram(_shader);

		_gl.BufferData(WebGLBufferType.ARRAY, _vertices, WebGLBufferUsageHint.STREAM_DRAW);

		var windowSize = _window.ClientSize;
		var projection = Matrix4x4.CreateOrthographicOffCenter(0, windowSize.X, windowSize.Y, 0, 0.1f, 100);


		var projectionUniform = _gl.GetUniformLocation(_shader, "uProjection");
		if (projectionUniform.Uid == -1) throw new Exception($"uProjection uniform not found in shader");
		_gl.UniformMatrix4fv(projectionUniform, projection.ToFloatArray());

		var textureLocation = _gl.GetUniformLocation(_shader, "uTexture");
		if (textureLocation.Uid == -1) throw new Exception($"uTexture uniform not found in shader");
		_gl.Uniform1i(textureLocation, 0);

		_gl.DrawElements(WebGLPrimitiveType.TRIANGLES, (_vertexCount / 4) * 6, WebGLDataType.USHORT, 0);

		_vertexCount = 0;

		return _vertexCount / 2;
	}

	public void Add(TVertex vertex)
	{
		if (vertex is not TexturedVertex2D tVertex) throw new Exception("Only TexturedVertex2D is implemented");

		if (_vertexCount >= _vertices.Length / 7)
		{
			Draw();
		}

		_vertices[_vertexCount * 7] = tVertex.Position.X;
		_vertices[(_vertexCount * 7) + 1] = tVertex.Position.Y;
		_vertices[(_vertexCount * 7) + 2] = tVertex.Color.RNormalized;
		_vertices[(_vertexCount * 7) + 3] = tVertex.Color.GNormalized;
		_vertices[(_vertexCount * 7) + 4] = tVertex.Color.BNormalized;
		_vertices[(_vertexCount * 7) + 5] = tVertex.TexturePosition.X;
		_vertices[(_vertexCount * 7) + 6] = tVertex.TexturePosition.Y;
		_vertexCount++;
	}

	Action<TVertex> IVertexBatch<TVertex>.AddAction => AddAction;
}
