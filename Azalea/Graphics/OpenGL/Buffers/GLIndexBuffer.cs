using Azalea.Graphics.OpenGL.Enums;

namespace Azalea.Graphics.OpenGL.Buffers;
public class GLIndexBuffer : GLBuffer
{
	public GLIndexBuffer()
		: base(GLBufferType.ElementArray) { }
}
