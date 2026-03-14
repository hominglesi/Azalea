using Azalea.Sounds.OpenAL.Enums;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Sounds.OpenAL;
internal unsafe static class ALC
{
	public const string LibraryPath = "soft_oal.dll";

	#region General

	[DllImport(LibraryPath, EntryPoint = "alGetString")]
	private static extern IntPtr getString(int param);

	public static string GetString(int param)
	{
		var ptr = getString(param);
		return Marshal.PtrToStringAnsi(ptr)!;
	}

	public static string GetVersion()
	{
		var ptr = getString(0xB002 /* AL_VERSION*/);
		return Marshal.PtrToStringAnsi(ptr)!;
	}

	public static string GetRenderer()
	{
		var ptr = getString(0xB007 /* AL_RENDERER */);
		return Marshal.PtrToStringAnsi(ptr)!;
	}

	[DllImport(LibraryPath, EntryPoint = "alDistanceModel")]
	public static extern void DistanceModel(int distanceModel);

	#endregion

	#region Context Lifetime
	[DllImport(LibraryPath, EntryPoint = "alcCreateContext")]
	private static extern ALC_Context createContext(IntPtr device, int* attributeList);

	public static ALC_Context CreateContext(IntPtr device, int[] attributeList)
	{
		fixed (int* ptr = attributeList)
		{
			return createContext(device, ptr);
		}
	}

	[DllImport(LibraryPath, EntryPoint = "alcMakeContextCurrent")]
	public static extern bool MakeContextCurrent(ALC_Context context);
	#endregion

	#region Buffers
	[DllImport(LibraryPath, EntryPoint = "alGenBuffers")]
	private static extern void genBuffers(int count, uint* buffers);

	public static uint[] GenBuffers(int count)
	{
		uint[] buffers = new uint[count];
		fixed (uint* p = buffers)
			genBuffers(count, p);

		return buffers;
	}

	public static uint GenBuffer()
	{
		return GenBuffers(1)[0];
	}

	[DllImport(LibraryPath, EntryPoint = "alDeleteBuffers")]
	private static extern void deleteBuffers(int count, uint* buffers);

	public static void DeleteBuffer(uint buffer)
	{
		deleteBuffers(1, &buffer);
	}

	[DllImport(LibraryPath, EntryPoint = "alBufferData")]
	private static extern void bufferData(uint buffer, ALFormat format, void* data, int size, int frequency);
	public static void BufferData(uint buffer, ALFormat format, ReadOnlySpan<byte> data, int size, int frequency)
	{
		fixed (void* p = data)
		{
			bufferData(buffer, format, p, size, frequency);
		}
	}

	#endregion

	#region Sources

	[DllImport(LibraryPath, EntryPoint = "alGenSources")]
	private static extern void genSources(int count, uint* sources);

	public static uint[] GenSources(int count)
	{
		uint[] sources = new uint[count];
		fixed (uint* p = sources)
			genSources(count, p);

		return sources;
	}

	public static uint GenSource()
	{
		return GenSources(1)[0];
	}

	[DllImport(LibraryPath, EntryPoint = "alDeleteSources")]
	private static extern void deleteSources(int count, uint* sources);

	public static void DeleteSource(uint source)
	{
		deleteSources(1, &source);
	}

	[DllImport(LibraryPath, EntryPoint = "alSourcei")]
	private static extern void sourcei(uint source, int param, int value);

	[DllImport(LibraryPath, EntryPoint = "alGetSourcei")]
	private static extern void getSourcei(uint source, int param, [Out] int* value);

	public static void BindBuffer(uint source, uint buffer)
	{
		//AL_BUFFER = 0x1009
		sourcei(source, 0x1009, (int)buffer);
	}

	[DllImport(LibraryPath, EntryPoint = "alSourcePause")]
	public static extern void SourcePause(uint source);

	[DllImport(LibraryPath, EntryPoint = "alSourcePlay")]
	public static extern void SourcePlay(uint source);

	[DllImport(LibraryPath, EntryPoint = "alSourceStop")]
	public static extern void SourceStop(uint source);

	[DllImport(LibraryPath, EntryPoint = "alSourcef")]
	private static extern void sourcef(uint source, int param, float value);

