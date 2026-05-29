using System.Runtime.InteropServices;

namespace Azalea.Native.OpenGL;

// Constants and Functions defined in OpenGL 1.0
// Defined here: https://registry.khronos.org/OpenGL/api/GLES/1.0/gl.h
// To simplify method calls GL_ and gl prefixes are stripped
public static partial class GL
{
	private const string OpenGLPath = "opengl32.dll";

	/* Extensions */
	public const int OES_VERSION_1_0 = 1;
	public const int OES_read_format = 1;
	public const int OES_compressed_paletted_texture = 1;

	/* ClearBufferMask */
	public const int DEPTH_BUFFER_BIT = 0x00000100;
	public const int STENCIL_BUFFER_BIT = 0x00000400;
	public const int COLOR_BUFFER_BIT = 0x00004000;

	/* Boolean */
	public const int FALSE = 0;
	public const int TRUE = 1;

	/* BeginMode */
	public const int POINTS = 0x0000;
	public const int LINES = 0x0001;
	public const int LINE_LOOP = 0x0002;
	public const int LINE_STRIP = 0x0003;
	public const int TRIANGLES = 0x0004;
	public const int TRIANGLE_STRIP = 0x0005;
	public const int TRIANGLE_FAN = 0x0006;

	/* AlphaFunction */
	public const int NEVER = 0x0200;
	public const int LESS = 0x0201;
	public const int EQUAL = 0x0202;
	public const int LEQUAL = 0x0203;
	public const int GREATER = 0x0204;
	public const int NOTEQUAL = 0x0205;
	public const int GEQUAL = 0x0206;
	public const int ALWAYS = 0x0207;

	/* BlendingFactorDest */
	public const int ZERO = 0;
	public const int ONE = 1;
	public const int SRC_COLOR = 0x0300;
	public const int ONE_MINUS_SRC_COLOR = 0x0301;
	public const int SRC_ALPHA = 0x0302;
	public const int ONE_MINUS_SRC_ALPHA = 0x0303;
	public const int DST_ALPHA = 0x0304;
	public const int ONE_MINUS_DST_ALPHA = 0x0305;

	/* BlendingFactorSrc */
	// ZERO
	// ONE
	public const int DST_COLOR = 0x0306;
	public const int ONE_MINUS_DST_COLOR = 0x0307;
	public const int SRC_ALPHA_SATURATE = 0x0308;
	// SRC_ALPHA
	// ONE_MINUS_SRC_ALPHA
	// DST_ALPHA
	// ONE_MINUS_DST_ALPHA

	/* ColorMaterialFace */
	// FRONT_AND_BACK

	/* ColorMaterialParameter */
	// AMBIENT_AND_DIFFUSE

	/* ColorPointerType */
	// UNSIGNED_BYTE
	// GL_FLOAT
	// GL_FIXED

	/* CullFaceMode */
	public const int FRONT = 0x0404;
	public const int BACK = 0x0405;
	public const int FRONT_AND_BACK = 0x0408;

	/* DepthFunction */
	// NEVER
	// LESS
	// EQUAL
	// LEQUAL
	// GREATER
	// NOTEQUAL
	// GEQUAL
	// ALWAYS

	/* EnableCap */
	public const int FOG = 0x0B60;
	public const int LIGHTING = 0x0B50;
	public const int TEXTURE_2D = 0x0DE1;
	public const int CULL_FACE = 0x0B44;
	public const int ALPHA_TEST = 0x0BC0;
	public const int BLEND = 0x0BE2;
	public const int COLOR_LOGIC_OP = 0x0BF2;
	public const int DITHER = 0x0BD0;
	public const int STENCIL_TEST = 0x0B90;
	public const int DEPTH_TEST = 0x0B71;
	// GL_LIGHT0
	// GL_LIGHT1
	// GL_LIGHT2
	// GL_LIGHT3
	// GL_LIGHT4
	// GL_LIGHT5
	// GL_LIGHT6
	// GL_LIGHT7
	public const int POINT_SMOOTH = 0x0B10;
	public const int LINE_SMOOTH = 0x0B20;
	public const int SCISSOR_TEST = 0x0C11;
	public const int COLOR_MATERIAL = 0x0B57;
	public const int NORMALIZE = 0x0BA1;
	public const int RESCALE_NORMAL = 0x803A;
	public const int POLYGON_OFFSET_FILL = 0x8037;
	public const int VERTEX_ARRAY = 0x8074;
	public const int NORMAL_ARRAY = 0x8075;
	public const int COLOR_ARRAY = 0x8076;
	public const int TEXTURE_COORD_ARRAY = 0x8078;
	public const int MULTISAMPLE = 0x809D;
	public const int SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
	public const int SAMPLE_ALPHA_TO_ONE = 0x809F;
	public const int SAMPLE_COVERAGE = 0x80A0;

