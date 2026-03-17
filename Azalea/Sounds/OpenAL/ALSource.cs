using Azalea.Sounds.OpenAL.Enums;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.Sounds.OpenAL;
internal class ALSource : Disposable
{
	private readonly ALAudioManager _audioManager;
	public uint Handle;

	public ALSource(ALAudioManager audioManager)
	{
		_audioManager = audioManager;
		Handle = _audioManager.GenerateSource();

		_audioManager.SetSourcePosition(Handle, Vector3.Zero);
		_audioManager.SetSourceVelocity(Handle, Vector3.Zero);
		_audioManager.SetSourceRelative(Handle, false);
	}

	private float _gain = 1;
	public float Gain
	{
		get => _gain;
		set
		{
			if (_gain == value) return;
			_audioManager.SetSourceGain(Handle, value);
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
			_audioManager.SetSourcePitch(Handle, value);
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
			_audioManager.SetSourceLooping(Handle, value);
			_looping = value;
		}
	}

	public void Play() => _audioManager.PlaySource(Handle);
	public void Pause() => _audioManager.PauseSource(Handle);
	public void Stop() => _audioManager.StopSource(Handle);

	public void BindBuffer(ALBuffer buffer) => _audioManager.BindSourceBuffer(Handle, buffer.Handle);
	public int GetBuffersProcessed() => _audioManager.GetSourceBuffersProcessed(Handle);
	public int GetBuffersQueued() => _audioManager.GetSourceBuffersQueued(Handle);
	public void QueueBuffer(uint handle) => _audioManager.QueueSourceBuffer(Handle, handle);
	public void QueueBuffer(ALBuffer buffer) => _audioManager.QueueSourceBuffer(Handle, buffer.Handle);
	public uint UnqueueBuffer() => _audioManager.UnqueueSourceBuffer(Handle);
	public void UnqueueAllBuffers() => _audioManager.UnqueueAllSourceBuffers(Handle);

	public ALSourceState GetState() => _audioManager.GetSourceState(Handle);
	public float GetSecOffset() => _audioManager.GetSourceSecOffset(Handle);
	public void SetSecOffset(float offset) => _audioManager.SetSourceSecOffset(Handle, offset);

	protected override void OnDispose()
		=> _audioManager.DeleteSource(Handle);
}
