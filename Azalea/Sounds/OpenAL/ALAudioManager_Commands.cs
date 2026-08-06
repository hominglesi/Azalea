using Azalea.Native.OpenAL;
using Azalea.Threading;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Sounds.OpenAL;
internal unsafe partial class ALAudioManager
{
	public const string OpenALPath = "soft_oal.dll";

	#region BindSourceBuffer

	internal record BindSourceBufferCommand(uint source, ValuePromise<uint> buffer) : AudioCommand;

	private static void bindSourceBuffer(uint source, uint buffer)
		=> AL.Sourcei(source, AL.BUFFER, (int)buffer);

	public void BindSourceBuffer(uint source, ValuePromise<uint> buffer)
	{
		if (IsAudioThread() && buffer.IsResolved)
			bindSourceBuffer(source, buffer.Value);
		else
			IssueCommand(new BindSourceBufferCommand(source, buffer));
	}

	#endregion
	#region BufferAndFreeData

	internal record BufferAndFreeDataCommand(ValuePromise<uint> buffer, byte[] data, int dataLength, int format, int frequency) : AudioCommand;

	private static void bufferAndFreeData(uint buffer, byte[] data, int dataLength, int format, int frequency)
	{
		bufferData(buffer, data, dataLength, format, frequency);
		ArrayPool<byte>.Shared.Return(data);
	}

	public void BufferAndFreeData(ValuePromise<uint> buffer, byte[] data, int dataLength, int format, int frequency)
	{
		if (IsAudioThread() && buffer.IsResolved)
			bufferAndFreeData(buffer.Value, data, dataLength, format, frequency);
		else
			IssueCommand(new BufferAndFreeDataCommand(buffer, data, dataLength, format, frequency));
	}

	#endregion
	#region BufferData

	internal record BufferDataCommand(ValuePromise<uint> buffer, byte[] data, int dataLength, int format, int frequency) : AudioCommand;

	private static void bufferData(uint buffer, byte[] data, int dataLength, int format, int frequency)
		=> AL.BufferData(buffer, format, ref data[0], dataLength, frequency);

	public void BufferData(ValuePromise<uint> buffer, byte[] data, int dataLength, int format, int frequency)
	{
		if (IsAudioThread() && buffer.IsResolved)
			bufferData(buffer.Value, data, dataLength, format, frequency);
		else
			IssueCommand(new BufferDataCommand(buffer, data, dataLength, format, frequency));
	}

	#endregion
	#region CloseDevice

	internal record CloseDeviceCommand(IntPtr device) : AudioCommand;

	private void closeDevice(IntPtr device) => ALC.CloseDevice(device);

	public void CloseDevice(IntPtr device)
	{
		if (IsAudioThread())
			closeDevice(device);
		else
			IssueCommand(new CloseDeviceCommand(device));
	}

	#endregion
	#region CreateContext

	private static IntPtr createContext(IntPtr device, int[] attributeList)
		=> ALC.CreateContext(device, ref attributeList[0]);

	public IntPtr CreateContext(IntPtr device, int[] attributeList)
	{
		AssertAudioThread();

		return createContext(device, attributeList);
	}

	#endregion
	#region DecrementCounter

	internal record DecrementCounterCommand(AtomicCounter counter) : AudioCommand;

	private void decrementCounter(AtomicCounter counter)
		=> counter.Decrement();

	public void DecrementCounter(AtomicCounter counter)
	{
		if (IsAudioThread())
			decrementCounter(counter);
		else
			IssueCommand(new DecrementCounterCommand(counter));
	}

	#endregion
	#region DeleteBuffer

	internal record DeleteBufferCommand(ValuePromise<uint> buffer) : AudioCommand;

	private static void deleteBuffer(uint buffer) => AL.DeleteBuffers(1, ref buffer);

	public void DeleteBuffer(ValuePromise<uint> buffer)
	{
		if (IsAudioThread() && buffer.IsResolved)
			deleteBuffer(buffer.Value);
		else
			IssueCommand(new DeleteBufferCommand(buffer));
	}

