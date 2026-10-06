using System;

namespace Azalea.Platform.Audio.OpenAL;

internal class ALSource(uint handle, uint[] buffers)
{
	public const int BufferCount = 8;

	public readonly uint Handle = handle;
	public readonly uint[] Buffers = buffers;
	public readonly float[] BufferStartTimes = new float[BufferCount];
	public int CurrentBufferStartTime = 0;
	public int NextBufferStartTime = 0;
	public float SourceOffset = 0;

	internal FFmpegStreamReader? CurrentReader { get; set; }

	public AudioInstance? CurrentInstance
	{
		get;
		internal set { if (field == value) return; field = value; OnInstanceChanged?.Invoke(field); }
	}
	public event Action<AudioInstance?>? OnInstanceChanged;
}