	/* ErrorCode */
	public const int NO_ERROR = 0;
	public const int INVALID_ENUM = 0x0500;
	public const int INVALID_VALUE = 0x0501;
	public const int INVALID_OPERATION = 0x0502;
	public const int STACK_OVERFLOW = 0x0503;
	public const int STACK_UNDERFLOW = 0x0504;
	public const int OUT_OF_MEMORY = 0x0505;

	/* FogMode */
	// GL_LINEAR
	public const int EXP = 0x0800;
	public const int EXP2 = 0x0801;

	/* FogParameter */
	public const int FOG_DENSITY = 0x0B62;
	public const int FOG_START = 0x0B63;
	public const int FOG_END = 0x0B64;
	public const int FOG_MODE = 0x0B65;
	public const int FOG_COLOR = 0x0B66;

	/* FrontFaceDirection */
	public const int CW = 0x0900;
	public const int CCW = 0x0901;

	/* GetPName */
	public const int SMOOTH_POINT_SIZE_RANGE = 0x0B12;
	public const int SMOOTH_LINE_WIDTH_RANGE = 0x0B22;
	public const int ALIASED_POINT_SIZE_RANGE = 0x846D;
	public const int ALIASED_LINE_WIDTH_RANGE = 0x846E;
	public const int IMPLEMENTATION_COLOR_READ_TYPE_OES = 0x8B9A;
	public const int IMPLEMENTATION_COLOR_READ_FORMAT_OES = 0x8B9B;
	public const int MAX_LIGHTS = 0x0D31;
	public const int MAX_TEXTURE_SIZE = 0x0D33;
	public const int MAX_MODELVIEW_STACK_DEPTH = 0x0D36;
	public const int MAX_PROJECTION_STACK_DEPTH = 0x0D38;
	public const int MAX_TEXTURE_STACK_DEPTH = 0x0D39;
	public const int MAX_VIEWPORT_DIMS = 0x0D3A;
	public const int MAX_ELEMENTS_VERTICES = 0x80E8;
	public const int MAX_ELEMENTS_INDICES = 0x80E9;
	public const int MAX_TEXTURE_UNITS = 0x84E2;
	public const int NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2;
	public const int COMPRESSED_TEXTURE_FORMATS = 0x86A3;
	public const int SUBPIXEL_BITS = 0x0D50;
	public const int RED_BITS = 0x0D52;
	public const int GREEN_BITS = 0x0D53;
	public const int BLUE_BITS = 0x0D54;
	public const int ALPHA_BITS = 0x0D55;
	public const int DEPTH_BITS = 0x0D56;
	public const int STENCIL_BITS = 0x0D57;

	/* HintMode */
	public const int DONT_CARE = 0x1100;
	public const int FASTEST = 0x1101;
	public const int NICEST = 0x1102;

	/* HintTarget */
	public const int PERSPECTIVE_CORRECTION_HINT = 0x0C50;
	public const int POINT_SMOOTH_HINT = 0x0C51;
	public const int LINE_SMOOTH_HINT = 0x0C52;
	public const int POLYGON_SMOOTH_HINT = 0x0C53;
	public const int FOG_HINT = 0x0C54;

	/* LightModelParameter */
	public const int LIGHT_MODEL_AMBIENT = 0x0B53;
	public const int LIGHT_MODEL_TWO_SIDE = 0x0B52;

	/* LightParameter */
	public const int AMBIENT = 0x1200;
	public const int DIFFUSE = 0x1201;
	public const int SPECULAR = 0x1202;
	public const int POSITION = 0x1203;
	public const int SPOT_DIRECTION = 0x1204;
	public const int SPOT_EXPONENT = 0x1205;
	public const int SPOT_CUTOFF = 0x1206;
	public const int CONSTANT_ATTENUATION = 0x1207;
	public const int LINEAR_ATTENUATION = 0x1208;
	public const int QUADRATIC_ATTENUATION = 0x1209;

