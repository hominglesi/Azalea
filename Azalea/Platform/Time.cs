using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform;

public static class Time
{
	private const float FpsInterval = 0.3f;

	private static float _deltaTime;
	private static int _framesSinceStart;
	private static float _timeSinceStart;

	private static float _nextFpsInterval = FpsInterval;
	private static int _lastFpsCount;
	private static float _framesSinceFpsInterval;
	private static float _timeSinceFpsInterval;

	public static float DeltaTime => _deltaTime;
	public static float DeltaTimeMs => _deltaTime * 1000;
	public static float FramesSinceStart => _framesSinceStart;
	public static float TimeSinceStart => _timeSinceStart;
	public static int FpsCount => _lastFpsCount;

	internal static void Update(float deltaTime)
	{
		_deltaTime = deltaTime;

		_framesSinceStart++;
		_framesSinceFpsInterval++;
		_timeSinceStart += deltaTime;
		_timeSinceFpsInterval += deltaTime;

		if (_timeSinceStart > _nextFpsInterval)
		{
			_lastFpsCount = (int)Math.Floor(_framesSinceFpsInterval / _timeSinceFpsInterval);

			_framesSinceFpsInterval = 0;
			_timeSinceFpsInterval = 0;
			_nextFpsInterval += FpsInterval;
		}
	}

	#region Precise Time

	[DllImport("Kernel32.dll", EntryPoint = "GetSystemTimePreciseAsFileTime", CallingConvention = CallingConvention.Winapi)]
	private static extern void getSystemTimePreciseAsFileTime(out long filetime);

	public static DateTime GetCurrentExactTime()
	{
		getSystemTimePreciseAsFileTime(out long filetime);

		return DateTime.FromFileTimeUtc(filetime);
	}

	#endregion
}
