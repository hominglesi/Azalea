using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.Web.Rendering;

internal class WebGLVertexBatch<TVertex> : Disposable, IVertexBatch<TVertex>
	where TVertex : unmanaged, IVertex
{
	private IWindow _window;
	public readonly Action<TVertex> AddAction;

	private WebGLIndexBuffer _indexBuffer;
	private WebGLVertexBuffer _vertexBuffer;
	private WebGLVertexArray _vertexArray;
	private WebGLShader _shader;

	private float[] _vertices;
	private int _vertexCount;
	private readonly int _stride;

	public WebGLVertexBatch(IWindow window, int size)
	{
		_window = window;
		AddAction = Add;

		var _indices = new ushort[size * IRenderer.INDICES_PER_QUAD];
		for (ushort i = 0, j = 0; i < size * IRenderer.VERTICES_PER_QUAD; i += IRenderer.VERTICES_PER_QUAD, j += IRenderer.INDICES_PER_QUAD)
		{
			_indices[j] = i;
			_indices[j + 1] = (ushort)(i + 1);
			_indices[j + 2] = (ushort)(i + 3);
			_indices[j + 3] = (ushort)(i + 2);
			_indices[j + 4] = (ushort)(i + 3);
			_indices[j + 5] = (ushort)(i + 1);
		}

		_indexBuffer = new WebGLIndexBuffer();
		_indexBuffer.SetData(_indices, GLUsageHint.StaticDraw);
		_vertexBuffer = new WebGLVertexBuffer();

		var vbLayout = new GLVertexBufferLayout();
		vbLayout.AddElement<float>(2);
		vbLayout.AddElement<float>(4);
		vbLayout.AddElement<float>(2);
		_stride = 8;

		_vertexArray = new WebGLVertexArray();
		_vertexArray.AddBuffer(_vertexBuffer, vbLayout);
		_vertices = new float[size * IRenderer.VERTICES_PER_QUAD * _stride];

		_shader = new WebGLShader();
	}

	public int Draw()
	{
		if (_vertexCount == 0)
			return 0;

		_vertexArray.Bind();
		_indexBuffer.Bind();
		_shader.Bind();

		_vertexBuffer.SetData(_vertices.AsSpan(0, _vertexCount * _stride), GLUsageHint.DynamicDraw);

		var clientSize = _window.ClientSize;
		var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, clientSize.X, clientSize.Y, 0, 0.1f, 100);
		_shader.SetUniform("u_Projection", projectionMatrix);
		//_shader.SetUniform("u_Texture", 0);

		var drawnVertices = (_vertexCount / 4) * 6;
		WebGL.DrawElements(GLBeginMode.Triangles, drawnVertices, GLDataType.UnsignedShort, 0);

		_vertexCount = 0;

		return drawnVertices;
	}

	public void Add(TVertex vertex)
	{
		if (vertex is not TexturedVertex2D tVertex) throw new Exception("Only TexturedVertex2D is implemented");

		if (_vertexCount >= _vertices.Length / _stride)
		{
			Draw();
		}

		_vertices[_vertexCount * _stride] = tVertex.Position.X;
		_vertices[(_vertexCount * _stride) + 1] = tVertex.Position.Y;
		_vertices[(_vertexCount * _stride) + 2] = tVertex.Color.RNormalized;
		_vertices[(_vertexCount * _stride) + 3] = tVertex.Color.GNormalized;
		_vertices[(_vertexCount * _stride) + 4] = tVertex.Color.BNormalized;
		_vertices[(_vertexCount * _stride) + 5] = tVertex.Color.ANormalized;
		_vertices[(_vertexCount * _stride) + 6] = tVertex.TexturePosition.X;
		_vertices[(_vertexCount * _stride) + 7] = tVertex.TexturePosition.Y;
		_vertexCount++;
	}

	Action<TVertex> IVertexBatch<TVertex>.AddAction => AddAction;

	protected override void OnDispose()
	{
		_vertexArray.Dispose();
		_indexBuffer.Dispose();
		_vertexBuffer.Dispose();
		_shader.Dispose();
	}
}
