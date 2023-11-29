using Azalea.Audio.OpenAL;
using System;

namespace Azalea.Audio;
internal interface ISoundData
{
	internal ALFormat Format { get; }
	internal ReadOnlySpan<byte> Data { get; }
	internal int Size { get; }
	internal int Frequency { get; }
}
