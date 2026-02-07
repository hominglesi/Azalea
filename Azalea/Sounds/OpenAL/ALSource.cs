using Azalea.Sounds.OpenAL.Enums;
using Azalea.Utils;

namespace Azalea.Sounds.OpenAL;
internal class ALSource : Disposable
{
	public uint Handle;

	public ALSource()
	{
		Handle = ALC.GenSource();
	}

	private float _gain = 1;
	public float Gain
	{
		get => _gain;
		set
		{
			if (_gain == value) return;
			ALC.SetSourceGain(Handle, value);
			_gain = value;
		}
	}

	public float _pitch = 1;
	public float Pitch
	{
		get => _pitch;
		set
		{
			if (_pitch == value) return;
			ALC.SetSourcePitch(Handle, value);
			_pitch = value;
		}
	}

	private bool _looping = false;
	public bool Looping
	{
		get => _looping;
		set
		{
			if (_looping == value) return;
			ALC.SetSourceLooping(Handle, value);
			_looping = value;
		}
	}

	public void Play() => ALC.SourcePlay(Handle);
	public void Pause() => ALC.SourcePause(Handle);
	public void Stop() => ALC.SourceStop(Handle);

	public void BindBuffer(ALBuffer buffer) => ALC.BindBuffer(Handle, buffer.Handle);
	public int GetBuffersProcessed() => ALC.GetBuffersProcessed(Handle);
	public int GetBuffersQueued() => ALC.GetBuffersQueued(Handle);
	public void QueueBuffer(uint handle) => ALC.SourceQueueBuffer(Handle, handle);
	public void QueueBuffer(ALBuffer buffer) => QueueBuffer(buffer.Handle);
	public uint UnqueueBuffer() => ALC.SourceUnqueueBuffer(Handle);

	public ALSourceState GetState() => ALC.GetSourceState(Handle);
	public float GetSecOffset() => ALC.GetSecOffset(Handle);

	protected override void OnDispose()
		=> ALC.DeleteSource(Handle);
}
