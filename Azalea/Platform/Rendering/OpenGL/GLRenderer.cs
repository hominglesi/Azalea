using Azalea.Graphics.Camera;
using Azalea.Graphics.Colors;
using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;
using Azalea.Platform.Windowing;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	private const bool _useFramebuffer = true;

	private static readonly GL.DebugProc _debugCallback = onDebugMessage;

	private readonly IPlatformDeviceContext _deviceContext;
	private GLContext _context;

	private Buffer _uniformBuffer;

	private Program _screenShader;
	private VertexArray _screenVertexArray;
	private Texture _screenColorbuffer;
	private Framebuffer _screenFramebuffer;


	internal GLRenderer(IPlatformDeviceContext deviceContext)
	{
		_deviceContext = deviceContext;

		Thread.Start();
		Thread.InitializedEvent.WaitOne();

		_uniformBuffer = Thread.GenerateBuffer();
		Thread.BindBuffer(GL.UNIFORM_BUFFER, _uniformBuffer);
		Thread.BufferData(GL.UNIFORM_BUFFER, Marshal.SizeOf<Matrix4x4>(), GL.STATIC_DRAW);
		Thread.BindBuffer(GL.UNIFORM_BUFFER, null);

		Thread.BindBufferRange(GL.UNIFORM_BUFFER, 0, _uniformBuffer, 0, Marshal.SizeOf<Matrix4x4>());

		_screenShader = new Program();
		LoadingContext.GenerateProgram(_screenShader, _screenVertexShader, _screenFragmentShader);

		float[] screenVertices = [
		-1.0f,  1.0f,  0.0f, 1.0f,
		-1.0f, -1.0f,  0.0f, 0.0f,
		 1.0f, -1.0f,  1.0f, 0.0f,

		-1.0f,  1.0f,  0.0f, 1.0f,
		 1.0f, -1.0f,  1.0f, 0.0f,
		 1.0f,  1.0f,  1.0f, 1.0f
		];

		_screenVertexArray = Thread.GenerateVertexArray();
		Thread.BindVertexArray(_screenVertexArray);
		var screenVertexBuffer = Thread.GenerateBuffer();
		Thread.BindBuffer(GL.ARRAY_BUFFER, screenVertexBuffer);
		Thread.BufferData(GL.ARRAY_BUFFER, sizeof(float) * screenVertices.Length, screenVertices, GL.STATIC_DRAW, false);
		Thread.EnableVertexAttribArray(0);
		Thread.VertexAttribPointer(0, 2, GL.FLOAT, false, 4 * sizeof(float), 0);
		Thread.EnableVertexAttribArray(1);
		Thread.VertexAttribPointer(1, 2, GL.FLOAT, false, 4 * sizeof(float), 2 * sizeof(float));
		Thread.BindVertexArray(null);

		_screenColorbuffer = Thread.GenerateTexture();
		Thread.TexImage2D(_screenColorbuffer, deviceContext.ClientSize.Value.X, deviceContext.ClientSize.Value.Y, null, false);

		_screenFramebuffer = Thread.GenerateFramebuffer();
		Thread.FramebufferTexture2D(_screenFramebuffer, _screenColorbuffer, GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, 0);

		Thread.UpdateClientSize(deviceContext.ClientSize);
		deviceContext.ClientSize.OnValueChanged += Thread.UpdateClientSize;
	}

	protected override void InitializationLogic()
	{
		assureGLInitialized();

		_context = GLContext.Create(_deviceContext, LoadingContext);
		_context.MakeCurrent();

		GL.Enable(GL.DEBUG_OUTPUT);
		GL.Enable(GL.DEBUG_OUTPUT_SYNCHRONOUS);
		GL.DebugMessageCallback(_debugCallback, nint.Zero);

		GL.wglSwapIntervalEXT(0);
	}

	internal override bool TryHandleCommand(RenderCommand command)
	{
		switch (command)
		{
			case GenerateProgramCommand(var program, var vertexShaderCode, var fragmentShaderCode):
				LoadingContext!.GenerateProgram(program, vertexShaderCode, fragmentShaderCode);
				return true;
		}

		return false;
	}

	private Vector2Int _lastClientSize = Vector2Int.Zero;
	private Vector2Int _clientSize = Vector2Int.Zero;
	private Color? _clearColor = null;
	private RectangleInt? _scissorRectangle = null;

	internal unsafe override void HandleCommandLogic(RenderCommand command)
	{
		switch (command)
		{
			case BindBufferCommand(var type, var buffer):
				if (buffer is null)
					GL.BindBuffer(type, 0);
				else
				{
					Debug.Assert(buffer.Handle is not null);
					GL.BindBuffer(type, buffer.Handle.Value);
				}
				break;
			case BindBufferRangeCommand(var type, var index, var buffer, var offset, var size):
				Debug.Assert(buffer.Handle is not null);
				GL.BindBufferRange(type, index, buffer.Handle.Value, offset, size);
				break;
			case BindFramebufferCommand(var type, var framebuffer):
				Debug.Assert(framebuffer.Handle is not null);
				GL.BindFramebuffer(GL.FRAMEBUFFER, framebuffer.Handle.Value);
				break;
			case BindTextureCommand(var type, var texture):
				texture.AssureReady();
				GL.BindTexture(type, texture.NativeTexture.Handle);
				break;
			case BindVertexArrayCommand(var vertexArray):
				if (vertexArray is null)
					GL.BindVertexArray(0);
				else
				{
					Debug.Assert(vertexArray.Handle is not null);
					GL.BindVertexArray(vertexArray.Handle.Value);
				}
				break;
			case BlendFunctionCommand(var sourceFactor, var destinationFactor):
				GL.BlendFunc(sourceFactor, destinationFactor);
				break;
			case BufferDataCommand(var type, var size, var data, var usage, var _):
				GL.BufferData(type, size, in data[0], usage);
				break;
			case BufferDataEmptyCommand(var type, var size, var usage):
				GL.BufferData(type, size, nint.Zero, usage);
				break;
			case BufferDataFloatCommand(var type, var size, var data, var usage, var _):
				GL.BufferData(type, size, in data[0], usage);
				break;
			case BufferDataUIntCommand(var type, var size, var data, var usage, var _):
				GL.BufferData(type, size, in data[0], usage);
				break;
			case BufferSubDataMatrix4x4Command(var type, var offset, var data):
				GL.BufferSubData(type, offset, Marshal.SizeOf<Matrix4x4>(), (nint)(&data));
				break;
			case ClearCommand(var color):
				if (_clearColor != color)
					GL.ClearColor(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
				GL.Clear(GL.COLOR_BUFFER_BIT);
				break;
			case DisableCommand(var capability):
				GL.Disable(capability);
				break;
			case DrawArraysCommand(var mode, var first, var count):
				GL.DrawArrays(mode, first, count);
				break;
			case DrawElementsCommand(var mode, var count, var type, var offset):
				GL.DrawElements(mode, count, type, offset);
				break;
			case EnableCommand(int capability):
				GL.Enable(capability);
				break;
			case EnableVertexAttribArrayCommand(uint index):
				GL.EnableVertexAttribArray(index);
				break;
			case FramebufferTexture2DCommand(var framebuffer, var texture, var target, var attachment, var textarget, var level):
				Debug.Assert(framebuffer.Handle is not null);
				texture.AssureReady();
				GL.BindFramebuffer(target, framebuffer.Handle.Value);
				GL.FramebufferTexture2D(target, attachment, textarget, texture.NativeTexture.Handle, level);
				GL.BindFramebuffer(target, 0);
				break;
			case GenerateBufferCommand(var buffer):
				uint bufferHandle = 0;
				GL.GenBuffers(1, ref bufferHandle);
				buffer.Initialize(bufferHandle);
				break;
			case GenerateFramebufferCommand(var framebuffer):
				uint framebufferHandle = 0;
				GL.GenFramebuffers(1, ref framebufferHandle);
				framebuffer.Initialize(framebufferHandle);
				break;
			case GenerateMipmapCommand(var target):
				GL.GenerateMipmap(target);
				break;
			case GenerateTextureCommand(var texture):
				uint textureHandle = 0;
				GL.GenTextures(1, ref textureHandle);
				GL.BindTexture(GL.TEXTURE_2D, textureHandle);
				GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
				GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
				GL.BindTexture(GL.TEXTURE_2D, 0);

				texture.Initialize(new GLTexture(textureHandle));
				break;
			case GenerateVertexArrayCommand(var vertexArray):
				uint vertexArrayHandle = 0;
				GL.GenVertexArrays(1, ref vertexArrayHandle);
				vertexArray.Initialize(vertexArrayHandle);
				break;
			case GetUniformLocationCommand(var uniformLocation, var program, var name):
				program.AssureInitialized();
				var nameBytes = Encoding.UTF8.GetBytes(name + "\0");
				int uniformLocationHandle = GL.GetUniformLocation(program.Handle.Value, nameBytes);
				uniformLocation.Initialize(uniformLocationHandle);
				break;
			case PolygonModeCommand(var face, var mode):
				GL.PolygonMode(face, mode);
				break;
			case PrintErrorsCommand():
				var error = GL.GetError();
				while (error != 0)
				{
					Console.WriteLine("OpenGL Error: " + error);
					error = GL.GetError();
				}
				break;
			case PrepareRenderingCommand():
				if (_lastClientSize != _clientSize)
				{
					GL.Viewport(0, 0, _clientSize.X, _clientSize.Y);
					GL.BindBuffer(GL.UNIFORM_BUFFER, _uniformBuffer.Handle!.Value);
					var projectionMatrix = MainCamera.Instance.CreateProjectionMatrix(_clientSize);
					GL.BufferSubData(GL.UNIFORM_BUFFER, 0, Marshal.SizeOf<Matrix4x4>(), (nint)(&projectionMatrix));
					GL.BindBuffer(GL.UNIFORM_BUFFER, 0);

					_screenColorbuffer.AssureReady();
					GL.BindTexture(GL.TEXTURE_2D, _screenColorbuffer.NativeTexture.Handle);
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGB, _clientSize.X, _clientSize.Y, 0, GL.RGB, GL.UNSIGNED_BYTE, nint.Zero);
					GL.BindTexture(GL.TEXTURE_2D, 0);

					_lastClientSize = _clientSize;
				}

				if (_useFramebuffer)
				{
					Debug.Assert(_screenFramebuffer.Handle is not null);
					GL.BindFramebuffer(GL.FRAMEBUFFER, _screenFramebuffer.Handle.Value);
				}
				break;
			case ScissorCommand(var rectangle):
				if (_scissorRectangle == rectangle)
					break;

				if (_scissorRectangle is null && rectangle is not null)
					GL.Enable(GL.SCISSOR_TEST);
				else if (_scissorRectangle is not null && rectangle is null)
					GL.Disable(GL.SCISSOR_TEST);

				_scissorRectangle = rectangle;

				if (_scissorRectangle is not null)
				{
					var screenRectangle =
						MainCamera.Instance.ToWorldSpace(_scissorRectangle.Value);

					if (screenRectangle.Width < 0) screenRectangle.Width = 0;
					if (screenRectangle.Height < 0) screenRectangle.Height = 0;

					GL.Scissor(screenRectangle.X, _clientSize.Y - screenRectangle.Y - screenRectangle.Height, screenRectangle.Width, screenRectangle.Height);
				}
				break;
			case SwapBuffersCommand:
				GL.Disable(GL.SCISSOR_TEST);
				_scissorRectangle = null;

				if (_useFramebuffer)
				{
					GL.BindFramebuffer(GL.FRAMEBUFFER, 0);
					GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
					GL.Clear(GL.COLOR_BUFFER_BIT);

					_screenShader.AssureInitialized();
					GL.UseProgram(_screenShader.Handle.Value);
					Debug.Assert(_screenVertexArray.Handle is not null);
					GL.BindVertexArray(_screenVertexArray.Handle.Value);
					_screenColorbuffer.AssureReady();
					GL.BindTexture(GL.TEXTURE_2D, _screenColorbuffer.NativeTexture.Handle);
					GL.DrawArrays(GL.TRIANGLES, 0, 6);
				}

				_context.SwapBuffers();
				break;
			case TexImage2DCommand(var texture, var width, var height, var pixels, var generateMipmap):
				if (texture.NativeTexture is not GLTexture glTexture)
					throw new Exception();

				GL.BindTexture(GL.TEXTURE_2D, texture.NativeTexture.Handle);

				if (pixels is null)
					GL.TexImage2D(glTexture.Target, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, IntPtr.Zero);
				else
					GL.TexImage2D(glTexture.Target, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, in pixels[0]);

				GL.BindTexture(GL.TEXTURE_2D, 0);

				break;
			case TexParameteriCommand(var texture, var target, var parameter, var value):
				texture.AssureReady();
				GL.BindTexture(target, texture.NativeTexture.Handle);
				GL.TexParameteri(target, parameter, value);
				GL.BindTexture(target, 0);
				break;
			case Uniform1iCommand(var uniformLocation, var int0):
				Debug.Assert(uniformLocation.Handle is not null);
				GL.Uniform1i(uniformLocation.Handle.Value, int0);
				break;
			case Uniform4fCommand(var uniformLocation, var float0, var float1, var float2, var float3):
				Debug.Assert(uniformLocation.Handle is not null);
				GL.Uniform4f(uniformLocation.Handle.Value, float0, float1, float2, float3);
				break;
			case UniformMatrix4fvCommand(var uniformLocation, var count, var transpose, Matrix4x4 value):
				Debug.Assert(uniformLocation.Handle is not null);
				GL.UniformMatrix4fv(uniformLocation.Handle.Value, count, transpose, ref value);
				break;
			case UpdateClientSizeCommand(var clientSize):
				_clientSize = clientSize;
				break;
			case UseProgramCommand(var program):
				program.AssureInitialized();
				GL.UseProgram(program.Handle.Value);
				break;
			case VertexAttribPointerCommand(var index, var size, var type, var normalized, int stride, nint pointer):
				GL.VertexAttribPointer(index, size, type, normalized, stride, pointer);
				break;
			default:
				throw new NotImplementedException("Command handling hasn't been implemented");
		}

		command.Return();
	}

	private static void onDebugMessage(uint source, uint type, uint id, uint severity, int length, nint message, nint userParam)
	{
		if (severity == GL.DEBUG_SEVERITY_NOTIFICATION)
			return;

		Console.WriteLine(Marshal.PtrToStringAnsi(message, length));
	}

	private const string _screenVertexShader = """
		#version 330 core
		layout (location = 0) in vec2 aPos;
		layout (location = 1) in vec2 aTexCoords;

		out vec2 TexCoords;

		void main()
		{
		    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0); 
		    TexCoords = aTexCoords;
		}  
	""";

	private const string _screenFragmentShader = """
		#version 330 core
		out vec4 FragColor;

		in vec2 TexCoords;

		uniform sampler2D screenTexture;

		void main()
		{ 
			FragColor = texture(screenTexture, TexCoords);
		}
	""";
}
