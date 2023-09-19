using Silk.NET.OpenAL;
using System;
using System.Buffers.Binary;
using System.Text;

namespace Azalea.Audios;
public class Sound : IDisposable
{
	internal uint Buffer;
	private AL _al;

	private short _channelCount = -1;
	private short _bitsPerSample = -1;
	private int _size;
	private int _frequency = -1;

	private int _lengthInSamples;
	private float _length;
	public float Length => _length;

	internal unsafe Sound(string filePath, AL al)
	{
		_al = al;
		Buffer = _al.GenBuffer();


		ReadOnlySpan<byte> file = AzaleaGame.Main.Resources.Get(filePath);
		var index = 0;
		if (file[index++] != 'R' || file[index++] != 'I' || file[index++] != 'F' || file[index++] != 'F')
		{
			Console.WriteLine("Given file is not in RIFF format");
			return;
		}

		var chunkSize = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
		index += 4;

		if (file[index++] != 'W' || file[index++] != 'A' || file[index++] != 'V' || file[index++] != 'E')
		{
			Console.WriteLine("Given file is not in WAVE format");
			return;
		}

		int byteRate = -1;
		short blockAlign = -1;
		BufferFormat format = 0;

		while (index + 4 < file.Length)
		{
			var identifier = "" + (char)file[index++] + (char)file[index++] + (char)file[index++] + (char)file[index++];
			var size = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
			index += 4;
			if (identifier == "fmt ")
			{
				if (size != 16)
				{
					Console.WriteLine($"Unknown Audio Format with subchunk1 size {size}");
				}
				else
				{
					var audioFormat = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
					index += 2;
					if (audioFormat != 1)
					{
						Console.WriteLine($"Unknown Audio Format with ID {audioFormat}");
					}
					else
					{
						_channelCount = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
						index += 2;
						_frequency = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
						index += 4;
						byteRate = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
						index += 4;
						blockAlign = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
						index += 2;
						_bitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
						index += 2;

						if (_channelCount == 1)
						{
							if (_bitsPerSample == 8)
								format = BufferFormat.Mono8;
							else if (_bitsPerSample == 16)
								format = BufferFormat.Mono16;
							else
							{
								Console.WriteLine($"Can't Play mono {_bitsPerSample} sound.");
							}
						}
						else if (_channelCount == 2)
						{
							if (_bitsPerSample == 8)
								format = BufferFormat.Stereo8;
							else if (_bitsPerSample == 16)
								format = BufferFormat.Stereo16;
							else
							{
								Console.WriteLine($"Can't Play stereo {_bitsPerSample} sound.");
							}
						}
						else
						{
							Console.WriteLine($"Can't play audio with {_channelCount} sound");
						}
					}
				}
			}
			else if (identifier == "data")
			{
				var data = file.Slice(index, size);
				index += size;

				fixed (byte* pData = data)
					_al.BufferData(Buffer, format, pData, size, _frequency);
				_size += size;
				Console.WriteLine($"Read {size} bytes Data");
			}
			else if (identifier == "JUNK")
			{
				// this exists to align things
				index += size;
			}
			else if (identifier == "iXML")
			{
				var v = file.Slice(index, size);
				var str = Encoding.ASCII.GetString(v);
				Console.WriteLine($"iXML Chunk: {str}");
				index += size;
			}
			else if (identifier == "LIST")
			{
				var v = file.Slice(index, size);
				var str = Encoding.ASCII.GetString(v).Substring(4);
				Console.WriteLine($"List Chunk: {str}");
				index += size;

			}
			else
			{
				Console.WriteLine($"Unknown Section: {identifier}");
				index += size;
			}
		}

		_lengthInSamples = _size * 8 / (_channelCount * _bitsPerSample);

		_length = _lengthInSamples / (float)_frequency;
	}

	protected bool Disposed;
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (Disposed == false)
		{
			_al.DeleteBuffer(Buffer);
			Disposed = true;
		}
	}
}
