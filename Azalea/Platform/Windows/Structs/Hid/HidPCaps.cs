using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct HidPCaps
{
	public readonly ushort Usage;
	public readonly ushort UsagePage;
	public readonly ushort InputReportByteLength;
	public readonly ushort OutputReportByteLength;
	public readonly ushort FeatureReportByteLength;

	private readonly ushort reserved0;
	private readonly ushort reserved1;
	private readonly ushort reserved2;
	private readonly ushort reserved3;
	private readonly ushort reserved4;
	private readonly ushort reserved5;
	private readonly ushort reserved6;
	private readonly ushort reserved7;
	private readonly ushort reserved8;
	private readonly ushort reserved9;
	private readonly ushort reserved10;
	private readonly ushort reserved11;
	private readonly ushort reserved12;
	private readonly ushort reserved13;
	private readonly ushort reserved14;
	private readonly ushort reserved15;
	private readonly ushort reserved16;

	public readonly ushort NumberLinkCollectionNodes;

	public readonly ushort NumberInputButtonCaps;
	public readonly ushort NumberInputValueCaps;
	public readonly ushort NumberInputDataIndices;

	public readonly ushort NumberOutputButtonCaps;
	public readonly ushort NumberOutputValueCaps;
	public readonly ushort NumberOutputDataIndices;

	public readonly ushort NumberFeatureButtonCaps;
	public readonly ushort NumberFeatureValueCaps;
	public readonly ushort NumberFeatureDataIndices;
}
