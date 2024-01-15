namespace Azalea.Platform.Windows;

//Ported from https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-pixelformatdescriptor
internal enum PixelFormatDescriptorFlags
{
	/// <summary> The buffer can draw to a window or device surface. </summary>
	DrawToWindow = 0x00000004,
	/// <summary>
	/// The buffer can draw to a memory bitmap.
	/// </summary>
	DrawToBitmap = 0x00000008,
	/// <summary>
	/// The buffer supports GDI drawing.
	/// This flag and <see cref="DoubleBuffer"/> are mutually exclusive in the current generic implementation.
	/// </summary>
	SupportGDI = 0x00000010,
	/// <summary> The buffer supports OpenGL drawing. </summary>
	SupportOpenGL = 0x00000020,
	/// <summary>
	/// The pixel format is supported by the GDI software implementation,
	/// which is also known as the generic implementation.
	/// If this bit is clear, the pixel format is supported by a device driver or hardware.
	/// </summary>
	GenericAccelerated = 0x00001000,
	/// <summary>
	/// The buffer uses RGBA pixels on a palette-managed device.
	/// A logical palette is required to achieve the best results for this pixel type.
	/// Colors in the palette should be specified according to the values of the
	/// cRedBits, cRedShift, cGreenBits, cGreenShift, cBluebits, and cBlueShift members.
	/// The palette should be created and realized in the device context
	/// before calling <see cref="Graphics.OpenGL.GL.MakeCurrent"/>.
	/// </summary>
	NeedPallete = 0x00000080,
	/// <summary>
	/// Defined in the pixel format descriptors of hardware that supports one
	/// hardware palette in 256-color mode only. For such systems to use hardware acceleration,
	/// the hardware palette must be in a fixed order (for example, 3-3-2) when in RGBA mode
	/// or must match the logical palette when in color-index mode.When this flag is set,
	/// you must call SetSystemPaletteUse in your program to force a one-to-one mapping of
	/// the logical palette and the system palette. If your OpenGL hardware supports
	/// multiple hardware palettes and the device driver can allocate spare hardware palettes
	/// for OpenGL, this flag is typically clear. This flag is not set in the generic pixel formats.
	/// </summary>
	NeedSystemPallete = 0x00000100,
	/// <summary>
	/// The buffer is double-buffered.
	/// This flag and <see cref="SupportGDI"/> are mutually exclusive in the current generic implementation.
	/// </summary>
	DoubleBuffer = 0x00000001,
	/// <summary> The buffer is stereoscopic. This flag is not supported in the current generic implementation. </summary>
	Stereo = 0x00000002,
	/// <summary>
	/// Indicates whether a device can swap individual layer planes with pixel formats that include double-buffered overlay
	/// or underlay planes. Otherwise all layer planes are swapped together as a group.
	/// When this flag is set, wglSwapLayerBuffers is supported.
	/// </summary>
	SwapLayerBuffers = 0x00000800,
}
