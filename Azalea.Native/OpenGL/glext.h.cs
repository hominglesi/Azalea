namespace Azalea.Native.OpenGL;

// Extention Constants and Functions defined in new versions of OpenGL
// Defined here: https://registry.khronos.org/OpenGL/api/GL/glext.h
// To simplify method calls GL_ and gl prefixes are stripped
public static partial class GL
{
	public const int READ_FRAMEBUFFER = 0x8CA8;
	public const int DRAW_FRAMEBUFFER = 0x8CA9;
	public const int READ_FRAMEBUFFER_BINDING = 0x8CAA;
	public const int RENDERBUFFER_SAMPLES = 0x8CAB;
	public const int FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 0x8CD0;
	public const int FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 0x8CD1;
	public const int FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 0x8CD2;
	public const int FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 0x8CD3;
	public const int FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER = 0x8CD4;
	public const int FRAMEBUFFER_COMPLETE = 0x8CD5;
	public const int FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6;
	public const int FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7;
	public const int FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 0x8CDB;
	public const int FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 0x8CDC;
	public const int FRAMEBUFFER_UNSUPPORTED = 0x8CDD;
	public const int MAX_COLOR_ATTACHMENTS = 0x8CDF;
	public const int COLOR_ATTACHMENT0 = 0x8CE0;
	public const int COLOR_ATTACHMENT1 = 0x8CE1;
	public const int COLOR_ATTACHMENT2 = 0x8CE2;
	public const int COLOR_ATTACHMENT3 = 0x8CE3;
	public const int COLOR_ATTACHMENT4 = 0x8CE4;
	public const int COLOR_ATTACHMENT5 = 0x8CE5;
	public const int COLOR_ATTACHMENT6 = 0x8CE6;
	public const int COLOR_ATTACHMENT7 = 0x8CE7;
	public const int COLOR_ATTACHMENT8 = 0x8CE8;
	public const int COLOR_ATTACHMENT9 = 0x8CE9;
	public const int COLOR_ATTACHMENT10 = 0x8CEA;
	public const int COLOR_ATTACHMENT11 = 0x8CEB;
	public const int COLOR_ATTACHMENT12 = 0x8CEC;
	public const int COLOR_ATTACHMENT13 = 0x8CED;
	public const int COLOR_ATTACHMENT14 = 0x8CEE;
	public const int COLOR_ATTACHMENT15 = 0x8CEF;
	public const int COLOR_ATTACHMENT16 = 0x8CF0;
	public const int COLOR_ATTACHMENT17 = 0x8CF1;
	public const int COLOR_ATTACHMENT18 = 0x8CF2;
	public const int COLOR_ATTACHMENT19 = 0x8CF3;
	public const int COLOR_ATTACHMENT20 = 0x8CF4;
	public const int COLOR_ATTACHMENT21 = 0x8CF5;
	public const int COLOR_ATTACHMENT22 = 0x8CF6;
	public const int COLOR_ATTACHMENT23 = 0x8CF7;
	public const int COLOR_ATTACHMENT24 = 0x8CF8;
	public const int COLOR_ATTACHMENT25 = 0x8CF9;
	public const int COLOR_ATTACHMENT26 = 0x8CFA;
	public const int COLOR_ATTACHMENT27 = 0x8CFB;
	public const int COLOR_ATTACHMENT28 = 0x8CFC;
	public const int COLOR_ATTACHMENT29 = 0x8CFD;
	public const int COLOR_ATTACHMENT30 = 0x8CFE;
	public const int COLOR_ATTACHMENT31 = 0x8CFF;
	public const int DEPTH_ATTACHMENT = 0x8D00;
	public const int STENCIL_ATTACHMENT = 0x8D20;
	public const int FRAMEBUFFER = 0x8D40;
}
