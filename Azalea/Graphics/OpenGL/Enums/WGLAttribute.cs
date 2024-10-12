namespace Azalea.Graphics.OpenGL.Enums;

// Mappings from https://registry.khronos.org/OpenGL/api/GL/wglext.h
internal enum WGLAttribute
{
	ContextCoreProfileBit = 0x00000001,
	DrawToWindow = 0x2001,
	Acceleration = 0x2003,
	SupportOpenGL = 0x2010,
	DoubleBuffer = 0x2011,
	PixelType = 0x2013,
	ColorBits = 0x2014,
	DepthBits = 0x2022,
	StencilBits = 0x2023,
	FullAcceleration = 0x2027,
	TypeRGBA = 0x202B,
	ContextMajorVersion = 0x2091,
	ContextMinorVersion = 0x2092,
	ContextProfileMask = 0x9126,
}
