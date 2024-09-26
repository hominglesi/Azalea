using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Web.Rendering;

public class WebGLShader : Disposable
{
	public object Handle { get; init; }

	public WebGLShader(string vertexShaderSource, string fragmentShaderSource)
	{
		Handle = WebGL.CreateProgram();
		var vertexShader = compileShader(GLShaderType.Vertex, vertexShaderSource);
		var fragmentShader = compileShader(GLShaderType.Fragment, fragmentShaderSource);

		WebGL.AttachShader(Handle, vertexShader);
		WebGL.AttachShader(Handle, fragmentShader);

		WebGL.LinkProgram(Handle);
		WebGL.ValidateProgram(Handle);

		if (WebGL.GetProgramParameter(Handle, GLParameterName.LinkStatus) == false)
		{
			Console.WriteLine("Link Error: " + WebGL.GetProgramInfoLog(Handle));
		}

		WebGL.DeleteShader(vertexShader);
		WebGL.DeleteShader(fragmentShader);
	}

	private static object compileShader(GLShaderType type, string source)
	{
		var id = WebGL.CreateShader(type);
		WebGL.ShaderSource(id, source);
		WebGL.CompileShader(id);

		if (WebGL.GetShaderParameter(id, GLParameterName.CompileStatus) == false)
		{
			Console.WriteLine("Shader compile error: " + WebGL.GetShaderInfoLog(id));
		}

		/*
		Error Checking
		int result;
		GL.GetShaderiv(id, GLParameterName.CompileStatus, &result);
		if (result == 0)
		{

			int length;
			GL.GetShaderiv(id, GLParameterName.InfoLogLength, &length);
			Console.WriteLine(length);
			
			var str = new StringBuilder(length);
			for(int i = 0; i < length; i++)
			{
				str.Append((char)Marshal.ReadByte(result, i));
			}	
			Console.WriteLine("THERE IS AN ERROR!!!!");
			GL.DeleteShader(id);
			return 0;
		}
		*/

		return id;
	}

	// Call Implementation
	public void Bind() => WebGL.UseProgram(Handle);
	public void Unbind() => throw new NotImplementedException();

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
		if (_uniformLocations.ContainsKey(name)) return _uniformLocations[name];

		Bind();
		var location = WebGL.GetUniformLocation(Handle, name);
		//if (location == -1) Console.WriteLine($"Uniform with name '{name}' was not found.");

		_uniformLocations.Add(name, location);
		return location;
	}

	protected override void OnDispose()
	{
		WebGL.DeleteProgram(Handle);
	}
}
