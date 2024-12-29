using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using System;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web.Rendering;

internal static partial class WebGL
{
	private const string ImportString = "JSImports";

	[JSImport("WebGL.ActiveTexture", ImportString)]
	internal static partial void ActiveTexture(int texture);

	[JSImport("WebGL.AttachShader", ImportString)]
	internal static partial void AttachShader([JSMarshalAs<JSType.Any>] object program, [JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.BindBuffer", ImportString)]
	private static partial void bindBuffer(int type, [JSMarshalAs<JSType.Any>] object buffer);
	internal static void BindBuffer(GLBufferType type, [JSMarshalAs<JSType.Any>] object buffer)
		=> bindBuffer((int)type, buffer);

	[JSImport("WebGL.BindTexture", ImportString)]
	private static partial void bindTexture(int type, [JSMarshalAs<JSType.Any>] object texture);
	internal static void BindTexture(GLTextureType type, object texture)
		=> bindTexture((int)type, texture);

	[JSImport("WebGL.BindVertexArray", ImportString)]
	internal static partial void BindVertexArray([JSMarshalAs<JSType.Any>] object vertexArray);

	[JSImport("WebGL.BlendFuncSeparate", ImportString)]
	private static partial void blendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);
	internal static void BlendFuncSeparate(GLBlendFunction srcRGB, GLBlendFunction dstRGB, GLBlendFunction srcAlpha, GLBlendFunction dstAlpha)
		=> blendFuncSeparate((int)srcRGB, (int)dstRGB, (int)srcAlpha, (int)dstAlpha);

	[JSImport("WebGL.BufferData", ImportString)]
	private static partial void bufferData(int target, [JSMarshalAs<JSType.MemoryView>] Span<byte> data, int usage);
	internal static void BufferData(GLBufferType target, Span<byte> data, GLUsageHint usage)
		=> bufferData((int)target, data, (int)usage);

	[JSImport("WebGL.ClearColor", ImportString)]
	internal static partial void ClearColor(float red, float green, float blue, float alpha);

	[JSImport("WebGL.Clear", ImportString)]
	private static partial void clear(int mask);
	internal static void Clear(GLBufferBit mask)
		=> clear((int)mask);

	[JSImport("WebGL.CompileShader", ImportString)]
	internal static partial void CompileShader([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.CreateBuffer", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateBuffer();

	[JSImport("WebGL.CreateProgram", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateProgram();

	[JSImport("WebGL.CreateShader", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	private static partial object createShader(int type);
	internal static object CreateShader(GLShaderType type)
		=> createShader((int)type);

	[JSImport("WebGL.CreateTexture", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateTexture();

	[JSImport("WebGL.CreateVertexArray", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object CreateVertexArray();

	[JSImport("WebGL.DeleteBuffer", ImportString)]
	internal static partial void DeleteBuffer([JSMarshalAs<JSType.Any>] object buffer);

	[JSImport("WebGL.DeleteProgram", ImportString)]
	internal static partial void DeleteProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.DeleteShader", ImportString)]
	internal static partial void DeleteShader([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.DeleteTexture", ImportString)]
	internal static partial void DeleteTexture([JSMarshalAs<JSType.Any>] object texture);

	[JSImport("WebGL.DeleteVertexArray", ImportString)]
	internal static partial void DeleteVertexArray([JSMarshalAs<JSType.Any>] object vertexArray);

	[JSImport("WebGL.Disable", ImportString)]
	private static partial void disable(int capability);
	internal static void Disable(GLCapability capability)
		=> disable((int)capability);

	[JSImport("WebGL.DrawArrays", ImportString)]
	private static partial void drawArrays(int mode, int first, int count);
	internal static void DrawArrays(GLBeginMode mode, int first, int count)
		=> drawArrays((int)mode, first, count);

	[JSImport("WebGL.DrawElements", ImportString)]
	private static partial void drawElements(int mode, int count, int type, int offset);
	internal static void DrawElements(GLBeginMode mode, int count, GLDataType type, int offset)
		=> drawElements((int)mode, count, (int)type, offset);

	[JSImport("WebGL.Enable", ImportString)]
	private static partial void enable(int capability);
	internal static void Enable(GLCapability capability)
		=> enable((int)capability);

	[JSImport("WebGL.EnableVertexAttribArray", ImportString)]
	internal static partial void EnableVertexAttribArray(int index);

	[JSImport("WebGL.GenerateMipmap", ImportString)]
	private static partial void generateMipmap(int target);
	internal static void GenerateMipmap(GLTextureType target)
		=> generateMipmap((int)target);

	[JSImport("WebGL.GetAttribLocation", ImportString)]
	internal static partial int GetAttribLocation([JSMarshalAs<JSType.Any>] object program, string pname);

	[JSImport("WebGL.GetBufferParameter", ImportString)]
	private static partial int getBufferParameter(int target, int pname);
	internal static int GetBufferParameter(GLBufferType target, GLParameterName pname)
		=> getBufferParameter((int)target, (int)pname);

	[JSImport("WebGL.GetProgramInfoLog", ImportString)]
	internal static partial string GetProgramInfoLog([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.GetProgramParameter", ImportString)]
	private static partial bool getProgramParameter([JSMarshalAs<JSType.Any>] object program, int pname);
	internal static bool GetProgramParameter([JSMarshalAs<JSType.Any>] object program, GLParameterName pname)
		=> getProgramParameter(program, (int)pname);

	[JSImport("WebGL.GetShaderInfoLog", ImportString)]
	internal static partial string GetShaderInfoLog([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.GetShaderParameter", ImportString)]
	private static partial bool getShaderParameter([JSMarshalAs<JSType.Any>] object shader, int pname);
	internal static bool GetShaderParameter([JSMarshalAs<JSType.Any>] object shader, GLParameterName pname)
		=> getShaderParameter(shader, (int)pname);

	[JSImport("WebGL.GetShaderSource", ImportString)]
	internal static partial string GetShaderSource([JSMarshalAs<JSType.Any>] object shader);

	[JSImport("WebGL.GetUniformLocation", ImportString)]
	[return: JSMarshalAs<JSType.Any>]
	internal static partial object GetUniformLocation([JSMarshalAs<JSType.Any>] object program, string name);

	[JSImport("WebGL.LinkProgram", ImportString)]
	internal static partial void LinkProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.Scissor", ImportString)]
	internal static partial void Scissor(int x, int y, int width, int height);

	[JSImport("WebGL.ShaderSource", ImportString)]
	internal static partial void ShaderSource([JSMarshalAs<JSType.Any>] object shader, string source);
	[JSImport("WebGL.TexImage2D", ImportString)]
	private static partial void texImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, [JSMarshalAs<JSType.MemoryView>] Span<byte> source);
	internal static void TexImage2D(GLTextureType target, int level, GLColorFormat internalformat, int width, int height, int border, GLColorFormat format, GLDataType type, Span<byte> source)
	=> texImage2D((int)target, level, (int)internalformat, width, height, border, (int)format, (int)type, source);

	[JSImport("WebGL.TexParameteri", ImportString)]
	private static partial void texParameteri(int target, int param, int pname);
	internal static void TexParameteri(GLTextureType target, GLTextureParameter param, int pname)
		=> texParameteri((int)target, (int)param, pname);

	[JSImport("WebGL.Uniform1i", ImportString)]
	internal static partial void Uniform1i([JSMarshalAs<JSType.Any>] object location, int value);

	[JSImport("WebGL.Uniform1iv", ImportString)]
	internal static partial void Uniform1iv([JSMarshalAs<JSType.Any>] object location, [JSMarshalAs<JSType.Array<JSType.Number>>] int[] value);

	[JSImport("WebGL.Uniform4f", ImportString)]
	internal static partial void Uniform4f([JSMarshalAs<JSType.Any>] object location, float v1, float v2, float v3, float v4);

	internal static void UniformColor([JSMarshalAs<JSType.Any>] object location, Color color)
		=> Uniform4f(location, color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);

	[JSImport("WebGL.UniformMatrix4fv", ImportString)]
	internal static partial void UniformMatrix4fv([JSMarshalAs<JSType.Any>] object location, bool transpose, [JSMarshalAs<JSType.Array<JSType.Number>>] double[] value);

	[JSImport("WebGL.UseProgram", ImportString)]
	internal static partial void UseProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.ValidateProgram", ImportString)]
	internal static partial void ValidateProgram([JSMarshalAs<JSType.Any>] object program);

	[JSImport("WebGL.VertexAttribPointer", ImportString)]
	private static partial void vertexAttribPointer(int index, int size, int type, bool normalized, int stride, int offset);
	internal static void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
		=> vertexAttribPointer(index, size, (int)type, normalized, stride, offset);

	[JSImport("WebGL.Viewport", ImportString)]
	internal static partial void Viewport(int x, int y, int width, int height);
}
