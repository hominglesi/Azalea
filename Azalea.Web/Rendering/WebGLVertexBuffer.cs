using Azalea.Graphics.OpenGL.Enums;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Web.Rendering;

public class WebGLVertexBuffer : UnmanagedObject<object>
{
	protected override object CreateObject() => WebGL.CreateBuffer();

	public void Bind() => WebGL.BindBuffer(GLBufferType.Array, Handle);

	public void SetData(Span<float> data, GLUsageHint hint)
	{
		Bind();
		WebGL.BufferData(GLBufferType.Array, MemoryMarshal.AsBytes(data), hint);
	}

	protected override void OnDispose() => WebGL.DeleteBuffer(Handle);
}
