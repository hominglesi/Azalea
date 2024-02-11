using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Shaders;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.OpenGL;
public class GLShader : Disposable, IShader
{
	private uint _handle;

	public GLShader(string vertexShaderSource, string fragmentShaderSource)
	{
		_handle = GL.CreateProgram();
		var vertexShader = compileShader(GLShaderType.Vertex, vertexShaderSource);
		var fragmentShader = compileShader(GLShaderType.Fragment, fragmentShaderSource);

		GL.AttachShader(_handle, vertexShader);
		GL.AttachShader(_handle, fragmentShader);

		GL.LinkProgram(_handle);
		GL.ValidateProgram(_handle);

		GL.DeleteShader(vertexShader);
		GL.DeleteShader(fragmentShader);
	}

	//TODO: move this to separate
	private static uint compileShader(GLShaderType type, string source)
	{
		var id = GL.CreateShader(type);
		GL.ShaderSource(id, source);
		GL.CompileShader(id);

		int result = GL.GetShaderiv(id, GLParameterName.CompileStatus);
		if (result == 0)
		{
			var info = GL.GetShaderInfoLog(id);
			Console.WriteLine(info);

			GL.DeleteShader(id);
			return 0;
		}

		return id;
	}

	public void Bind() => GL.UseProgram(_handle);
	public void Unbind() => GL.UseProgram(0);

	public void SetUniform(string name, int i)
	{
		Bind();
		GL.Uniform1i(getUniformLocation(name), i);
	}

	public void SetUniform(string name, int[] array)
	{
		Bind();
		GL.Uniform1iv(getUniformLocation(name), array);
	}

	public void SetUniform(string name, Vector4 vec4)
	{
		Bind();
		GL.Uniform4f(getUniformLocation(name), vec4.X, vec4.Y, vec4.Z, vec4.W);
	}

	public void SetUniform(string name, Color color)
	{
		Bind();
		GL.UniformColor(getUniformLocation(name), color);
	}

	public void SetUniform(string name, Matrix4x4 matrix)
	{
		Bind();
		GL.UniformMatrix4(getUniformLocation(name), 1, false, matrix);
	}

	private Dictionary<string, int> _uniformLocations = new();

	private int getUniformLocation(string name)
	{
		if (_uniformLocations.ContainsKey(name)) return _uniformLocations[name];

		Bind();
		var location = GL.GetUniformLocation(_handle, name);
		if (location == -1) Console.WriteLine($"Uniform with name '{name}' was not found.");

		_uniformLocations.Add(name, location);
		return location;
	}

	protected override void OnDispose()
	{
		GL.DeleteProgram(_handle);
	}
}
