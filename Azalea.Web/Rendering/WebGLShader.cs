using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Web.Rendering;

public class WebGLShader : UnmanagedObject<object>
{
	protected override object CreateObject() => WebGL.CreateProgram();

	public WebGLShader()
	{
		var vertexShader = compileShader(GLShaderType.Vertex, _vertexShaderCode);
		var fragmentShader = compileShader(GLShaderType.Fragment, _fragmentShaderCode);

		WebGL.AttachShader(Handle, vertexShader);
		WebGL.AttachShader(Handle, fragmentShader);

		WebGL.LinkProgram(Handle);
		WebGL.ValidateProgram(Handle);

		if (WebGL.GetProgramParameter(Handle, GLParameterName.LinkStatus) == false)
			Console.WriteLine("Link Error: " + WebGL.GetProgramInfoLog(Handle));

		WebGL.DeleteShader(vertexShader);
		WebGL.DeleteShader(fragmentShader);
	}

	private static object compileShader(GLShaderType type, string source)
	{
		var id = WebGL.CreateShader(type);
		WebGL.ShaderSource(id, source);
		WebGL.CompileShader(id);

		if (WebGL.GetShaderParameter(id, GLParameterName.CompileStatus) == false)
			Console.WriteLine("Shader compile error: " + WebGL.GetShaderInfoLog(id));

		return id;
	}
	public void Bind() => WebGL.UseProgram(Handle);

	public void SetUniform(string name, int i)
	{
		Bind();
		WebGL.Uniform1i(getUniformLocation(name), i);
	}

	public void SetUniform(string name, int[] array)
	{
		Bind();
		WebGL.Uniform1iv(getUniformLocation(name), array);
	}

	public void SetUniform(string name, float f0, float f1, float f2, float f3)
	{
		Bind();
		WebGL.Uniform4f(getUniformLocation(name), f0, f1, f2, f3);
	}

	public void SetUniform(string name, Color color)
	{
		Bind();
		WebGL.UniformColor(getUniformLocation(name), color);
	}

	public void SetUniform(string name, Matrix4x4 matrix)
	{
		Bind();
		WebGL.UniformMatrix4fv(getUniformLocation(name), false, [
			matrix.M11, matrix.M12, matrix.M13, matrix.M14,
			matrix.M21, matrix.M22, matrix.M23, matrix.M24,
			matrix.M31, matrix.M32, matrix.M33, matrix.M34,
			matrix.M41, matrix.M42, matrix.M43, matrix.M44,
		]);
	}

	private Dictionary<string, object> _uniformLocations = new();

	private object getUniformLocation(string name)
	{
		if (_uniformLocations.TryGetValue(name, out object? value)) return value;

		Bind();
		var location = WebGL.GetUniformLocation(Handle, name)
			?? throw new Exception($"Uniform with name '{name}' was not found.");

		_uniformLocations.Add(name, location);
		return location;
	}

	protected override void OnDispose() => WebGL.DeleteProgram(Handle);

	private const string _vertexShaderCode = """
		#version 300 es
		precision highp float;

		in vec2 vPos;
		in vec4 vCol;
		in vec2 vTex;

		uniform mat4 u_Projection;

		out vec4 oCol;
		out vec2 oTex;

		void main(){
			gl_Position = u_Projection * vec4(vPos.x, vPos.y, 1.0, 1.0);
			oCol = vCol;
			oTex = vTex;
		}
	""";

	private const string _fragmentShaderCode = """
		#version 300 es
		precision mediump float;

		in vec4 oCol;
		in vec2 oTex;

		uniform sampler2D u_Texture;

		out vec4 FragColor;

		void main(){
			FragColor = texture(u_Texture, oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
		}
	""";
}
