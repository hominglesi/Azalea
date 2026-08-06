using System.Runtime.InteropServices;

namespace Azalea.Native.OpenAL;
public static partial class AL
{
	private const string SoftOalPath = "soft_oal.dll";

	public const int NONE = 0;
	public const int FALSE = 0;
	public const int TRUE = 1;
	public const int SOURCE_RELATIVE = 0x202;
	public const int CONE_INNER_ANGLE = 0x1001;
	public const int CONE_OUTER_ANGLE = 0x1002;
	public const int PITCH = 0x1003;
	public const int POSITION = 0x1004;
	public const int DIRECTION = 0x1005;
	public const int VELOCITY = 0x1006;
	public const int LOOPING = 0x1007;
	public const int BUFFER = 0x1009;
	public const int GAIN = 0x100A;
	public const int MIN_GAIN = 0x100D;
	public const int MAX_GAIN = 0x100E;
	public const int ORIENTATION = 0x100F;
	public const int SOURCE_STATE = 0x1010;
	public const int INITIAL = 0x1011;
	public const int PLAYING = 0x1012;
	public const int PAUSED = 0x1013;
	public const int STOPPED = 0x1014;
	public const int BUFFERS_QUEUED = 0x1015;
	public const int BUFFERS_PROCESSED = 0x1016;
	public const int REFERENCE_DISTANCE = 0x1020;
	public const int ROLLOFF_FACTOR = 0x1021;
	public const int CONE_OUTER_GAIN = 0x1022;
	public const int MAX_DISTANCE = 0x1023;
	public const int FORMAT_MONO8 = 0x1100;
	public const int FORMAT_MONO16 = 0x1101;
	public const int FORMAT_STEREO8 = 0x1102;
	public const int FORMAT_STEREO16 = 0x1103;
	public const int FREQUENCY = 0x2001;
	public const int SIZE = 0x2004;
	public const int NO_ERROR = 0;
	public const int INVALID_NAME = 0xA001;
	public const int INVALID_ENUM = 0xA002;
	public const int INVALID_VALUE = 0xA003;
	public const int INVALID_OPERATION = 0xA004;
	public const int OUT_OF_MEMORY = 0xA005;
	public const int VENDOR = 0xB001;
	public const int VERSION = 0xB002;
	public const int RENDERER = 0xB003;
	public const int EXTENSIONS = 0xB004;
	public const int DOPPLER_FACTOR = 0xC000;
	public const int DOPPLER_VELOCITY = 0xC001;
	public const int DISTANCE_MODEL = 0xD000;
	public const int INVERSE_DISTANCE = 0xD001;
	public const int INVERSE_DISTANCE_CLAMPED = 0xD002;
	public const int SEC_OFFSET = 0x1024;
	public const int SAMPLE_OFFSET = 0x1025;
	public const int BYTE_OFFSET = 0x1026;

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#albufferdata">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alBufferData")]
	public static partial void BufferData(uint buffer, int format, ref byte data, int size, int freq);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alDeleteBuffers">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alDeleteBuffers")]
	public static partial void DeleteBuffers(int n, ref uint buffers);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alDeleteSources">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alDeleteSources")]
	public static partial void DeleteSources(int n, ref uint sources);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alDistanceModel">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alDistanceModel")]
	public static partial void DistanceModel(int value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alGenBuffers">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alGenBuffers")]
	public static partial void GenBuffers(int n, ref uint buffers);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alGenSources">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alGenSources")]
	public static partial void GenSources(int n, ref uint sources);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alGenSources">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alGetError")]
	public static partial int GetError();

	/// <summary><see href="https://www.openal.org/documentation/OpenAL_Programmers_Guide.pdf">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alGetSourcef")]
	public static partial void GetSourcef(uint source, int param, ref float value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#algetsourcei">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alGetSourcei")]
	public static partial void GetSourcei(uint source, int param, ref int value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#algetsourcei">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alGetString")]
	public static partial nint GetString(int param);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#algetsourcei">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alListener3f")]
	public static partial void Listener3f(int param, float v1, float v2, float v3);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#algetsourcei">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alListenerf")]
	public static partial void Listenerf(int param, float value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSource3f">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSource3f")]
	public static partial void Source3f(uint source, int param, float v1, float v2, float v3);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourcef">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourcef")]
	public static partial void Sourcef(uint source, int param, float value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourcei">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourcei")]
	public static partial void Sourcei(uint source, int param, int value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourcePause">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourcePause")]
	public static partial void SourcePause(uint source);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourcePlay">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourcePlay")]
	public static partial void SourcePlay(uint source);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourceQueueBuffers">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourceQueueBuffers")]
	public static partial void SourceQueueBuffers(uint source, int n, ref uint buffers);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourceStop">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourceStop")]
	public static partial void SourceStop(uint source);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alSourceUnqueueBuffers">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alSourceUnqueueBuffers")]
	public static partial void SourceUnqueueBuffers(uint source, int n, ref uint buffers);
}
