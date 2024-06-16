using System;
using System.IO;

namespace Azalea.Text;

/// <summary>
/// Binary reader specialized for reading True Type Font (.ttf) files
/// </summary>
public class FontReader : BinaryReader
{
	public FontReader(Stream stream)
		: base(stream) { }

	/// <summary>
	/// Seeks forward by the specified amount of bytes
	/// </summary>
	/// <param name="bytes">Specifies the number of bytes to skip</param>
	public void SkipBytes(int bytes)
		=> BaseStream.Position += bytes;

	/// <summary>
	/// Reads a 2-byte unsigned integer from the current stream using big-endian encoding 
	/// and advances the position of the stream by two bytes.
	/// </summary>
	/// <returns>A 2-byte unsigned integer read from this stream.</returns>
	public override ushort ReadUInt16()
	{
		var value = base.ReadUInt16();

		if (BitConverter.IsLittleEndian)
			value = (ushort)(value >> 8 | value << 8);

		return value;
	}

	/// <summary>
	/// Reads a 4-byte unsigned integer from the current stream using big-endian encoding 
	/// and advances the position of the stream by four bytes.
	/// </summary>
	/// <returns>A 4-byte unsigned integer read from this stream.</returns>
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

	/// <summary>
	/// Reads a 4 character string from the current stream
	/// and advances the position of the stream by 4 bytes.
	/// </summary>
	/// <returns>A 4 character string read from the stream.</returns>
	public string ReadTag()
	{
		Span<char> tag = stackalloc char[4];

		for (int i = 0; i < tag.Length; i++)
			tag[i] = (char)base.ReadByte();

		return tag.ToString();
	}
}
