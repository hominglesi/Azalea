using Azalea.Graphics.Colors;
using Azalea.Native.OpenGL;
using Azalea.Platform.Windowing;
using System;
using System.Diagnostics;

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
			case BindBufferCommand(var type, var buffer) bindBufferCommand:
				if (buffer is null)
					GL.BindBuffer(type, 0);
				else
				{
					Debug.Assert(buffer.Handle is not null);
					GL.BindBuffer(type, buffer.Handle.Value);
				}

				BindBufferCommand.Return(bindBufferCommand);
				break;
			case BufferDataCommand(var type, var size, var data, var usage) bufferDataCommand:
				if (data is null)
					throw new NotImplementedException();

				GL.BufferData(type, size, in data[0], usage);

				BufferDataCommand.Return(bufferDataCommand);
				break;
			case ClearCommand(var color) clearCommand:
				if (_clearColor != color)
					GL.ClearColor(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
				GL.Clear(GL.COLOR_BUFFER_BIT);

				ClearCommand.Return(clearCommand);
				break;
			case GenerateFramebufferCommand(var framebuffer) createFramebufferCommand:
				uint framebufferHandle = 0;
				GL.GenFramebuffers(1, ref framebufferHandle);
				framebuffer.Initialize(framebufferHandle);

				GenerateFramebufferCommand.Return(createFramebufferCommand);
				break;
			case FramebufferTexture2DCommand(var framebuffer, var texture, var target, var attachment, var textarget, var level) framebufferTexture2DCommand:
				Debug.Assert(framebuffer.Handle is not null);
				Debug.Assert(texture.Handle is not null);
				GL.BindFramebuffer(target, framebuffer.Handle.Value);
				GL.FramebufferTexture2D(target, attachment, textarget, texture.Handle.Value, level);
				GL.BindFramebuffer(target, 0);

				FramebufferTexture2DCommand.Return(framebufferTexture2DCommand);
				break;
			case GenerateBufferCommand(var buffer) generateBufferCommand:
				uint bufferHandle = 0;
				GL.GenBuffers(1, ref bufferHandle);
				buffer.Initialize(bufferHandle);

				GenerateBufferCommand.Return(generateBufferCommand);
				break;
			case SwapBuffersCommand swapBuffersCommand:
				_context.SwapBuffers();

				SwapBuffersCommand.Return(swapBuffersCommand);
				break;
			case TexImage2DCommand(var texture, var target, var level, var internalFormat, var width, var height, var border, var format, var type, var pixels) texImage2DCommand:
				Debug.Assert(texture.Handle is not null);
				GL.BindTexture(target, texture.Handle.Value);
				if (pixels is null) GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, IntPtr.Zero);
				else GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, in pixels[0]);
				GL.BindTexture(target, 0);

				TexImage2DCommand.Return(texImage2DCommand);
				break;
			case TexParameteriCommand(var texture, var target, var parameter, var value) texParameteriCommand:
				Debug.Assert(texture.Handle is not null);
				GL.BindTexture(target, texture.Handle.Value);
				GL.TexParameteri(target, parameter, value);
				GL.BindTexture(target, 0);

				TexParameteriCommand.Return(texParameteriCommand);
				break;
		}
	}

}
