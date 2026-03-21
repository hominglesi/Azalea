using System.Runtime.InteropServices;
using PFDFlags = Azalea.Platform.Windows.PixelFormatDescriptorFlags;

namespace Azalea.Platform.Windows;
[StructLayout(LayoutKind.Sequential)]
public struct PixelFormatDescriptor
{
	private uint size;
	private uint version;
	private int flags;
	private byte pixelType;
	private byte colorBits;
	private byte redBits = 0;
	private byte redShift = 0;
	private byte greenBits = 0;
	private byte greenShift = 0;
	private byte blueBits = 0;
	private byte blueShift = 0;
	private byte alphaBits = 8;
	private byte alphaShift = 0;
	private byte accumBits = 0;
	private byte accumRedBits = 0;
	private byte accumGreenBits = 0;
	private byte accumBlueBits = 0;
	private byte accumAlphaBits = 0;
	private byte depthBits = 24;
	private byte stencilBits = 8;
	private byte auxBuffers = 0;
	private byte layerType = 0; // PFD_MAIN_PLANE
	private byte reserved = 0;
	private int layerMask = 0;
	private int visibleMask = 0;
	private int damageMask = 0;

	public PixelFormatDescriptor()
	{
		size = (uint)Marshal.SizeOf<PixelFormatDescriptor>();
		version = 1;
		flags = (int)(PFDFlags.DrawToWindow | PFDFlags.SupportOpenGL | PFDFlags.DoubleBuffer);
		pixelType = 0;
		colorBits = 32;
	}
}
