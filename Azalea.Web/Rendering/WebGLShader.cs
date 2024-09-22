using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Web.Rendering;

public class WebGLShader : Disposable
{
	private uint _handle;

	public WebGLShader(string vertexShaderSource, string fragmentShaderSource)
	{
		// Call Implementation
		// _handle = GL.CreateProgram();
		var vertexShader = compileShader(GLShaderType.Vertex, vertexShaderSource);
		var fragmentShader = compileShader(GLShaderType.Fragment, fragmentShaderSource);

		// Call Implementation
		// GL.AttachShader(_handle, vertexShader);
		// GL.AttachShader(_handle, fragmentShader);

		// GL.LinkProgram(_handle);
		// GL.ValidateProgram(_handle);

		// GL.DeleteShader(vertexShader);
		// GL.DeleteShader(fragmentShader);
	}

	//TODO: Create Implementation
	private static uint compileShader(GLShaderType type, string source)
	{
		//var id = GL.CreateShader(type);
		//GL.ShaderSource(id, source);
		//GL.CompileShader(id);

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

		return 0;//id;
	}

	// Call Implementation
	public void Bind() => throw new NotImplementedException();
	public void Unbind() => throw new NotImplementedException();

	public void SetUniform(string name, int i)
	{
		Bind();
		// Call Implementation
		// GL.Uniform1i(getUniformLocation(name), i);
	}

	public void SetUniform(string name, int[] array)
	{
		Bind();
		// Call Implementation
		// GL.Uniform1iv(getUniformLocation(name), array);
	}

	public void SetUniform(string name, float f0, float f1, float f2, float f3)
	{
		Bind();
		// Call Implementation
		// GL.Uniform4f(getUniformLocation(name), f0, f1, f2, f3);
	}

	public void SetUniform(string name, Color color)
	{
		Bind();
		// Call Implementation
		// GL.UniformColor(getUniformLocation(name), color);
	}

	public void SetUniform(string name, Matrix4x4 matrix)
	{
		Bind();
		// Call Implementation
		// GL.UniformMatrix4(getUniformLocation(name), 1, false, matrix);
	}

	private Dictionary<string, int> _uniformLocations = new();

	private int getUniformLocation(string name)
	{
		if (_uniformLocations.ContainsKey(name)) return _uniformLocations[name];

		Bind();
		// Call Implementation
		var location = 0; //GL.GetUniformLocation(_handle, name);
		if (location == -1) Console.WriteLine($"Uniform with name '{name}' was not found.");

		_uniformLocations.Add(name, location);
		return location;
	}

	protected override void OnDispose()
	{
		// Call Implementation
		// GL.DeleteProgram(_handle);
	}
}
