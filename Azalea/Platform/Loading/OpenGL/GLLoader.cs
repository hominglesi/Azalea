// SHIPPABLE
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.OpenGL;
using System;
using System.Diagnostics;
using System.Text;

namespace Azalea.Platform.Loading.OpenGL;

internal class GLLoader : PlatformLoader
{
	public GLContext? Context;

	protected override void InitializationLogic()
	{
		var dummyWindow = Windowing.PlatformWindow.Create("", Vector2Int.Zero, initiallyVisible: false, initializeOle: false);
		var dummyDeviceContext = dummyWindow.BorrowDeviceContext();

		var dummyContext = GLContext.CreateSimple(dummyDeviceContext);
		dummyContext.MakeCurrent();

		GL.LoadDynamicFunctions(dummyContext.GetProcAddress);

		Context = GLContext.Create(dummyDeviceContext);
		Context.MakeCurrent();

		WhitePixel = this.CreateTexture(1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue], false, TextureFiltering.Nearest);

		var quadVertex = Assets.GetText("Shaders/DefaultQuadVertex.glsl")!;
		var quadFragment = Assets.GetText("Shaders/DefaultQuadFragment.glsl")!;
		DefaultQuadShader = LoadShader(quadVertex, quadFragment);

		var textFragment = Assets.GetText("Shaders/DefaultTextFragment.glsl")!;
		DefaultTextShader = LoadShader(quadVertex, textFragment);
	}

	protected override void HandleCommandLogic(LoadingCommand command)
	{
		switch (command)
		{
			case CreateTextureCommand(var texture, var width, var height, var pixels, var generateMipmap, var filtering):
				uint textureHandle = 0;
				GL.GenTextures(1, ref textureHandle);
				GL.BindTexture(GL.TEXTURE_2D, textureHandle);

				var filter = filtering == TextureFiltering.Linear ? GL.LINEAR : GL.NEAREST;

				GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, filter);
				GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, filter);

				if (pixels is null)
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, IntPtr.Zero);
				else
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, in pixels[0]);

				if (generateMipmap)
					GL.GenerateMipmap(GL.TEXTURE_2D);

				GL.Flush();

				((GLTexture)texture).Initialize(textureHandle);
				texture.FinishLoadingOperation();

				LoadedTextures.Add(texture);

				break;
			case CreateProgramCommand(var program, var vertexShaderCode, var fragmentShaderCode):
				var vertexShader = GL.CreateShader(GL.VERTEX_SHADER);
				var fragmentShader = GL.CreateShader(GL.FRAGMENT_SHADER);
				unsafe
				{
					var vertexCodeBuffer = Encoding.UTF8.GetBytes(vertexShaderCode);
					var fragmentCodeBuffer = Encoding.UTF8.GetBytes(fragmentShaderCode);
					fixed (byte* vsP = &vertexCodeBuffer[0])
					fixed (byte* fsP = &fragmentCodeBuffer[0])
					{
						var vsLength = vertexCodeBuffer.Length;
						var vsPointer = (IntPtr)vsP;
						var fsLength = fragmentCodeBuffer.Length;
						var fsPointer = (IntPtr)fsP;
						GL.ShaderSource(vertexShader, 1, ref vsPointer, in vsLength);
						GL.ShaderSource(fragmentShader, 1, ref fsPointer, in fsLength);
					}
				}

				GL.CompileShader(vertexShader);
				GL.CompileShader(fragmentShader);

				int vsStatus = 0, fsStatus = 0;
				GL.GetShaderiv(vertexShader, GL.COMPILE_STATUS, ref vsStatus);
				GL.GetShaderiv(fragmentShader, GL.COMPILE_STATUS, ref fsStatus);

				if (vsStatus != 1 || fsStatus != 1)
				{
					var infoLog = new StringBuilder(512);

					if (vsStatus != 1)
					{
						GL.GetShaderInfoLog(vertexShader, 512, out _, infoLog);
						Console.WriteLine("Vertex Shader Compilation Error: " + infoLog.ToString());
					}
					if (fsStatus != 1)
					{
						GL.GetShaderInfoLog(fragmentShader, 512, out _, infoLog);
						Console.WriteLine("Fragment Shader Compilation Error: " + infoLog.ToString());
					}
				}

				var programHandle = GL.CreateProgram();
				GL.AttachShader(programHandle, vertexShader);
				GL.AttachShader(programHandle, fragmentShader);

				GL.LinkProgram(programHandle);
				GL.ValidateProgram(programHandle);

				int linkStatus = 0, validateStatus = 0;
				GL.GetProgramiv(programHandle, GL.LINK_STATUS, ref linkStatus);
				GL.GetProgramiv(programHandle, GL.VALIDATE_STATUS, ref validateStatus);
				if (linkStatus != 1 || validateStatus != 1)
				{
					var programInfoLog = new StringBuilder(512);
					GL.GetProgramInfoLog(programHandle, 512, out _, programInfoLog);
					Console.WriteLine("Program Compilation Error: " + programInfoLog.ToString());
				}

				GL.DeleteShader(vertexShader);
				GL.DeleteShader(fragmentShader);
				GL.Flush();

				program.Initialize(programHandle);
				break;
			case RebindContextCommand():
				Debug.Assert(Context is not null);
				Context.MakeCurrent();
				break;
			case ReleaseContextCommand():
				Debug.Assert(Context is not null);
				Context.Release();
				break;
		}

		command.Return();
	}

	internal override NativeTexture CreateEmptyTexture() => new GLTexture();
}
