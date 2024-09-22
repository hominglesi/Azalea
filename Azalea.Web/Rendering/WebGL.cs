using Azalea.Graphics.OpenGL.Enums;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Rendering;

public partial class WebGL
{
	[JSImport("clearColor", "JSImports")]
	internal static partial void ClearColor(float r, float g, float b, float a);

	[JSImport("clearScreen", "JSImports")]
	internal static partial void Clear();

	[JSImport("createBuffer", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateBuffer();

	[JSImport("bindBuffer", "JSImports")]
	private static partial void bindBuffer(int type, int buffer);
	internal static void BindBuffer(GLBufferType type, int buffer)
		=> bindBuffer((int)type, buffer);

	[JSImport("bufferData", "JSImports")]
	private static partial void bufferData(int target, int size, int usage);
	internal static void BufferData(GLBufferType target, int size, GLBufferType usage) => bufferData((int)target, size, (int)usage);

	[JSImport("createShader", "JSImports")]
	private static partial void createShader(int type);
	internal static void CreateShader(GLShaderType type)
		=> createShader((int)type);

	[JSImport("shaderSource", "JSImports")]
	internal static partial void ShaderSource([JSMarshalAs<JSType.Any>] object shader, string source);

	[JSImport("compileShader", "JSImports")]
	internal static partial void CompileShader([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("createProgram", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateProgram();

	[JSImport("attachShader", "JSImports")]
	internal static partial void AttachShader([JSMarshalAs<JSType.Any>] object program, [JSMarshalAs<JSType.Any>] object shader);

	[JSImport("linkProgram", "JSImports")]
	internal static partial void LinkProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("vertexAttribPointer", "JSImports")]
	private static partial void vertexAttribPointer(int index, int size, int type, bool normalized, int stride, int offset);
	internal static void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
		=> vertexAttribPointer(index, size, (int)type, normalized, stride, offset);

	[JSImport("enableVertexAttribArray", "JSImports")]
	internal static partial void EnableVertexAttribArray(int index);

	[JSImport("enable", "JSImports")]
	private static partial void enable(int capability);
	internal static void Enable(GLCapability capability)
		=> enable((int)capability);

	[JSImport("disable", "JSImports")]
	private static partial void disable(int capability);
	internal static void Disable(GLCapability capability)
		=> disable((int)capability);

	[JSImport("blendFuncSeparate", "JSImports")]
	private static partial void blendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);
	internal static void BlendFuncSeparate(GLBlendFunction srcRGB, GLBlendFunction dstRGB, GLBlendFunction srcAlpha, GLBlendFunction dstAlpha)
		=> blendFuncSeparate((int)srcRGB, (int)dstRGB, (int)srcAlpha, (int)dstAlpha);

	[JSImport("getUniformLocation", "JSImports")]
	internal static partial void GetUniformLocation([JSMarshalAs<JSType.Any>] object program, string name);

	[JSImport("uniform1i", "JSImports")]
	internal static partial void Uniform1i([JSMarshalAs<JSType.Any>] object location, int value);

	[JSImport("uniform1iv", "JSImports")]
	internal static partial void Uniform1iv([JSMarshalAs<JSType.Any>] object location, [JSMarshalAs<JSType.Array<JSType.Number>>] int[] value);

	[JSImport("uniform4f", "JSImports")]
	internal static partial void Uniform4f([JSMarshalAs<JSType.Any>] object location, float v1, float v2, float v3, float v4);

	[JSImport("uniformMatrix4fv", "JSImports")]
	internal static partial void UniformMatrix4fv([JSMarshalAs<JSType.Any>] object location, bool transpose, [JSMarshalAs<JSType.Array<JSType.Number>>] double[] value);

	[JSImport("drawElements", "JSImport")]
	private static partial void drawElements(int mode, int count, int type, int offset);
	internal static void DrawElements(GLBeginMode mode, int count, GLDataType type, int offset)
		=> drawElements((int)mode, count, (int)type, offset);

	[JSImport("scissor", "JSImport")]
	internal static partial void Scissor(int x, int y, int width, int height);

	[JSImport("setViewport", "JSImport")]
	internal static partial void SetViewport(int x, int y, int width, int height, int minDepth, int maxDepth);

	[JSImport("bindTexture", "JSImports")]
	private static partial void bindTexture(int target, [JSMarshalAs<JSType.Any>] object texture);
	internal static void BindTexture(GLTextureType type, object texture)
		=> bindTexture((int)type, texture);

	[JSImport("createTexture", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateTexture();

}
