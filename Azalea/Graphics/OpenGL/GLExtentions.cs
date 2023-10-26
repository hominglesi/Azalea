using Azalea.Graphics.OpenGL.Enums;
using System;

namespace Azalea.Graphics.OpenGL;
public static class GLExtentions
{
	public static GLDataType GlDataTypeFromType(Type type)
	{
		return type.Name switch
		{
			nameof(SByte) => GLDataType.Byte,
			nameof(Byte) => GLDataType.UnsignedByte,
			nameof(Int16) => GLDataType.Short,
			nameof(UInt16) => GLDataType.UnsignedShort,
			nameof(Int32) => GLDataType.Int,
			nameof(UInt32) => GLDataType.UnsignedInt,
			nameof(Single) => GLDataType.Float,
			_ => throw new InvalidOperationException("Provided type does not have a valid OpenGL counterpart."),
		};
	}

	public static int SizeFromGLDataType(GLDataType type)
	{
		return type switch
		{
			GLDataType.Byte => 1,
			GLDataType.UnsignedByte => 1,
			GLDataType.Short => 2,
			GLDataType.UnsignedShort => 2,
			GLDataType.Int => 4,
			GLDataType.UnsignedInt => 4,
			GLDataType.Float => 4,
			_ => throw new InvalidOperationException("Provided type is not valid."),
		};
	}
}
