using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace Azalea.Graphics.Veldrid.Batches;

internal class VeldridVertexBatch<TVertex> : IVertexBatch<TVertex>
	where TVertex : unmanaged, IVertex
{
	private static readonly string VertexShaderSource = @"
        #version 430 core
        layout (location = 0) in vec2 Position;
        layout (location = 1) in vec4 Color;
        layout (location = 2) in vec2 TexturePosition;
        
        layout (set = 0, binding = 0) uniform Projection {
            mat4 matrix;
        };


        layout (location = 0) out vec4 oCol;
        layout (location = 1) out vec2 oTex;
        layout (location = 2) out mat4 oMat;

        void main()
        {
            gl_Position = matrix * vec4(Position.x, Position.y, 1.0, 1.0);
            oCol = Color;
            oTex = TexturePosition;
        }
        ";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
        #version 430 core
        layout (location = 0) in vec4 oCol;
        layout (location = 1) in vec2 oTex;
        layout (location = 2) in mat4 oMat;

        layout (set = 1, binding = 0) uniform texture2D Texture;
        layout (set = 1, binding = 1) uniform sampler Sampler;

        layout (location = 0) out vec4 FragColor;

        void main()
        {
            FragColor = texture(sampler2D(Texture, Sampler), oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
        }
        ";

	private VeldridRenderer _renderer;
	private IWindow _window;
	private Shader[] _shaders;
	private DeviceBuffer _vertexBuffer;
	private DeviceBuffer _indexBuffer;
	private DeviceBuffer _projectionBuffer;
	private Pipeline _pipeline;
	private ResourceSet _projectionSet;
	private ResourceLayout _textureLayout;

	public Action<TVertex> AddAction;

	private readonly TexturedVertex2DTemp[] _vertices;
	private readonly ushort[] _indices;

	private uint _vertexCount;

	public unsafe VeldridVertexBatch(VeldridRenderer renderer, IWindow window, int size)
	{
		_renderer = renderer;
		_window = window;

		AddAction = Add;

		_vertices = new TexturedVertex2DTemp[size * IRenderer.VERTICES_PER_QUAD];
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

		_vertexBuffer = _renderer.Factory.CreateBuffer(new BufferDescription((uint)(size * IRenderer.VERTICES_PER_QUAD) * (8 * sizeof(float)), BufferUsage.VertexBuffer));
		_indexBuffer = _renderer.Factory.CreateBuffer(new BufferDescription((uint)(size * IRenderer.INDICES_PER_QUAD) * sizeof(ushort), BufferUsage.IndexBuffer));
		_projectionBuffer = _renderer.Factory.CreateBuffer(new BufferDescription(16 * sizeof(float), BufferUsage.UniformBuffer));

		var _projectionLayout = _renderer.Factory.CreateResourceLayout(new ResourceLayoutDescription(
			new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

		_projectionSet = _renderer.Factory.CreateResourceSet(new ResourceSetDescription(
			_projectionLayout,
			_projectionBuffer));

		_textureLayout = _renderer.Factory.CreateResourceLayout(new ResourceLayoutDescription(
			new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
			new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)));

		_renderer.GraphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);

		var vertexLayout = new VertexLayoutDescription(
			new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
			new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
			new VertexElementDescription("TexturePosition", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

		var vertexShaderDesc = new ShaderDescription(
			ShaderStages.Vertex,
			Encoding.UTF8.GetBytes(VertexShaderSource),
			"main");
		var fragmentShaderDesc = new ShaderDescription(
			ShaderStages.Fragment,
			Encoding.UTF8.GetBytes(FragmentShaderSource),
			"main");

		_shaders = _renderer.Factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

		var pipelineDescription = new GraphicsPipelineDescription()
		{
			BlendState = BlendStateDescription.SingleAlphaBlend,
			DepthStencilState = new DepthStencilStateDescription(
				depthTestEnabled: true,
				depthWriteEnabled: true,
				comparisonKind: ComparisonKind.LessEqual),
			RasterizerState = new RasterizerStateDescription(
				cullMode: FaceCullMode.Back,
				fillMode: PolygonFillMode.Solid,
				frontFace: FrontFace.CounterClockwise,
				depthClipEnabled: false,
				scissorTestEnabled: false),
			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ResourceLayouts = new[] { _projectionLayout, _textureLayout },
			ShaderSet = new ShaderSetDescription(
				vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
				shaders: _shaders),
			Outputs = _renderer.GraphicsDevice.SwapchainFramebuffer.OutputDescription
		};

		_pipeline = _renderer.Factory.CreateGraphicsPipeline(pipelineDescription);
	}

	public int Draw()
	{
		if (_vertexCount == 0)
			return 0;

		_renderer.CommandList.SetVertexBuffer(0, _vertexBuffer);
		_renderer.CommandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
		_renderer.CommandList.SetPipeline(_pipeline);

		_renderer.CommandList.UpdateBuffer(_vertexBuffer, 0, _vertices);

		var windowSize = _window.ClientSize;
		var projection = Matrix4x4.CreateOrthographicOffCenter(0, windowSize.X, windowSize.Y, 0, 0.1f, 100);
		_renderer.CommandList.UpdateBuffer(_projectionBuffer, 0, projection);

		var textureResources = _renderer.GetBoundTextureResources()[0];

		_renderer.CommandList.SetGraphicsResourceSet(0, _projectionSet);
		_renderer.CommandList.SetGraphicsResourceSet(1, textureResources.GetResourceSet(_renderer, _textureLayout));

		_renderer.CommandList.DrawIndexed(
			indexCount: (_vertexCount / 4) * 6,
			instanceCount: (_vertexCount / 4) * 2,
			indexStart: 0,
			vertexOffset: 0,
			instanceStart: 0);

		_vertexCount = 0;
		return ((int)_vertexCount / 4) * 2;
	}

	public void Add(TVertex vertex)
	{
		if (vertex is not TexturedVertex2D tVertex) throw new Exception("Only TexturedVertex2D is implemented");

		if (_vertexCount >= _vertices.Length)
		{
			Draw();
		}

		_vertices[_vertexCount] = new TexturedVertex2DTemp
		{
			Position = tVertex.Position,
			TexturePosition = tVertex.TexturePosition,
			Color = tVertex.Color.ToVector4()
		};
		_vertexCount++;
	}

	Action<TVertex> IVertexBatch<TVertex>.AddAction => AddAction;

	[StructLayout(LayoutKind.Sequential)]
	internal struct TexturedVertex2DTemp : IVertex
	{
		public Vector2 Position;

		public Vector4 Color;

		public Vector2 TexturePosition;
	}
}