	public static void SetSourceGain(uint source, float gain)
	{
		//0x100A = AL_GAIN
		sourcef(source, 0x100A, gain);
	}

	public static void SetSourcePitch(uint source, float pitch)
	{
		//0x1003 = AL_PITCH
		sourcef(source, 0x1003, pitch);
	}

	public static void SetSourceLooping(uint source, bool looping)
	{
		//0x1007 = AL_LOOPING
		sourcei(source, 0x1007, looping ? 1 : 0);
	}

	public static void SetSecOffset(uint source, float offset)
	{
		//0x1024 = AL_SEC_OFFSET
		sourcef(source, 0x1024, offset);
	}

	public static void SetSourceRelative(uint source, bool relative)
	{
		//0x202 = AL_SOURCE_RELATIVE
		sourcei(source, 0x202, relative ? 1 : 0);
	}

	[DllImport(LibraryPath, EntryPoint = "alSource3f")]
	private static extern void source3f(uint source, int param, float value1, float value2, float value3);

	public static void SetSourcePosition(uint source, Vector3 position)
	{
		//0x1004 = AL_POSITION
		source3f(source, 0x1004, position.X, position.Y, position.Z);
	}

	public static void SetSourceVelocity(uint source, Vector3 velocity)
	{
		//0x1006 = AL_VELOCITY
		source3f(source, 0x1006, velocity.X, velocity.Y, velocity.Z);
	}

	[DllImport(LibraryPath, EntryPoint = "alGetSourcef")]
	private static extern void getSourcef(uint source, int param, [Out] float* value);
	public static int GetBuffersProcessed(uint source)
	{
		//0x1016 = AL_BUFFERS_PROCESSED
		int buffersProcessed;
		getSourcei(source, 0x1016, &buffersProcessed);
		return buffersProcessed;
	}

	public static int GetBuffersQueued(uint source)
	{
		//0x1015 = AL_BUFFERS_QUEUED
		int buffersProcessed;
		getSourcei(source, 0x1015, &buffersProcessed);
		return buffersProcessed;
	}

	public static ALSourceState GetSourceState(uint source)
	{
		//0x1010 = AL_SOURCE_STATE
		int sourceState;
		getSourcei(source, 0x1010, &sourceState);
		return (ALSourceState)sourceState;
	}

	public static float GetSecOffset(uint source)
	{
		//0x1024 = AL_SEC_OFFSET
		float secOffset;
		getSourcef(source, 0x1024, &secOffset);
		return secOffset;
	}

	[DllImport(LibraryPath, EntryPoint = "alSourceQueueBuffers")]
	private static extern void sourceQueueBuffers(uint source, int size, uint* buffers);

	public static void SourceQueueBuffer(uint source, uint buffer)
	{
		sourceQueueBuffers(source, 1, &buffer);
	}

	[DllImport(LibraryPath, EntryPoint = "alSourceUnqueueBuffers")]
	private static extern void sourceUnqueueBuffers(uint source, int size, uint* buffers);

	public static uint SourceUnqueueBuffer(uint source)
	{
		uint buffer;
		sourceUnqueueBuffers(source, 1, &buffer);
		return buffer;
	}

	#endregion

	#region Listeners

	[DllImport(LibraryPath, EntryPoint = "alListenerf")]
	private static extern void listenerf(int param, float value);

	[DllImport(LibraryPath, EntryPoint = "alListener3f")]
	private static extern void listener3f(int param, float value1, float value2, float value3);

	public static void SetListenerGain(float gain)
	{
		//0x100A = AL_GAIN
		listenerf(0x100A, gain);
	}
	public static void SetListenerPosition(Vector3 position)
	{
		//0x1004 = AL_POSITION
		listener3f(0x1004, position.X, position.Y, position.Z);
	}

	public static void SetListenerVelocity(Vector3 velocity)
	{
		//0x1006 = AL_VELOCITY
		listener3f(0x1006, velocity.X, velocity.Y, velocity.Z);
	}

	#endregion


	#region Errors
	[DllImport(LibraryPath, EntryPoint = "alGetError")]
	public static extern ALError GetError();

	public static void PrintErrors()
	{
		var error = ALC.GetError();
		while (error != ALError.NoError)
		{
			Console.WriteLine("OpenAL Error: " + error);
			error = ALC.GetError();
		}

	}
	#endregion
}