	#endregion
	#region DeleteSource

	internal record DeleteSourceCommand(uint source) : AudioCommand;

	public static void deleteSource(uint source) => AL.DeleteSources(1, ref source);

	public void DeleteSource(uint source)
	{
		if (IsAudioThread())
			deleteSource(source);
		else
			IssueCommand(new DeleteSourceCommand(source));
	}

	#endregion
	#region EnumerateOutputDevices

	private static IEnumerable<string> enumerateOutputDevices()
	{
		var deviceList = ALC.GetString(nint.Zero, ALC.ALL_DEVICES_SPECIFIER);

		string? deviceName;
		while (string.IsNullOrEmpty(deviceName = Marshal.PtrToStringAnsi(deviceList)) == false)
		{
			yield return deviceName;
			deviceList += deviceName.Length + 1;
		}
	}

	public IEnumerable<string> EnumerateOutputDevices()
	{
		AssertAudioThread();

		return enumerateOutputDevices();
	}

	#endregion
	#region GenerateBuffer

	internal record GenerateBufferCommand(Promise<uint> result) : AudioCommand;

	private uint generateBuffer()
	{
		uint buffer = 0;
		AL.GenBuffers(1, ref buffer);
		return buffer;
	}

	public ValuePromise<uint> GenerateBuffer()
	{
		if (IsAudioThread())
			return new ValuePromise<uint>(generateBuffer());

		var result = new Promise<uint>();
		IssueCommand(new GenerateBufferCommand(result));
		return new ValuePromise<uint>(result);
	}

	#endregion
	#region GenerateSource



	private uint generateSource()
	{
		uint source = 0;
		AL.GenSources(1, ref source);
		return source;
	}

	public uint GenerateSource()
	{
		AssertAudioThread();

		return generateSource();
	}

	#endregion
	#region GetALRenderer

	private static string getALRenderer()
	{
		var ptr = AL.GetString(AL.RENDERER);
		return Marshal.PtrToStringAnsi(ptr)!;
	}

	public string GetALRenderer()
	{
		AssertAudioThread();

		return getALRenderer();
	}

	#endregion
	#region GetALVersion

	private static string getALVersion()
	{
		var ptr = AL.GetString(AL.VERSION);
		return Marshal.PtrToStringAnsi(ptr)!;
	}

	public string GetALVersion()
	{
		AssertAudioThread();

		return getALVersion();
	}

	#endregion
	#region GetDefaultDeviceName

	public string getDefaultDeviceName()
	{
		var str = ALC.GetString(IntPtr.Zero, ALC.DEFAULT_ALL_DEVICES_SPECIFIER);
		return Marshal.PtrToStringAnsi(str)!;
	}

	public string GetDefaultDeviceName()
	{
		AssertAudioThread();

		return getDefaultDeviceName();
	}

	#endregion
	#region GetDeviceConnected

	public bool getDeviceConnected(IntPtr device)
	{
		int connected = 0;
		ALC.GetIntegerv(device, ALC.CONNECTED, 1, ref connected);
		return connected != 0;
	}

	public bool GetDeviceConnected(IntPtr device)
	{
		AssertAudioThread();

		return getDeviceConnected(device);
	}

	#endregion
	#region GetDeviceFrequency

	public int getDeviceFrequency(IntPtr device)
	{
		int frequency = 0;
		ALC.GetIntegerv(device, ALC.FREQUENCY, 1, ref frequency);
		return frequency;
	}

	public int GetDeviceFrequency(IntPtr device)
	{
		AssertAudioThread();

		return getDeviceFrequency(device);
	}

	#endregion
	#region GetDeviceHRTF

	public bool getDeviceHRTF(IntPtr device)
	{
		int hrtf = 0;
		ALC.GetIntegerv(device, ALC.HRTF_SOFT, 1, ref hrtf);
		return hrtf != 0;
	}

	public bool GetDeviceHRTF(IntPtr device)
	{
		AssertAudioThread();

		return getDeviceHRTF(device);
	}

	#endregion
	#region GetDeviceName