	/* DataType */
	public const int BYTE = 0x1400;
	public const int UNSIGNED_BYTE = 0x1401;
	public const int SHORT = 0x1402;
	public const int UNSIGNED_SHORT = 0x1403;
	public const int FLOAT = 0x1406;
	public const int FIXED = 0x140C;

	/* LogicOp */
	public const int CLEAR = 0x1500;
	public const int AND = 0x1501;
	public const int AND_REVERSE = 0x1502;
	public const int COPY = 0x1503;
	public const int AND_INVERTED = 0x1504;
	public const int NOOP = 0x1505;
	public const int XOR = 0x1506;
	public const int OR = 0x1507;
	public const int NOR = 0x1508;
	public const int EQUIV = 0x1509;
	public const int INVERT = 0x150A;
	public const int OR_REVERSE = 0x150B;
	public const int COPY_INVERTED = 0x150C;
	public const int OR_INVERTED = 0x150D;
	public const int NAND = 0x150E;
	public const int SET = 0x150F;

	/* MaterialFace */
	// FRONT_AND_BACK

	/* MaterialParameter */
	public const int EMISSION = 0x1600;
	public const int SHININESS = 0x1601;
	public const int AMBIENT_AND_DIFFUSE = 0x1602;
	// AMBIENT
	// DIFFUSE
	// SPECULAR

	/* MatrixMode */
	public const int MODELVIEW = 0x1700;
	public const int PROJECTION = 0x1701;
	public const int TEXTURE = 0x1702;

	/* NormalPointerType */
	// BYTE
	// SHORT
	// FLOAT
	// FIXED

	/* PixelFormat */
	public const int ALPHA = 0x1906;
	public const int RGB = 0x1907;
	public const int RGBA = 0x1908;
	public const int LUMINANCE = 0x1909;
	public const int LUMINANCE_ALPHA = 0x190A;

	/* PixelStoreParameter */
	public const int UNPACK_ALIGNMENT = 0x0CF5;
	public const int PACK_ALIGNMENT = 0x0D05;

	/* PixelType */
	// UNSIGNED_BYTE
	public const int UNSIGNED_SHORT_4_4_4_4 = 0x8033;
	public const int UNSIGNED_SHORT_5_5_5_1 = 0x8034;
	public const int UNSIGNED_SHORT_5_6_5 = 0x8363;

	/* ShadingModel */
	public const int FLAT = 0x1D00;
	public const int SMOOTH = 0x1D01;

	/* StencilFunction */
	// NEVER
	// LESS
	// EQUAL
	// LEQUAL
	// GREATER
	// NOTEQUAL
	// GEQUAL
	// ALWAYS

	/* StencilOp */
	// GL_ZERO
	public const int KEEP = 0x1E00;
	public const int REPLACE = 0x1E01;
	public const int INCR = 0x1E02;
	public const int DECR = 0x1E03;
	// GL_INVERT

	/* StringName */
	public const int VENDOR = 0x1F00;
	public const int RENDERER = 0x1F01;
	public const int VERSION = 0x1F02;
	public const int EXTENSIONS = 0x1F03;

	/* TexCoordPointerType */
	// SHORT
	// FLOAT
	// FIXED
	// BYTE

	/* TextureEnvMode */
	public const int MODULATE = 0x2100;
	public const int DECAL = 0x2101;
	// BLEND
	public const int ADD = 0x0104;
	// REPLACE

	/* TextureEnvParameter */
	public const int TEXTURE_ENV_MODE = 0x2200;
	public const int TEXTURE_ENV_COLOR = 0x2201;

	/* TextureEnvTarget */
	public const int TEXTURE_ENV = 0x2300;

	/* TextureMagFilter */
	public const int NEAREST = 0x2600;
	public const int LINEAR = 0x2601;

	/* TextureMinFilter */
	// NEAREST
	// LINEAR
	public const int NEAREST_MIPMAP_NEAREST = 0x2700;
	public const int LINEAR_MIPMAP_NEAREST = 0x2701;
	public const int NEAREST_MIPMAP_LINEAR = 0x2702;
	public const int LINEAR_MIPMAP_LINEAR = 0x2703;

	/* TextureParameterName */
	public const int TEXTURE_MAG_FILTER = 0x2800;
	public const int TEXTURE_MIN_FILTER = 0x2801;
	public const int TEXTURE_WRAP_S = 0x2802;
	public const int TEXTURE_WRAP_T = 0x2803;

