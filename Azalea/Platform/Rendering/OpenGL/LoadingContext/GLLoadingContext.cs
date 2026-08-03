using Azalea.Lists;
using Azalea.Native.OpenGL;
using Azalea.Threading;
using System;
using System.Text;
using System.Threading;

namespace Azalea.Platform.Rendering.OpenGL.LoadingContext;

/// <summary>
/// An OpenGL context used for loading assets which all other contexts
/// use as a shared context.
/// </summary>
internal partial class GLLoadingContext : ICommandHandler<LoadingCommand>
{
	public readonly nint Handle;

	public readonly Texture WhitePixel;

	public GLLoadingContext()
	{
		Thread = new GLLoadingThread();
		Thread.Start();

		Thread.Initialized.WaitOne();
		Thread.Initialized.Dispose();

		WhitePixel = new Texture();
		this.GenerateTexture(WhitePixel);
		this.TexImage2D(WhitePixel, 1, 1, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue], false);

		Handle = Thread.Context!.Handle;
	}

	public ICommandAwaitable? Enqueue(LoadingCommand command) => Thread.Enqueue(command);

	#region LoadingThread

	internal readonly GLLoadingThread Thread;

	internal partial class GLLoadingThread() : GameThread<LoadingCommand>(1)
	{
		private Windowing.PlatformWindow? _window;
		internal GLContext? Context;

		public override string DisplayName => "GL Loading Thread";
		internal ObservableList<Texture> LoadedTextures { get; } = [];
		internal ObservableList<Program> LoadedPrograms { get; } = [];


		internal EventWaitHandle Initialized = new(false, EventResetMode.ManualReset);

		protected override void Initialize()
		{
			_window = Windowing.PlatformWindow.Create("", Vector2Int.Zero, initiallyVisible: false);
			Context = GLContext.Create(_window.BorrowDeviceContext());
			Context.MakeCurrent();

			Initialized.Set();
		}

		protected override void Update() { }

		protected override void HandleCommand(LoadingCommand command)
		{
			switch (command)
			{
				case GenerateProgramCommand(var program, var vertexShaderCode, var fragmentShaderCode):
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
					LoadedPrograms.Add(program);

					break;
				case GenerateTextureCommand(var texture):
					uint textureHandle = 0;
					GL.GenTextures(1, ref textureHandle);
					GL.BindTexture(GL.TEXTURE_2D, textureHandle);
					GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
					GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
					GL.Flush();

					texture.Initialize(new GLTexture(textureHandle));
					LoadedTextures.Add(texture);
					break;
				case RebindContextCommand():
					Context!.MakeCurrent();
					break;
				case ReleaseContextCommand():
					Context!.Release();
					break;
				case TexImage2DCommand(var texture, var width, var height, var pixels, var generateMipmap):
					if (texture.NativeTexture is not GLTexture glTexture)
						throw new Exception();

					GL.BindTexture(glTexture.Target, glTexture.Handle);

					if (pixels is null)
						GL.TexImage2D(glTexture.Target, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, IntPtr.Zero);
					else
						GL.TexImage2D(glTexture.Target, 0, GL.RGBA, width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, in pixels[0]);

					if (generateMipmap)
						GL.GenerateMipmap(glTexture.Target);

					GL.Flush();
					texture.FinishLoadingOperation();
					break;
			}

			command.Return();
		}
	}

	#endregion
}