	public string getDeviceName(IntPtr device)
	{
		var str = ALC.GetString(device, ALC.ALL_DEVICES_SPECIFIER);
		return Marshal.PtrToStringAnsi(str)!;
	}

	public string GetDeviceName(IntPtr device)
	{
		AssertAudioThread();

		return getDeviceName(device);
	}

	#endregion
	#region GetSourceBuffersProcessed

	private int getSourceBuffersProcessed(uint source)
	{
		int buffersProcessed = 0;
		AL.GetSourcei(source, AL.BUFFERS_PROCESSED, ref buffersProcessed);
		return buffersProcessed;
	}

	public int GetSourceBuffersProcessed(uint source)
	{
		AssertAudioThread();

		return getSourceBuffersProcessed(source);
	}

	#endregion
	#region GetSourceBuffersQueued

	private int getSourceBuffersQueued(uint source)
	{
		int buffersQueued = 0;
		AL.GetSourcei(source, AL.BUFFERS_QUEUED, ref buffersQueued);
		return buffersQueued;
	}

	public int GetSourceBuffersQueued(uint source)
	{
		AssertAudioThread();

		return getSourceBuffersQueued(source);
	}

	#endregion
	#region GetSourceSecOffset

	public static float getSourceSecOffset(uint source)
	{
		float secOffset = 0;
		AL.GetSourcef(source, AL.SEC_OFFSET, ref secOffset);
		return secOffset;
	}

	public float GetSourceSecOffset(uint source)
	{
		AssertAudioThread();

		return getSourceSecOffset(source);
	}

	#endregion
	#region GetSourceState

	public static int getSourceState(uint source)
	{
		int sourceState = 0;
		AL.GetSourcei(source, AL.SOURCE_STATE, ref sourceState);
		return sourceState;
	}

	public int GetSourceState(uint source)
	{
		AssertAudioThread();

		return getSourceState(source);
	}

	#endregion
	#region MakeContextCurrent



	private static bool makeContextCurrent(IntPtr context)
	{
		return ALC.MakeContextCurrent(context);
	}

	public bool MakeContextCurrent(IntPtr context)
	{
		AssertAudioThread();

		return makeContextCurrent(context);
	}

	#endregion
	#region OpenDevice



	private static IntPtr openDevice(string? deviceName)
		=> ALC.OpenDevice(deviceName);

	public IntPtr OpenDevice(string? deviceName)
	{
		AssertAudioThread();

		return openDevice(deviceName);
	}

	#endregion
	#region PauseSource

	internal record PauseSourceCommand(uint source) : AudioCommand;



	private static void pauseSource(uint source) => AL.SourcePause(source);

	public void PauseSource(uint source)
	{
		if (IsAudioThread())
			pauseSource(source);
		else
			IssueCommand(new PauseSourceCommand(source));
	}

	#endregion
	#region PlaySource

	internal record PlaySourceCommand(uint source) : AudioCommand;



	private static void playSource(uint source) => AL.SourcePlay(source);

	public void PlaySource(uint source)
	{
		if (IsAudioThread())
			playSource(source);
		else
			IssueCommand(new PlaySourceCommand(source));
	}

	#endregion
	#region PrintErrors

	internal record PrintErrorsCommand() : AudioCommand;



	private static void printErrors()
	{
		var error = AL.GetError();
		while (error != AL.NO_ERROR)
		{
			Console.WriteLine("OpenAL Error: " + error);
			error = AL.GetError();
		}
	}

	public void PrintErrors()
	{
		if (IsAudioThread())
			printErrors();
		else
			IssueCommand(new PrintErrorsCommand());
	}

	#endregion
	#region QueueSourceBuffer

	internal record QueueSourceBufferCommand(uint source, ValuePromise<uint> buffer) : AudioCommand;



	private static void queueSourceBuffer(uint source, uint buffer)
		=> AL.SourceQueueBuffers(source, 1, ref buffer);

	public void QueueSourceBuffer(uint source, ValuePromise<uint> buffer)
	{
		if (IsAudioThread() && buffer.IsResolved)
			queueSourceBuffer(source, buffer.Value);
		else
			IssueCommand(new QueueSourceBufferCommand(source, buffer));
	}

