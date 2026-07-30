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
	private static readonly GL.DebugProc _debugCallback = onDebugMessage;

	private readonly PlatformDeviceContext _deviceContext;
	private GLContext _context;

	internal GLRenderer(PlatformDeviceContext deviceContext)
	{
		_deviceContext = deviceContext;

		Thread.Start();
		Thread.InitializedEvent.WaitOne();
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
			case GenerateTextureCommand(var texture):
				LoadingContext!.GenerateTexture(texture);
				return true;
			case TexImage2DCommand(var texture, var width, var height, var pixels, var generateMipmap):
				LoadingContext!.TexImage2D(texture, width, height, pixels, generateMipmap);
				return true;
		}

		return false;
	}

	private Color? _clearColor = null;
	private RectangleInt? _scissorRectangle = null;

	internal override void HandleCommandLogic(RenderCommand command)
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
				if (data is null)
					GL.BufferData(type, size, IntPtr.Zero, usage);
				else
					GL.BufferData(type, size, in data[0], usage);
				break;
			case BufferDataFloatCommand(var type, var size, var data, var usage, var _):
				if (data is null)
					GL.BufferData(type, size, IntPtr.Zero, usage);
				else
					GL.BufferData(type, size, in data[0], usage);
				break;
			case BufferDataUIntCommand(var type, var size, var data, var usage, var _):
				if (data is null)
					GL.BufferData(type, size, IntPtr.Zero, usage);
				else
					GL.BufferData(type, size, in data[0], usage);
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
				GL.Disable(GL.SCISSOR_TEST);
				_scissorRectangle = null;
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

					// Hardcoded for now
					var framebufferHeight = 561;

					GL.Scissor(screenRectangle.X, framebufferHeight - screenRectangle.Y - screenRectangle.Height, screenRectangle.Width, screenRectangle.Height);
				}
				break;
			case SwapBuffersCommand:
				_context.SwapBuffers();
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

	private static void onDebugMessage(uint source, uint type, uint id, uint severity, int length, nint message, nint userParam)
	{
		if (severity == GL.DEBUG_SEVERITY_NOTIFICATION)
			return;

		Console.WriteLine(Marshal.PtrToStringAnsi(message, length));
	}
}
