using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.OpenGL;
public class GLShader : IDisposable
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

	public void Bind() => GL.UseProgram(_handle);
	public void Unbind() => GL.UseProgram(0);

	public void SetUniform(string name, int i)
	{
		Bind();
		GL.Uniform1i(getUniformLocation(name), i);
	}

	public void SetUniform(string name, float f0, float f1, float f2, float f3)
	{
		Bind();
		GL.Uniform4f(getUniformLocation(name), f0, f1, f2, f3);
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

	#region Disposing
	private bool _disposed;
	~GLShader() => Dispose(false);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed) return;

		if (disposing)
		{
			GL.DeleteProgram(_handle);
		}

		_disposed = true;
	}
	#endregion
}
