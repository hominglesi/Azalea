using Azalea.Sounds.OpenAL.Enums;
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

	[LibraryImport(OpenALPath)]
	private static partial void alcGetIntegerv(IntPtr device, int param, int size, int* value);

	[LibraryImport(OpenALPath, StringMarshalling = StringMarshalling.Utf8)]
	private static partial IntPtr alcGetProcAddress(IntPtr device, string functionName);

	[LibraryImport(OpenALPath)]
	private static partial IntPtr alcGetString(IntPtr device, int param);

	[LibraryImport(OpenALPath)]
	private static partial void alGetSourcef(uint source, int param, float* value);

	[LibraryImport(OpenALPath)]
	private static partial void alGetSourcei(uint source, int param, int* value);

	[LibraryImport(OpenALPath)]
	private static partial IntPtr alGetString(int param);

	[LibraryImport(OpenALPath)]
	private static partial void alListener3f(int param, float value1, float value2, float value3);

	[LibraryImport(OpenALPath)]
	private static partial void alListenerf(int param, float value);

	[LibraryImport(OpenALPath)]
	private static partial void alSource3f(uint source, int param, float value1, float value2, float value3);

	[LibraryImport(OpenALPath)]
	private static partial void alSourcef(uint source, int param, float value);

	[LibraryImport(OpenALPath)]
	private static partial void alSourcei(uint source, int param, int value);

	#region BindSourceBuffer

	internal record BindSourceBufferCommand(uint source, ValuePromise<uint> buffer) : AudioCommand;

	private static void bindSourceBuffer(uint source, uint buffer)
	{
		const int AL_BUFFER = 0x1009;
		alSourcei(source, AL_BUFFER, (int)buffer);
	}

	public void BindSourceBuffer(uint source, ValuePromise<uint> buffer)
	{
		if (IsAudioThread() && buffer.IsResolved)
			bindSourceBuffer(source, buffer.Value);
		else
			IssueCommand(new BindSourceBufferCommand(source, buffer));
	}

	#endregion
	#region BufferAndFreeData

	internal record BufferAndFreeDataCommand(ValuePromise<uint> buffer, byte[] data, int dataLength, ALFormat format, int frequency) : AudioCommand;

	private static void bufferAndFreeData(uint buffer, byte[] data, int dataLength, ALFormat format, int frequency)
	{
		bufferData(buffer, data, dataLength, format, frequency);
		ArrayPool<byte>.Shared.Return(data);
	}

	public void BufferAndFreeData(ValuePromise<uint> buffer, byte[] data, int dataLength, ALFormat format, int frequency)
	{
		if (IsAudioThread() && buffer.IsResolved)
			bufferAndFreeData(buffer.Value, data, dataLength, format, frequency);
		else
			IssueCommand(new BufferAndFreeDataCommand(buffer, data, dataLength, format, frequency));
	}

	#endregion
	#region BufferData

	internal record BufferDataCommand(ValuePromise<uint> buffer, byte[] data, int dataLength, ALFormat format, int frequency) : AudioCommand;

	[LibraryImport(OpenALPath)]
	private static partial void alBufferData(uint buffer, ALFormat format, void* data, int size, int frequency);

	private static void bufferData(uint buffer, byte[] data, int dataLength, ALFormat format, int frequency)
	{
		fixed (void* p = data)
			alBufferData(buffer, format, p, dataLength, frequency);
	}

	public void BufferData(ValuePromise<uint> buffer, byte[] data, int dataLength, ALFormat format, int frequency)
	{
		if (IsAudioThread() && buffer.IsResolved)
			bufferData(buffer.Value, data, dataLength, format, frequency);
		else
			IssueCommand(new BufferDataCommand(buffer, data, dataLength, format, frequency));
	}

	#endregion
	#region CloseDevice

	internal record CloseDeviceCommand(IntPtr device) : AudioCommand;

	[LibraryImport(OpenALPath)]
	[return: MarshalAs(UnmanagedType.U1)]
	private static partial bool alcCloseDevice(IntPtr device);

	private void closeDevice(IntPtr device) => alcCloseDevice(device);

	public void CloseDevice(IntPtr device)
	{
		if (IsAudioThread())
			closeDevice(device);
		else
			IssueCommand(new CloseDeviceCommand(device));
	}

	#endregion
	#region CreateContext

	[LibraryImport(OpenALPath)]
	private static partial IntPtr alcCreateContext(IntPtr device, int* attributeList);

	private static IntPtr createContext(IntPtr device, int[] attributeList)
	{
		fixed (int* ptr = attributeList)
			return alcCreateContext(device, ptr);
	}

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

	[LibraryImport(OpenALPath)]
	private static partial void alDeleteBuffers(int count, uint* buffers);

	private static void deleteBuffer(uint buffer)
	{
		alDeleteBuffers(1, &buffer);
	}

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

	[LibraryImport(OpenALPath)]
	private static partial void alDeleteSources(int count, uint* sources);

	public static void deleteSource(uint source)
	{
		alDeleteSources(1, &source);
	}

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
		const int ALC_ALL_DEVICES_SPECIFIER = 0x1013;
		var deviceList = alcGetString(IntPtr.Zero, ALC_ALL_DEVICES_SPECIFIER);

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

	internal record GenerateBufferCommand(Promise2<uint> result) : AudioCommand;

	[LibraryImport(OpenALPath)]
	private static partial void alGenBuffers(int count, uint* buffers);

	private uint generateBuffer()
	{
		uint buffer;
		alGenBuffers(1, &buffer);
		return buffer;
	}

	public ValuePromise<uint> GenerateBuffer()
	{
		if (IsAudioThread())
			return new ValuePromise<uint>(generateBuffer());

		var result = new Promise2<uint>();
		IssueCommand(new GenerateBufferCommand(result));
		return new ValuePromise<uint>(result);
	}

	#endregion
	#region GenerateSource

	[LibraryImport(OpenALPath)]
	private static partial void alGenSources(int count, uint* sources);

	private uint generateSource()
	{
		uint source;
		alGenSources(1, &source);
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
		const int AL_RENDERER = 0xB007;
		var ptr = alGetString(AL_RENDERER);
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
		const int AL_VERSION = 0xB002;
		var ptr = alGetString(AL_VERSION);
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
		const int ALC_DEFAULT_ALL_DEVICES_SPECIFIER = 0x1012;
		var str = alcGetString(IntPtr.Zero, ALC_DEFAULT_ALL_DEVICES_SPECIFIER);
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
		const int ALC_CONNECTED = 0x313;
		int connected;
		alcGetIntegerv(device, ALC_CONNECTED, 1, &connected);
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
		const int ALC_FREQUENCY = 0x1007;
		int frequency;
		alcGetIntegerv(device, ALC_FREQUENCY, 1, &frequency);
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
		const int ALC_HRTF_SOFT = 0x1992;
		int hrtf;
		alcGetIntegerv(device, ALC_HRTF_SOFT, 1, &hrtf);
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
		const int ALC_ALL_DEVICES_SPECIFIER = 0x1013;
		var str = alcGetString(device, ALC_ALL_DEVICES_SPECIFIER);
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
		const int AL_BUFFERS_PROCESSED = 0x1016;
		int buffersProcessed;
		alGetSourcei(source, AL_BUFFERS_PROCESSED, &buffersProcessed);
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
		const int AL_BUFFERS_QUEUED = 0x1015;
		int buffersQueued;
		alGetSourcei(source, AL_BUFFERS_QUEUED, &buffersQueued);
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
		const int AL_SEC_OFFSET = 0x1024;
		float secOffset;
		alGetSourcef(source, AL_SEC_OFFSET, &secOffset);
		return secOffset;
	}

	public float GetSourceSecOffset(uint source)
	{
		AssertAudioThread();

		return getSourceSecOffset(source);
	}

	#endregion
	#region GetSourceState

	public static ALSourceState getSourceState(uint source)
	{
		const int AL_SOURCE_STATE = 0x1010;
		int sourceState;
		alGetSourcei(source, AL_SOURCE_STATE, &sourceState);
		return (ALSourceState)sourceState;
	}

	public ALSourceState GetSourceState(uint source)
	{
		AssertAudioThread();

		return getSourceState(source);
	}

	#endregion
	#region MakeContextCurrent

	[LibraryImport(OpenALPath)]
	[return: MarshalAs(UnmanagedType.U1)]
	private static partial bool alcMakeContextCurrent(IntPtr context);

	private static bool makeContextCurrent(IntPtr context)
	{
		return alcMakeContextCurrent(context);
	}

	public bool MakeContextCurrent(IntPtr context)
	{
		AssertAudioThread();

		return makeContextCurrent(context);
	}

	#endregion
	#region OpenDevice

	[LibraryImport(OpenALPath, StringMarshalling = StringMarshalling.Utf8)]
	private static partial IntPtr alcOpenDevice(string? deviceName);

	private static IntPtr openDevice(string? deviceName)
		=> alcOpenDevice(deviceName);

	public IntPtr OpenDevice(string? deviceName)
	{
		AssertAudioThread();

		return openDevice(deviceName);
	}

	#endregion
	#region PauseSource

	internal record PauseSourceCommand(uint source) : AudioCommand;

	[LibraryImport(OpenALPath)]
	private static partial void alSourcePause(uint source);

	private static void pauseSource(uint source) => alSourcePause(source);

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

	[LibraryImport(OpenALPath)]
	private static partial void alSourcePlay(uint source);

	private static void playSource(uint source) => alSourcePlay(source);

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

	[LibraryImport(OpenALPath, EntryPoint = "alGetError")]
	private static partial ALError alGetError();

	private static void printErrors()
	{
		var error = alGetError();
		while (error != ALError.NoError)
		{
			Console.WriteLine("OpenAL Error: " + error);
			error = alGetError();
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

	[LibraryImport(OpenALPath)]
	private static partial void alSourceQueueBuffers(uint source, int size, uint* buffers);

	private static void queueSourceBuffer(uint source, uint buffer)
	{
		alSourceQueueBuffers(source, 1, &buffer);
	}

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
				alcGetProcAddress(device, "alcReopenDeviceSOFT"));

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

	[LibraryImport(OpenALPath)]
	public static partial void alDistanceModel(int distanceModel);

	private static void setDistanceModel(int distanceModel)
		=> alDistanceModel(distanceModel);

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

	private static void setListenerGain(float gain)
	{
		const int AL_GAIN = 0x100A;
		alListenerf(AL_GAIN, gain);
	}

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
	{
		const int AL_POSITION = 0x1004;
		alListener3f(AL_POSITION, position.X, position.Y, position.Z);
	}

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
	{
		const int AL_VELOCITY = 0x1006;
		alListener3f(AL_VELOCITY, velocity.X, velocity.Y, velocity.Z);
	}

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
	{
		const int AL_GAIN = 0x100A;
		alSourcef(source, AL_GAIN, gain);
	}

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
	{
		const int AL_LOOPING = 0x1007;
		alSourcei(source, AL_LOOPING, looping ? 1 : 0);
	}

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
	{
		const int AL_PITCH = 0x1003;
		alSourcef(source, AL_PITCH, pitch);
	}

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
	{
		const int AL_POSITION = 0x1004;
		alSource3f(source, AL_POSITION, position.X, position.Y, position.Z);
	}

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
	{
		const int AL_SOURCE_RELATIVE = 0x202;
		alSourcei(source, AL_SOURCE_RELATIVE, relative ? 1 : 0);
	}

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
	{
		const int AL_SEC_OFFSET = 0x1024;
		alSourcef(source, AL_SEC_OFFSET, offset);
	}

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
	{
		const int AL_VELOCITY = 0x1006;
		alSource3f(source, AL_VELOCITY, velocity.X, velocity.Y, velocity.Z);
	}

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

	[LibraryImport(OpenALPath)]
	private static partial void alSourceStop(uint source);

	private static void stopSource(uint source) => alSourceStop(source);

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

	internal record UnqueueSourceBufferCommand(uint source, Promise2<uint> result) : AudioCommand;

	[LibraryImport(OpenALPath)]
	private static partial void alSourceUnqueueBuffers(uint source, int size, uint* buffers);

	private static uint unqueueSourceBuffer(uint source)
	{
		uint buffer;
		alSourceUnqueueBuffers(source, 1, &buffer);
		return buffer;
	}

	public ValuePromise<uint> UnqueueSourceBuffer(uint source)
	{
		if (IsAudioThread())
			return new ValuePromise<uint>(unqueueSourceBuffer(source));

		var promise = new Promise2<uint>();
		IssueCommand(new UnqueueSourceBufferCommand(source, promise));
		return new ValuePromise<uint>(promise);
	}

	#endregion
}
