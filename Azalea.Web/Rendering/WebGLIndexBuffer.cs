using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Web.Rendering;

public class WebGLIndexBuffer : UnmanagedObject<object>
{
	protected override object CreateObject() => WebGL.CreateBuffer();

	public void Bind() => WebGL.BindBuffer(GLBufferType.ElementArray, Handle);

	public void SetData(Span<ushort> data, GLUsageHint hint)
	{
		Bind();
		WebGL.BufferData(GLBufferType.ElementArray, MemoryMarshal.AsBytes(data), hint);
	}

	protected override void OnDispose() => WebGL.DeleteBuffer(Handle);
}
