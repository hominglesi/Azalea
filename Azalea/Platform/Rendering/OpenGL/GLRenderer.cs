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
using System.Threading;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	private const bool _redrawOnResize = true;

	private static readonly GL.DebugProc _debugCallback = onDebugMessage;

	private readonly IPlatformDeviceContext _deviceContext;
	private GLContext? _context;

	private Buffer _uniformBuffer;

	private int _activeScreenFramebuffer = 0;
	private int _lastActiveScreenFramebuffer = 0;
	private const int __screenFramebufferCount = 3;
	private readonly ScreenFramebuffer[] _screenFramebuffers = new ScreenFramebuffer[__screenFramebufferCount];

	private GLContext? _windowContext;
	private Framebuffer? _windowFramebuffer;

	private readonly object _windowRedrawingLock = new();

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

		for (int i = 0; i < __screenFramebufferCount; i++)
		{
			var colorbuffer = Thread.GenerateTexture();
			var framebuffer = Thread.GenerateFramebuffer();
			Thread.FramebufferTexture2D(framebuffer, colorbuffer, GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, 0);

			_screenFramebuffers[i] = new(framebuffer, colorbuffer, Vector2Int.Zero);
		}

		deviceContext.ClientSize.OnValueChanged += newClientSize =>
		{
			// This code is gonna be run on the window thread
			// since that's the thread that changes the client size
			// We have a separate glContext that shares its resources with
			// the rendering thread so it can use the screen framebuffer
			// and blit it to the window on resize

			if (_redrawOnResize == false || newClientSize == Vector2Int.Zero)
				return;

			Monitor.Enter(_windowRedrawingLock);

			if (_windowContext is null)
			{
				_windowContext = GLContext.Create(_deviceContext, LoadingContext);
				_windowContext.MakeCurrent();

				_windowFramebuffer = new Framebuffer();
				uint windowFramebufferHandle = 0;
				GL.GenFramebuffers(1, ref windowFramebufferHandle);
				_windowFramebuffer.Initialize(windowFramebufferHandle);
			}

			var lastActiveFramebuffer = _screenFramebuffers[_lastActiveScreenFramebuffer];

			if (Monitor.TryEnter(lastActiveFramebuffer.TextureLock) == false)
			{
				Console.WriteLine($"Window redraw failed to enter lock!");
				Monitor.Enter(lastActiveFramebuffer.TextureLock);
			}

			lastActiveFramebuffer.Texture.AssureReady();

			Debug.Assert(_windowFramebuffer is not null);
			Debug.Assert(_windowFramebuffer.Handle.HasValue);
			GL.BindFramebuffer(GL.FRAMEBUFFER, _windowFramebuffer.Handle.Value);
			GL.FramebufferTexture2D(GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0,
				GL.TEXTURE_2D, lastActiveFramebuffer.Texture.NativeTexture.Handle, 0);

			GL.BindFramebuffer(GL.READ_FRAMEBUFFER, _windowFramebuffer.Handle.Value);
			GL.BindFramebuffer(GL.DRAW_FRAMEBUFFER, 0);

			var colorbufferSize = lastActiveFramebuffer.Size;

			GL.Viewport(0, 0, newClientSize.X, newClientSize.Y);

			if (FramebufferSize == Vector2Int.Zero)
			{
				if (_clearColor.HasValue)
					GL.ClearColor(_clearColor.Value.RNormalized, _clearColor.Value.GNormalized, _clearColor.Value.BNormalized, 1);
				GL.Clear(GL.COLOR_BUFFER_BIT);

				GL.BlitFramebuffer(0, colorbufferSize.Y - Math.Min(colorbufferSize.Y, newClientSize.Y), Math.Min(colorbufferSize.X, newClientSize.X), colorbufferSize.Y,
					0, newClientSize.Y - Math.Min(colorbufferSize.Y, newClientSize.Y), Math.Min(colorbufferSize.X, newClientSize.X), newClientSize.Y,
					GL.COLOR_BUFFER_BIT, GL.LINEAR);
			}
			else
			{
				GL.BlitFramebuffer(0, 0, colorbufferSize.X, colorbufferSize.Y,
					0, 0, newClientSize.X, newClientSize.Y,
					GL.COLOR_BUFFER_BIT, GL.NEAREST);
			}

			_windowContext.SwapBuffers();
			GL.Flush();

			Monitor.Exit(lastActiveFramebuffer.TextureLock);
			Monitor.Exit(_windowRedrawingLock);
		};
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

	private Vector2Int _intendedClientSize;
	private Color? _clearColor = null;
	private RectangleInt? _scissorRectangle = null;

	protected unsafe override void HandleCommandLogic(RenderCommand command)
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
				GL.BindFramebuffer(type, framebuffer.Handle.Value);
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
				{
					GL.ClearColor(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
					_clearColor = color;
				}

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
			case PrepareRenderingCommand(var intendedClientSize):
				var screenFramebuffer = _screenFramebuffers[_activeScreenFramebuffer];
				Monitor.Enter(screenFramebuffer.TextureLock);
				screenFramebuffer.Texture.AssureReady();

				_intendedClientSize = intendedClientSize;
				var targetFramebufferSize = FramebufferSize ==
					Vector2Int.Zero ? _intendedClientSize : FramebufferSize;

				if (screenFramebuffer.Size != targetFramebufferSize)
				{
					GL.BindBuffer(GL.UNIFORM_BUFFER, _uniformBuffer.Handle!.Value);
					var projectionMatrix = MainCamera.Instance.CreateProjectionMatrix(targetFramebufferSize);
					GL.BufferSubData(GL.UNIFORM_BUFFER, 0, Marshal.SizeOf<Matrix4x4>(), (nint)(&projectionMatrix));
					GL.BindBuffer(GL.UNIFORM_BUFFER, 0);

					GL.BindTexture(GL.TEXTURE_2D, screenFramebuffer.Texture.NativeTexture.Handle);
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGB, targetFramebufferSize.X, targetFramebufferSize.Y, 0, GL.RGB, GL.UNSIGNED_BYTE, nint.Zero);
					GL.BindTexture(GL.TEXTURE_2D, 0);

					_screenFramebuffers[_activeScreenFramebuffer].Size = targetFramebufferSize;
				}

				Debug.Assert(screenFramebuffer.Framebuffer.Handle.HasValue);
				GL.BindFramebuffer(GL.FRAMEBUFFER, screenFramebuffer.Framebuffer.Handle.Value);
				GL.Viewport(0, 0, targetFramebufferSize.X, targetFramebufferSize.Y);
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

					GL.Scissor(screenRectangle.X, _screenFramebuffers[_activeScreenFramebuffer].Size.Y - screenRectangle.Y - screenRectangle.Height, screenRectangle.Width, screenRectangle.Height);
				}
				break;
			case SwapBuffersCommand:
				GL.Disable(GL.SCISSOR_TEST);
				_scissorRectangle = null;

				screenFramebuffer = _screenFramebuffers[_activeScreenFramebuffer];

				if (_intendedClientSize != _deviceContext.ClientSize)
				{
					// This means the current frame is invalid and we'll
					// simply redraw it next frame
					Monitor.Exit(screenFramebuffer.TextureLock);
					break;
				}

				if (Monitor.TryEnter(_windowRedrawingLock))
				{
					if (_intendedClientSize == _deviceContext.ClientSize)
					{
						Debug.Assert(screenFramebuffer.Framebuffer.Handle.HasValue);
						GL.BindFramebuffer(GL.READ_FRAMEBUFFER, screenFramebuffer.Framebuffer.Handle.Value);
						GL.BindFramebuffer(GL.DRAW_FRAMEBUFFER, 0);

						GL.Viewport(0, 0, _intendedClientSize.X, _intendedClientSize.Y);
						// We need to make sure that the window hasn't been resized in the mean time
						if (_intendedClientSize == _deviceContext.ClientSize)
							GL.BlitFramebuffer(0, 0, screenFramebuffer.Size.X, screenFramebuffer.Size.Y,
								0, 0, _intendedClientSize.X, _intendedClientSize.Y,
								GL.COLOR_BUFFER_BIT, GL.NEAREST);

						if (_intendedClientSize == _deviceContext.ClientSize)
						{
							Debug.Assert(_context is not null);
							_context.SwapBuffers();
						}

					}

					Monitor.Exit(_windowRedrawingLock);
				}

				GL.Flush();
				_lastActiveScreenFramebuffer = _activeScreenFramebuffer;
				_activeScreenFramebuffer = (_activeScreenFramebuffer + 1) % __screenFramebufferCount;
				Monitor.Exit(screenFramebuffer.TextureLock);
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

	protected override bool TryHandleCommand(RenderCommand command)
	{
		switch (command)
		{
			case GenerateProgramCommand(var program, var vertexShaderCode, var fragmentShaderCode):
				LoadingContext!.GenerateProgram(program, vertexShaderCode, fragmentShaderCode);
				return true;
		}

		return false;
	}

	private static void onDebugMessage(uint source, uint type, uint id, uint severity, int length, nint message, nint userParam)
	{
		if (severity == GL.DEBUG_SEVERITY_NOTIFICATION)
			return;

		Console.WriteLine(Marshal.PtrToStringAnsi(message, length));
	}

	private struct ScreenFramebuffer
	{
		public readonly Framebuffer Framebuffer;
		public readonly Texture Texture;
		public object TextureLock = new();
		public Vector2Int Size;

		public ScreenFramebuffer(Framebuffer framebuffer, Texture texture, Vector2Int size)
		{
			Framebuffer = framebuffer;
			Texture = texture;
			Size = size;
		}
	}
}