	/* TextureTarget */
	// TEXTURE_2D

	/* TextureUnit */
	public const int TEXTURE0 = 0x84C0;
	public const int TEXTURE1 = 0x84C1;
	public const int TEXTURE2 = 0x84C2;
	public const int TEXTURE3 = 0x84C3;
	public const int TEXTURE4 = 0x84C4;
	public const int TEXTURE5 = 0x84C5;
	public const int TEXTURE6 = 0x84C6;
	public const int TEXTURE7 = 0x84C7;
	public const int TEXTURE8 = 0x84C8;
	public const int TEXTURE9 = 0x84C9;
	public const int TEXTURE10 = 0x84CA;
	public const int TEXTURE11 = 0x84CB;
	public const int TEXTURE12 = 0x84CC;
	public const int TEXTURE13 = 0x84CD;
	public const int TEXTURE14 = 0x84CE;
	public const int TEXTURE15 = 0x84CF;
	public const int TEXTURE16 = 0x84D0;
	public const int TEXTURE17 = 0x84D1;
	public const int TEXTURE18 = 0x84D2;
	public const int TEXTURE19 = 0x84D3;
	public const int TEXTURE20 = 0x84D4;
	public const int TEXTURE21 = 0x84D5;
	public const int TEXTURE22 = 0x84D6;
	public const int TEXTURE23 = 0x84D7;
	public const int TEXTURE24 = 0x84D8;
	public const int TEXTURE25 = 0x84D9;
	public const int TEXTURE26 = 0x84DA;
	public const int TEXTURE27 = 0x84DB;
	public const int TEXTURE28 = 0x84DC;
	public const int TEXTURE29 = 0x84DD;
	public const int TEXTURE30 = 0x84DE;
	public const int TEXTURE31 = 0x84DF;

	/* TextureWrapMode */
	public const int REPEAT = 0x2901;
	public const int CLAMP_TO_EDGE = 0x812F;

	/* PixelInternalFormat */
	public const int PALETTE4_RGB8_OES = 0x8B90;
	public const int PALETTE4_RGBA8_OES = 0x8B91;
	public const int PALETTE4_R5_G6_B5_OES = 0x8B92;
	public const int PALETTE4_RGBA4_OES = 0x8B93;
	public const int PALETTE4_RGB5_A1_OES = 0x8B94;
	public const int PALETTE8_RGB8_OES = 0x8B95;
	public const int PALETTE8_RGBA8_OES = 0x8B96;
	public const int PALETTE8_R5_G6_B5_OES = 0x8B97;
	public const int PALETTE8_RGBA4_OES = 0x8B98;
	public const int PALETTE8_RGB5_A1_OES = 0x8B99;

	/* VertexPointerType */
	// SHORT
	// FLOAT
	// FIXED
	// BYTE

	/* LightName */
	public const int LIGHT0 = 0x4000;
	public const int LIGHT1 = 0x4001;
	public const int LIGHT2 = 0x4002;
	public const int LIGHT3 = 0x4003;
	public const int LIGHT4 = 0x4004;
	public const int LIGHT5 = 0x4005;
	public const int LIGHT6 = 0x4006;
	public const int LIGHT7 = 0x4007;

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindTexture.xhtml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath, EntryPoint = "glBindTexture")]
	public static partial void BindTexture(int target, uint texture);

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/es2.0/xhtml/glClear.xml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath, EntryPoint = "glClear")]
	public static partial void Clear(int mask);

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/es2.0/xhtml/glClearColor.xml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath, EntryPoint = "glClearColor")]
	public static partial void ClearColor(float red, float green, float blue, float alpha);

	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glGenTextures.xhtml">Official Documentation</see></summary>
	[LibraryImport(OpenGLPath, EntryPoint = "glGenTextures")]
	public static partial void GenTextures(int n, ref uint textures);

	[LibraryImport(OpenGLPath, EntryPoint = "glTexImage2D")]
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexImage2D.xhtml">Official Documentation</see></summary>
	public static partial void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, in byte pixels);

	[LibraryImport(OpenGLPath, EntryPoint = "glTexImage2D")]
	/// <summary><see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glTexImage2D.xhtml">Official Documentation</see></summary>
	public static partial void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, IntPtr pixels);
}
