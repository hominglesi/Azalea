using Azalea.Graphics.OpenGL.Enums;
using System.Collections.Generic;

namespace Azalea.Graphics.OpenGL;
public class GLVertexBufferLayout
{
	private readonly List<GLVertexBufferElement> _elements = new();
	public int Stride { get; private set; }

	public void AddElement<T>(int count)
	{
		GLDataType type = GLExtentions.GlDataTypeFromType(typeof(T));
		_elements.Add(new GLVertexBufferElement(type, count, false));
		Stride += GLExtentions.SizeFromGLDataType(type) * count;
	}

	public IReadOnlyList<GLVertexBufferElement> Elements => _elements;
}

public readonly struct GLVertexBufferElement
{
	public readonly int Count;
	public readonly GLDataType Type;
	public readonly bool Normalized;

	public GLVertexBufferElement(GLDataType type, int count, bool normalized)
	{
		Type = type;
		Count = count;
		Normalized = normalized;
	}
}
