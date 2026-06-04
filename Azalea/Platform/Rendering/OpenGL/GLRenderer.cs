using Azalea.Graphics.Colors;
using Azalea.Native.OpenGL;
using Azalea.Platform.Windowing;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	private readonly PlatformDeviceContext _deviceContext;
	private GLContext _context;

	internal GLRenderer(PlatformDeviceContext deviceContext)
	{
		_deviceContext = deviceContext;
	}

	protected override void Initialize()
	{
		assureGLInitialized();

		_context = GLContext.Create(_deviceContext);
		_context.MakeCurrent();
	}

	protected override void Update()
	{

	}

	private Color? _clearColor = null;

	internal override void HandleCommand(RenderCommand command)
	{
		switch (command)
		{
			case AttachShaderCommand(var program, var shader):
				Debug.Assert(program.Handle is not null);
				Debug.Assert(shader.Handle is not null);
				GL.AttachShader(program.Handle.Value, shader.Handle.Value);
				break;
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
				Debug.Assert(texture.Handle is not null);
				GL.BindTexture(type, texture.Handle.Value);
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
			case CompileShaderCommand(var shader):
				Debug.Assert(shader.Handle is not null);
				GL.CompileShader(shader.Handle.Value);
				break;
			case DeleteShaderCommand(var shader):
				Debug.Assert(shader.Handle is not null);
				GL.DeleteShader(shader.Handle.Value);
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
				Debug.Assert(texture.Handle is not null);
				GL.BindFramebuffer(target, framebuffer.Handle.Value);
				GL.FramebufferTexture2D(target, attachment, textarget, texture.Handle.Value, level);
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
			case GenerateProgramCommand(var program):
				uint programHandle = GL.CreateProgram();
				program.Initialize(programHandle);
				break;
			case GenerateShaderCommand(var shader, var type):
				uint shaderHandle = GL.CreateShader(type);
				shader.Initialize(shaderHandle);
				break;
			case GenerateTextureCommand(var texture):
				uint textureHandle = 0;
				GL.GenTextures(1, ref textureHandle);
				texture.Initialize(textureHandle);
				break;
			case GenerateVertexArrayCommand(var vertexArray):
				uint vertexArrayHandle = 0;
				GL.GenVertexArrays(1, ref vertexArrayHandle);
				vertexArray.Initialize(vertexArrayHandle);
				break;
			case GetUniformLocationCommand(var uniformLocation, var program, var name):
				Debug.Assert(program.Handle is not null);
				var nameBytes = Encoding.UTF8.GetBytes(name + "\0");
				int uniformLocationHandle = GL.GetUniformLocation(program.Handle.Value, nameBytes);
				uniformLocation.Initialize(uniformLocationHandle);
				break;
			case LinkProgramCommand(var program):
				Debug.Assert(program.Handle is not null);
				GL.LinkProgram(program.Handle.Value);
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
			case PrintProgramCompileStatusCommand(Program program):
				int success = 0;
				GL.GetProgramiv(program.Handle.GetValueOrDefault(), GL.LINK_STATUS, ref success);
				if (success != 1)
				{
					var programInfoLog = new StringBuilder(512);
					GL.GetProgramInfoLog(program.Handle.GetValueOrDefault(), 512, out _, programInfoLog);
					Console.WriteLine("Program Compilation Error: " + programInfoLog.ToString());
				}
				else
					Console.WriteLine("Program Successfully Compiled!");
				break;
			case PrintShaderCompileStatusCommand(Shader shader):
				success = 0;
				GL.GetShaderiv(shader.Handle.GetValueOrDefault(), GL.COMPILE_STATUS, ref success);
				if (success != 1)
				{
					var shaderInfoLog = new StringBuilder(512);
					GL.GetShaderInfoLog(shader.Handle.GetValueOrDefault(), 512, out _, shaderInfoLog);
					Console.WriteLine("Shader Compilation Error: " + shaderInfoLog.ToString());
				}
				else
					Console.WriteLine("Shader Successfully Compiled!");
				break;
			case ShaderSourceCommand(var shader, var sourceCode):
				Debug.Assert(shader.Handle is not null);
				unsafe
				{
					var sourceBuffer = Encoding.UTF8.GetBytes(sourceCode);
					fixed (byte* p = &sourceBuffer[0])
					{
						var length = sourceBuffer.Length;
						var intPointer = (IntPtr)p;
						GL.ShaderSource(shader.Handle.Value, 1, ref intPointer, in length);
					}
				}
				break;
			case SwapBuffersCommand:
				_context.SwapBuffers();
				break;
			case TexImage2DCommand(var target, var level, var internalFormat, var width, var height, var border, var format, var type, var pixels):
				if (pixels is null)
					GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, IntPtr.Zero);
				else
					GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, in pixels[0]);
				break;
			case TexParameteriCommand(var texture, var target, var parameter, var value):
				Debug.Assert(texture.Handle is not null);
				GL.BindTexture(target, texture.Handle.Value);
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
				Debug.Assert(program.Handle is not null);
				GL.UseProgram(program.Handle.Value);
				break;
			case VertexAttribPointerCommand(var index, var size, var type, var normalized, int stride, nint pointer):
				GL.VertexAttribPointer(index, size, type, normalized, stride, pointer);
				break;
			default:
				throw new NotImplementedException("Command handling hasn't been implemented");
		}

		/*

		Console.WriteLine("Processed " + command.GetType().Name);
		
		int glError;

		while ((glError = GL.GetError()) != 0)
			Console.WriteLine("GL ERROR: " + glError);

		*/

		command.Return();
	}

}
