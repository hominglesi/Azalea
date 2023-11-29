using Azalea.Audio.OpenAL;
using Azalea.Extentions;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;

namespace Azalea.Audio;

//http://soundfile.sapp.org/doc/WaveFormat/
internal class WavSound : ISoundData
{
	private byte[] _wavData;

	public WavSound(Stream stream)
	{
		//We save the original wav file data and later slice it to prevent additional memory allocations
		_wavData = stream.ReadAllBytesToArray();

		ReadOnlySpan<byte> wav = _wavData;
		var index = 0;
		if (wav[index++] != 'R' || wav[index++] != 'I' || wav[index++] != 'F' || wav[index++] != 'F')
		{
			throw new Exception("Given file is not in RIFF format");
		}

		int chunkSize = BinaryPrimitives.ReadInt32LittleEndian(wav.Slice(index, 4));
		index += 4;

		if (wav[index++] != 'W' || wav[index++] != 'A' || wav[index++] != 'V' || wav[index++] != 'E')
		{
			throw new Exception("Given file is not in WAVE format");
		}

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
				_size = size;
				index += size;
			}
			else
			{
				Console.WriteLine($"Unknown Section: {identifier}");
				index += size;
			}
		}

		Debug.Assert(_dataOffset != 0);

		_lengthInSamples = _size * 8 / (_numChannels * _bitsPerSample);
		_length = _lengthInSamples / (float)_sampleRate;

	}

	private short _audioFormat;
	private short _numChannels;
	private int _sampleRate;
	private int _byteRate;
	private short _blockAlign;
	private short _bitsPerSample;

	private ALFormat _format;
	private int _dataOffset;
	private int _size;
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
			_sampleRate = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
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
					_format = ALFormat.Mono8;
				else if (_bitsPerSample == 16)
					_format = ALFormat.Mono16;
				else
				{
					Console.WriteLine($"Can't Play mono {_bitsPerSample} sound.");
				}
			}
			else if (_numChannels == 2)
			{
				if (_bitsPerSample == 8)
					_format = ALFormat.Stereo8;
				else if (_bitsPerSample == 16)
					_format = ALFormat.Stereo16;
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

	public ALFormat Format => _format;
	public ReadOnlySpan<byte> Data => ((ReadOnlySpan<byte>)_wavData).Slice(_dataOffset, _size);
	public int Size => _size;
	public int Frequency => _sampleRate;
}
