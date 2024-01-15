using System.Runtime.InteropServices;
using PFDFlags = Azalea.Platform.Windows.PixelFormatDescriptorFlags;

namespace Azalea.Platform.Windows;
[StructLayout(LayoutKind.Sequential)]
public struct PixelFormatDescriptor
{
	private uint size;
	private uint version;
	private int flags;
	byte pixelType;
	byte colorBits;
	byte redBits = 0;
	byte redShift = 0;
	byte greenBits = 0;
	byte greenShift = 0;
	byte blueBits = 0;
	byte blueShift = 0;
	byte alphaBits = 0;
	byte alphaShift = 0;
	byte accumBits = 0;
	byte accumRedBits = 0;
	byte accumGreenBits = 0;
	byte accumBlueBits = 0;
	byte accumAlphaBits = 0;
	byte depthBits = 24;
	byte stencilBits = 8;
	byte auxBuffers = 0;
	byte layerType = 0;
	byte reserved = 0;
	int layerMask = 0;
	int visibleMask = 0;
	int damageMask = 0;

	public PixelFormatDescriptor()
	{
		size = (uint)Marshal.SizeOf<PixelFormatDescriptor>();
		version = 1;
		flags = (int)(PFDFlags.DrawToWindow | PFDFlags.SupportOpenGL | PFDFlags.DoubleBuffer);
		pixelType = 0;
		colorBits = 32;

	}
}