	#endregion
	#region ReopenDevice

	internal record ReopenDeviceCommand(IntPtr device, string? deviceName, int[] attributes) : AudioCommand;

	private delegate bool ReopenDeviceDelegate(IntPtr device, [MarshalAs(UnmanagedType.LPStr)] string? deviceName, int[] attributes);
	private ReopenDeviceDelegate? _alcReopenDeviceSOFT;

	private void reopenDevice(IntPtr device, string? deviceName, int[] attributes)
	{
		_alcReopenDeviceSOFT ??=
			Marshal.GetDelegateForFunctionPointer<ReopenDeviceDelegate>(
				ALC.GetProcAddress(device, "alcReopenDeviceSOFT"));

		_alcReopenDeviceSOFT(device, deviceName, attributes);
	}

	public void ReopenDevice(IntPtr device, string? deviceName, int[] attributes)
	{
		if (IsAudioThread())
			reopenDevice(device, deviceName, attributes);
		else
			IssueCommand(new ReopenDeviceCommand(device, deviceName, attributes));
	}

	#endregion
	#region SetDistanceModel

	internal record SetDistanceModelCommand(int distanceModel) : AudioCommand;

	private static void setDistanceModel(int distanceModel)
		=> AL.DistanceModel(distanceModel);

	public void SetDistanceModel(int distanceModel)
	{
		if (IsAudioThread())
			setDistanceModel(distanceModel);
		else
			IssueCommand(new SetDistanceModelCommand(distanceModel));
	}

	#endregion
	#region SetListenerGain

	internal record SetListenerGainCommand(float gain) : AudioCommand;

	private static void setListenerGain(float gain) => AL.Listenerf(AL.GAIN, gain);

	public void SetListenerGain(float gain)
	{
		if (IsAudioThread())
			setListenerGain(gain);
		else
			IssueCommand(new SetListenerGainCommand(gain));
	}

	#endregion
	#region SetListenerPosition

	internal record SetListenerPositionCommand(Vector3 position) : AudioCommand;

	private static void setListenerPosition(Vector3 position)
		=> AL.Listener3f(AL.POSITION, position.X, position.Y, position.Z);

	public void SetListenerPosition(Vector3 position)
	{
		if (IsAudioThread())
			setListenerPosition(position);
		else
			IssueCommand(new SetListenerPositionCommand(position));
	}

	#endregion
	#region SetListenerVelocity

	internal record SetListenerVelocityCommand(Vector3 velocity) : AudioCommand;

	private static void setListenerVelocity(Vector3 velocity)
		=> AL.Listener3f(AL.VELOCITY, velocity.X, velocity.Y, velocity.Z);

	public void SetListenerVelocity(Vector3 velocity)
	{
		if (IsAudioThread())
			setListenerVelocity(velocity);
		else
			IssueCommand(new SetListenerVelocityCommand(velocity));
	}

	#endregion
	#region SetSourceGain

	internal record SetSourceGainCommand(uint source, float gain) : AudioCommand;

	private static void setSourceGain(uint source, float gain)
		=> AL.Sourcef(source, AL.GAIN, gain);

	public void SetSourceGain(uint source, float gain)
	{
		if (IsAudioThread())
			setSourceGain(source, gain);
		else
			IssueCommand(new SetSourceGainCommand(source, gain));
	}

	#endregion
	#region SetSourceLooping

	internal record SetSourceLoopingCommand(uint source, bool looping) : AudioCommand;

	private static void setSourceLooping(uint source, bool looping)
		=> AL.Sourcei(source, AL.LOOPING, looping ? 1 : 0);

	public void SetSourceLooping(uint source, bool looping)
	{
		if (IsAudioThread())
			setSourceLooping(source, looping);
		else
			IssueCommand(new SetSourceLoopingCommand(source, looping));
	}

	#endregion
	#region SetSourcePitch

	internal record SetSourcePitchCommand(uint source, float pitch) : AudioCommand;

