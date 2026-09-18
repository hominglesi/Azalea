using Azalea.Utils;
using System;

namespace Azalea.Platform.Audio.OpenAL;
internal partial class ALByteSource(uint handle)
{
	public readonly uint Handle = handle;

	public AudioByteInstance? CurrentInstance
	{
		get;
		internal set { if (field == value) return; field = value; OnInstanceChanged?.Invoke(field); }
	}
	public event Action<AudioByteInstance?>? OnInstanceChanged;
}
