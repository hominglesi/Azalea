using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using System;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Rendering;

internal static partial class WebGL
{
	[JSImport("WebGL.AttachShader", "JSImports")]
	internal static partial void AttachShader([JSMarshalAs<JSType.Any>] object program, [JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.BindBuffer", "JSImports")]
	private static partial void bindBuffer(int type, [JSMarshalAs<JSType.Any>] object buffer);
	internal static void BindBuffer(GLBufferType type, [JSMarshalAs<JSType.Any>] object buffer)
		=> bindBuffer((int)type, buffer);

	[JSImport("WebGL.BindTexture", "JSImports")]
	private static partial void bindTexture(int type, [JSMarshalAs<JSType.Any>] object texture);
	internal static void BindTexture(GLTextureType type, object texture)
		=> bindTexture((int)type, texture);

	[JSImport("WebGL.BlendFuncSeparate", "JSImports")]
	private static partial void blendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);
	internal static void BlendFuncSeparate(GLBlendFunction srcRGB, GLBlendFunction dstRGB, GLBlendFunction srcAlpha, GLBlendFunction dstAlpha)
		=> blendFuncSeparate((int)srcRGB, (int)dstRGB, (int)srcAlpha, (int)dstAlpha);

	[JSImport("WebGL.BufferData", "JSImports")]
	private static partial void bufferData(int target, [JSMarshalAs<JSType.MemoryView>] Span<int> data, int usage);
	internal static void BufferData(GLBufferType target, Span<int> data, GLUsageHint usage)
		=> bufferData((int)target, data, (int)usage);

	[JSImport("WebGL.BufferData", "JSImports")]
	private static partial void bufferData(int target, [JSMarshalAs<JSType.MemoryView>] Span<byte> data, int usage);
	internal static void BufferData(GLBufferType target, Span<byte> data, GLUsageHint usage)
		=> bufferData((int)target, data, (int)usage);

	[JSImport("WebGL.ClearColor", "JSImports")]
	internal static partial void ClearColor(float red, float green, float blue, float alpha);

	[JSImport("WebGL.Clear", "JSImports")]
	private static partial void clear(int mask);
	internal static void Clear(GLBufferBit mask)
		=> clear((int)mask);

	[JSImport("WebGL.CompileShader", "JSImports")]
	internal static partial void CompileShader([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.CreateBuffer", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateBuffer();

	[JSImport("WebGL.CreateProgram", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateProgram();

	[JSImport("WebGL.CreateShader", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	private static partial object createShader(int type);
	internal static object CreateShader(GLShaderType type)
		=> createShader((int)type);

	[JSImport("WebGL.CreateTexture", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateTexture();

	[JSImport("WebGL.DeleteBuffer", "JSImports")]
	internal static partial void DeleteBuffer([JSMarshalAs<JSType.Any>] object buffer);

	[JSImport("WebGL.DeleteProgram", "JSImports")]
	internal static partial void DeleteProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.DeleteShader", "JSImports")]
	internal static partial void DeleteShader([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.Disable", "JSImports")]
	private static partial void disable(int capability);
	internal static void Disable(GLCapability capability)
		=> disable((int)capability);

	[JSImport("WebGL.DrawArrays", "JSImports")]
	private static partial void drawArrays(int mode, int first, int count);
	internal static void DrawArrays(GLBeginMode mode, int first, int count)
		=> drawArrays((int)mode, first, count);

	[JSImport("WebGL.DrawElements", "JSImports")]
	private static partial void drawElements(int mode, int count, int type, int offset);
	internal static void DrawElements(GLBeginMode mode, int count, GLDataType type, int offset)
		=> drawElements((int)mode, count, (int)type, offset);

	[JSImport("WebGL.Enable", "JSImports")]
	private static partial void enable(int capability);
	internal static void Enable(GLCapability capability)
		=> enable((int)capability);

	[JSImport("WebGL.EnableVertexAttribArray", "JSImports")]
	internal static partial void EnableVertexAttribArray(int index);

	[JSImport("WebGL.GetAttribLocation", "JSImports")]
	internal static partial int GetAttribLocation([JSMarshalAs<JSType.Any>] object program, string pname);

	[JSImport("WebGL.GetBufferParameter", "JSImports")]
	private static partial int getBufferParameter(int target, int pname);
	internal static int GetBufferParameter(GLBufferType target, GLParameterName pname)
		=> getBufferParameter((int)target, (int)pname);

	[JSImport("WebGL.GetProgramInfoLog", "JSImports")]
	internal static partial string GetProgramInfoLog([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.GetProgramParameter", "JSImports")]
	private static partial bool getProgramParameter([JSMarshalAs<JSType.Any>] object program, int pname);
	internal static bool GetProgramParameter([JSMarshalAs<JSType.Any>] object program, GLParameterName pname)
		=> getProgramParameter(program, (int)pname);

	[JSImport("WebGL.GetShaderInfoLog", "JSImports")]
	internal static partial string GetShaderInfoLog([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.GetShaderParameter", "JSImports")]
	private static partial bool getShaderParameter([JSMarshalAs<JSType.Any>] object shader, int pname);
	internal static bool GetShaderParameter([JSMarshalAs<JSType.Any>] object shader, GLParameterName pname)
		=> getShaderParameter(shader, (int)pname);

	[JSImport("WebGL.GetShaderSource", "JSImports")]
	internal static partial string GetShaderSource([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.GetUniformLocation", "JSImports")]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object GetUniformLocation([JSMarshalAs<JSType.Any>] object program, string name);

	[JSImport("WebGL.LinkProgram", "JSImports")]
	internal static partial void LinkProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.Scissor", "JSImport")]
	internal static partial void Scissor(int x, int y, int width, int height);

	[JSImport("WebGL.ShaderSource", "JSImports")]
	internal static partial void ShaderSource([JSMarshalAs<JSType.Any>] object shader, string source);

	[JSImport("WebGL.Uniform1i", "JSImports")]
	internal static partial void Uniform1i([JSMarshalAs<JSType.Any>] object location, int value);

	[JSImport("WebGL.Uniform1iv", "JSImports")]
	internal static partial void Uniform1iv([JSMarshalAs<JSType.Any>] object location, [JSMarshalAs<JSType.Array<JSType.Number>>] int[] value);

	[JSImport("WebGL.Uniform4f", "JSImports")]
	internal static partial void Uniform4f([JSMarshalAs<JSType.Any>] object location, float v1, float v2, float v3, float v4);

	internal static void UniformColor([JSMarshalAs<JSType.Any>] object location, Color color)
		=> Uniform4f(location, color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);

	[JSImport("WebGL.UniformMatrix4fv", "JSImports")]
	internal static partial void UniformMatrix4fv([JSMarshalAs<JSType.Any>] object location, bool transpose, [JSMarshalAs<JSType.Array<JSType.Number>>] double[] value);

	[JSImport("WebGL.UseProgram", "JSImports")]
	internal static partial void UseProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.ValidateProgram", "JSImports")]
	internal static partial void ValidateProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.VertexAttribPointer", "JSImports")]
	private static partial void vertexAttribPointer(int index, int size, int type, bool normalized, int stride, int offset);
	internal static void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
		=> vertexAttribPointer(index, size, (int)type, normalized, stride, offset);

	[JSImport("WebGL.Viewport", "JSImports")]
	internal static partial void Viewport(int x, int y, int width, int height);
}