	private static void setSourcePitch(uint source, float pitch)
		=> AL.Sourcef(source, AL.PITCH, pitch);

	public void SetSourcePitch(uint source, float pitch)
	{
		if (IsAudioThread())
			setSourcePitch(source, pitch);
		else
			IssueCommand(new SetSourcePitchCommand(source, pitch));
	}

	#endregion
	#region SetSourcePosition

	internal record SetSourcePositionCommand(uint source, Vector3 position) : AudioCommand;

	private static void setSourcePosition(uint source, Vector3 position)
		=> AL.Source3f(source, AL.POSITION, position.X, position.Y, position.Z);

	public void SetSourcePosition(uint source, Vector3 position)
	{
		if (IsAudioThread())
			setSourcePosition(source, position);
		else
			IssueCommand(new SetSourcePositionCommand(source, position));
	}

	#endregion
	#region SetSourceRelative

	internal record SetSourceRelativeCommand(uint source, bool relative) : AudioCommand;

	private static void setSourceRelative(uint source, bool relative)
		=> AL.Sourcei(source, AL.SOURCE_RELATIVE, relative ? 1 : 0);

	public void SetSourceRelative(uint source, bool relative)
	{
		if (IsAudioThread())
			setSourceRelative(source, relative);
		else
			IssueCommand(new SetSourceRelativeCommand(source, relative));
	}

	#endregion
	#region SetSourceSecOffset

	internal record SetSourceSecOffsetCommand(uint source, float secOffset) : AudioCommand;

	private static void setSourceSecOffset(uint source, float offset)
		=> AL.Sourcef(source, AL.SEC_OFFSET, offset);

	public void SetSourceSecOffset(uint source, float offset)
	{
		if (IsAudioThread())
			setSourceSecOffset(source, offset);
		else
			IssueCommand(new SetSourceSecOffsetCommand(source, offset));
	}

	#endregion
	#region SetSourceVelocity

	internal record SetSourceVelocityCommand(uint source, Vector3 velocity) : AudioCommand;

	private static void setSourceVelocity(uint source, Vector3 velocity)
		=> AL.Source3f(source, AL.VELOCITY, velocity.X, velocity.Y, velocity.Z);

	public void SetSourceVelocity(uint source, Vector3 velocity)
	{
		if (IsAudioThread())
			setSourceVelocity(source, velocity);
		else
			IssueCommand(new SetSourceVelocityCommand(source, velocity));
	}

	#endregion
	#region StopSource

	internal record StopSourceCommand(uint source) : AudioCommand;

	private static void stopSource(uint source) => AL.SourceStop(source);

	public void StopSource(uint source)
	{
		if (IsAudioThread())
			stopSource(source);
		else
			IssueCommand(new StopSourceCommand(source));
	}

	#endregion
	#region UnqueueAllSourceBuffers

	internal record UnqueueAllSourceBuffersCommand(uint source) : AudioCommand;

	private void unqueueAllSourceBuffers(uint source)
	{
		var queuedBuffers = getSourceBuffersQueued(source);
		while (queuedBuffers-- > 0)
			unqueueSourceBuffer(source);
	}

	public void UnqueueAllSourceBuffers(uint source)
	{
		if (IsAudioThread())
			unqueueAllSourceBuffers(source);
		else
			IssueCommand(new UnqueueAllSourceBuffersCommand(source));
	}

	#endregion
	#region UnqueueSourceBuffer

	internal record UnqueueSourceBufferCommand(uint source, Promise<uint> result) : AudioCommand;

	private static uint unqueueSourceBuffer(uint source)
	{
		uint buffer = 0;
		AL.SourceUnqueueBuffers(source, 1, ref buffer);
		return buffer;
	}

	public ValuePromise<uint> UnqueueSourceBuffer(uint source)
	{
		if (IsAudioThread())
			return new ValuePromise<uint>(unqueueSourceBuffer(source));

		var promise = new Promise<uint>();
		IssueCommand(new UnqueueSourceBufferCommand(source, promise));
		return new ValuePromise<uint>(promise);
	}

	#endregion
}
