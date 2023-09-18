using Silk.NET.OpenAL;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Azalea.Platform.Silk;
public unsafe class SilkAudio : IDisposable
{
	public static SilkAudio Instance { get; }

	public bool IsPlaying;

	private readonly ALContext _alc;
	private readonly AL _al;
	private readonly Context* _context;
	private readonly Device* _device;

	private readonly uint[] _sources = new uint[32];
	private readonly List<AudioInstance> _instances = new();

	static SilkAudio()
	{
		Instance = new SilkAudio();
	}

	public SilkAudio()
	{
		_alc = ALContext.GetApi(true);
		_al = AL.GetApi(true);
		_device = _alc.OpenDevice("");
		if (_device == null)
		{
			Console.WriteLine("Could not create device");
			return;
		}

		_context = _alc.CreateContext(_device, null);
		_alc.MakeContextCurrent(_context);

		_al.GetError();

		_sources = _al.GenSources(32);

		isInitialized = true;
	}

	private bool isInitialized;

	private int _sourceIndex;
	public unsafe void PlaySound(AudioInstance audio)
	{
		_al.SourceStop(_sources[_sourceIndex]);

		_al.SetSourceProperty(_sources[_sourceIndex], SourceInteger.Buffer, audio.Buffer);
		_al.SetSourceProperty(_sources[_sourceIndex], SourceBoolean.Looping, false);

		_al.SourcePlay(_sources[_sourceIndex]);

		_sourceIndex++;
		if (_sourceIndex >= 32) _sourceIndex = 0;

		IsPlaying = true;
	}

	public AudioInstance CreateInstance(string filePath)
	{
		var newInstance = new AudioInstance(filePath, _al);
		_instances.Add(newInstance);
		return newInstance;
	}


	protected bool Disposed;

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool isDisposing)
	{
		if (Disposed == false && isInitialized == true)
		{
			foreach (var instance in _instances)
			{
				instance.Dispose();
			}

			_al.DeleteSources(_sources);
			_alc.DestroyContext(_context);
			_alc.CloseDevice(_device);
			_al.Dispose();
			_alc.Dispose();

			Disposed = true;
		}
	}


	public class AudioInstance : IDisposable
	{
		internal uint Buffer;
		private AL _al;

		internal AudioInstance(string filePath, AL al)
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

			short numChannels = -1;
			int sampleRate = -1;
			int byteRate = -1;
			short blockAlign = -1;
			short bitsPerSample = -1;
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
							numChannels = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
							index += 2;
							sampleRate = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
							index += 4;
							byteRate = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
							index += 4;
							blockAlign = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
							index += 2;
							bitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
							index += 2;

							if (numChannels == 1)
							{
								if (bitsPerSample == 8)
									format = BufferFormat.Mono8;
								else if (bitsPerSample == 16)
									format = BufferFormat.Mono16;
								else
								{
									Console.WriteLine($"Can't Play mono {bitsPerSample} sound.");
								}
							}
							else if (numChannels == 2)
							{
								if (bitsPerSample == 8)
									format = BufferFormat.Stereo8;
								else if (bitsPerSample == 16)
									format = BufferFormat.Stereo16;
								else
								{
									Console.WriteLine($"Can't Play stereo {bitsPerSample} sound.");
								}
							}
							else
							{
								Console.WriteLine($"Can't play audio with {numChannels} sound");
							}
						}
					}
				}
				else if (identifier == "data")
				{
					var data = file.Slice(index, size);
					index += size;

					fixed (byte* pData = data)
						_al.BufferData(Buffer, format, pData, size, sampleRate);
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
}
