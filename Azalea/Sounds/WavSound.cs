using Azalea.Extentions;
using Azalea.Sounds.OpenAL;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;

namespace Azalea.Sounds;

//http://soundfile.sapp.org/doc/WaveFormat/
internal class WavSound
{
	private readonly byte[] _wavBytes;

	public Span<byte> Data => _wavBytes.AsSpan().Slice(_dataOffset, _dataLength);
	public ALFormat Format { get; private set; }
	public int Frequency { get; private set; }

	public WavSound(Stream stream)
	{
		//We save the original wav file data and later slice it to prevent additional memory allocations
		_wavBytes = stream.ReadAllBytesToArray();

		ReadOnlySpan<byte> wav = _wavBytes;
		var index = 0;
		if (wav[index++] != 'R' || wav[index++] != 'I' || wav[index++] != 'F' || wav[index++] != 'F')
			throw new ArgumentException("Given stream is not of a valid .wav file");

		int chunkSize = BinaryPrimitives.ReadInt32LittleEndian(wav.Slice(index, 4));
		index += 4;

		if (wav[index++] != 'W' || wav[index++] != 'A' || wav[index++] != 'V' || wav[index++] != 'E')
			throw new ArgumentException("Given stream is not of a valid .wav file");

		while (index + 4 < wav.Length)
		{
			var identifier = "" + (char)wav[index++] + (char)wav[index++] + (char)wav[index++] + (char)wav[index++];
			var size = BinaryPrimitives.ReadInt32LittleEndian(wav.Slice(index, 4));
			index += 4;

			if (identifier == "fmt ")
			{
				if (size != 16)
				{
					Console.WriteLine($"Unknown Audio Format with subchunk1 size {size}");
				}
				else
				{
					readFmtSubchunk(wav, index);
					index += 16;
				}
			}
			else if (identifier == "data")
			{
				if (_dataOffset != 0)
				{
					throw new Exception("This wav file contains multiple 'data' sections. Please report this issue so it can be resolved");
				}
				_dataOffset = index;
				_dataLength = size;
				index += size;
			}
			else
			{
				if (identifier != "LIST" && identifier != "id3 ")
					Console.WriteLine($"Unknown Section: {identifier}");

				index += size;
			}
		}

		Debug.Assert(_dataOffset != 0);

		_lengthInSamples = _dataLength * 8 / (_numChannels * _bitsPerSample);
		_length = _lengthInSamples / (float)Frequency;
	}

	private short _audioFormat;
	private short _numChannels;
	private int _byteRate;
	private short _blockAlign;
	private short _bitsPerSample;

	private int _dataOffset;
	private int _dataLength;
	private int _lengthInSamples;
	private float _length;

	private void readFmtSubchunk(ReadOnlySpan<byte> data, int offset)
	{
		_audioFormat = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
		offset += 2;

		if (_audioFormat != 1)
		{
			Console.WriteLine($"Unknown Audio Format with ID {_audioFormat}");
		}
		else
		{
			_numChannels = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
			offset += 2;
			Frequency = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
			offset += 4;
			_byteRate = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
			offset += 4;
			_blockAlign = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
			offset += 2;
			_bitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
			offset += 2;

			if (_numChannels == 1)
			{
				if (_bitsPerSample == 8)
					Format = ALFormat.Mono8;
				else if (_bitsPerSample == 16)
					Format = ALFormat.Mono16;
				else
				{
					Console.WriteLine($"Can't Play mono {_bitsPerSample} sound.");
				}
			}
			else if (_numChannels == 2)
			{
				if (_bitsPerSample == 8)
					Format = ALFormat.Stereo8;
				else if (_bitsPerSample == 16)
					Format = ALFormat.Stereo16;
				else
				{
					Console.WriteLine($"Can't Play stereo {_bitsPerSample} sound.");
				}
			}
			else
			{
				Console.WriteLine($"Can't play audio with {_numChannels} sound");
			}
		}
	}
}
