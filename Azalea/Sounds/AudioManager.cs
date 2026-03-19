using Azalea.Sounds.OpenAL;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Azalea.Sounds;
internal abstract class AudioManager : Disposable, IAudioManager
{
	internal abstract record AudioCommand;

	private readonly int _audioThreadId;
	private readonly Channel<AudioCommand> _commandChannel;

	protected AudioManager()
	{
		_audioThreadId = Environment.CurrentManagedThreadId;

		_commandChannel = Channel.CreateUnbounded<AudioCommand>(new()
		{
			SingleReader = true
		});
	}

	public void AssertAudioThread()
	{
		if (IsAudioThread() == false)
			throw new InvalidOperationException("Current thread is not Audio thread");
	}

	public bool IsAudioThread() => Environment.CurrentManagedThreadId == _audioThreadId;

	internal record BatchCommand(List<AudioCommand> commands) : AudioCommand;
	private Dictionary<int, List<AudioCommand>> _commandQueues = [];
	public void BeginCommandQueue()
	{
		var callingThread = Environment.CurrentManagedThreadId;

		if (_commandQueues.ContainsKey(callingThread))
			throw new Exception("Cannot begin two command queues at the same time!");

		_commandQueues[callingThread] = [];
	}

	public void SubmitCommandQueue()
	{
		var callingThread = Environment.CurrentManagedThreadId;

		if (_commandQueues.TryGetValue(callingThread, out var queue))
		{
			_commandQueues.Remove(callingThread);
			IssueCommand(new BatchCommand(queue));
		}
		else throw new Exception("Command queue hasn't been started yet");
	}

	public void IssueCommand(AudioCommand command)
	{
		var callingThread = Environment.CurrentManagedThreadId;

		if (_commandQueues.TryGetValue(callingThread, out var queue))
		{
			queue.Add(command);
		}
		else if (_commandChannel.Writer.TryWrite(command) == false)
			Console.WriteLine("Could not write command");
	}

	protected abstract void HandleCommand(AudioCommand command);
	public void HandleCommands()
	{
		while (_commandChannel.Reader.TryRead(out var command))
			HandleCommand(command);
	}

	public abstract float MasterVolume { get; set; }

	public abstract IAudioSource[] AudioChannels { get; }
	public abstract IAudioSource[] AudioByteChannels { get; }
	public abstract IAudioSource[] AudioByteInternalChannels { get; }

	public abstract IAudioInstance Play(Sound sound, float gain = 1, bool looping = false);
	public abstract IAudioInstance PlayByte(SoundByte soundByte, float gain = 1, bool looping = false);
	public abstract IAudioInstance PlayByteInternal(SoundByte soundByte, float gain = 1, bool looping = false);
	public abstract SoundByte CreateSoundByte(byte[] data, int dataLength, ALFormat format, int frequency);

	public virtual void Update() { }
}
