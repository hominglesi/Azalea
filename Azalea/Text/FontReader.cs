using System;
using System.IO;

namespace Azalea.Text;

/// <summary>
/// Binary reader specialized for reading True Type Font (.ttf) files.
/// </summary>
public class FontReader : BinaryReader
{
	public FontReader(Stream stream)
		: base(stream) { }

	public void SkipBytes(uint bytes) => BaseStream.Position += bytes;

	public void GoTo(uint bytes) => BaseStream.Position = bytes;
	public void GoTo(long bytes) => BaseStream.Position = bytes;

	public uint GetLocation() => (uint)BaseStream.Position;

	public override ushort ReadUInt16()
	{
		var value = base.ReadUInt16();

		if (BitConverter.IsLittleEndian)
			value = (ushort)(value >> 8 | value << 8);

		return value;
	}

	public override uint ReadUInt32()
	{
		var value = base.ReadUInt32();

		if (BitConverter.IsLittleEndian)
		{
			var b1 = (value >> 0) & 0xff;
			var b2 = (value >> 8) & 0xff;
			var b3 = (value >> 16) & 0xff;
			var b4 = (value >> 24) & 0xff;

			value = b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
		}

		return value;
	}

	public override short ReadInt16() => (short)ReadUInt16();

	public string ReadTag()
	{
		Span<char> tag = stackalloc char[4];

		for (int i = 0; i < tag.Length; i++)
			tag[i] = (char)base.ReadByte();

		return tag.ToString();
	}
}
