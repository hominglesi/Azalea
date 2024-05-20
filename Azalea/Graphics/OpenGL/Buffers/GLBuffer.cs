using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;

namespace Azalea.Graphics.OpenGL.Buffers;
public class GLBuffer : Disposable
{
	public uint Handle { get; init; }
	protected GLBufferType Type { get; init; }

	public GLBuffer(GLBufferType type)
	{
		Type = type;
		Handle = CreateBuffer();
	}

	protected virtual uint CreateBuffer()
		=> GL.GenBuffer();

	protected virtual void DeleteBuffer()
		=> GL.DeleteBuffer(Handle);

	public virtual void Bind() => GL.BindBuffer(Type, Handle);
	public virtual void Unbind() => GL.BindBuffer(Type, Handle);

	public void BufferData(int size, IntPtr data, GLUsageHint hint)
	{
		Bind();
		GL.BufferData(Type, (IntPtr)size, data, hint);
	}

	public void BufferSubData(int offset, int size, IntPtr data)
	{
		Bind();
		GL.NamedBufferSubData(Handle, (IntPtr)offset, (IntPtr)size, data);
	}

	public void BufferData<T>(T[] data, GLUsageHint hint)
		where T : unmanaged
	{
		Bind();
		GL.BufferData(Type, data, hint);
	}

	public void BufferData<T>(T[] data, int length, GLUsageHint hint)
		where T : unmanaged
	{
		Bind();
		GL.BufferData(Type, data, length, hint);
	}

	protected override void OnDispose()
		=> DeleteBuffer();
}
