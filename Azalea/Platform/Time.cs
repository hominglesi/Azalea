using System;
using System.Diagnostics;

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

	private static DateTime _lastFrameTimestamp;
	internal static void Setup()
	{
		_lastFrameTimestamp = GetCurrentPreciseTime();
	}

	internal static void UpdateDeltaTime()
	{
		var frameTimestamp = GetCurrentPreciseTime();
		_deltaTime = (float)frameTimestamp.Subtract(_lastFrameTimestamp).TotalSeconds;
		_lastFrameTimestamp = frameTimestamp;

		_framesSinceStart++;
		_framesSinceFpsInterval++;
		_timeSinceStart += _deltaTime;
		_timeSinceFpsInterval += _deltaTime;

		if (_timeSinceStart > _nextFpsInterval)
		{
			_lastFpsCount = (int)Math.Floor(_framesSinceFpsInterval / _timeSinceFpsInterval);

			_framesSinceFpsInterval = 0;
			_timeSinceFpsInterval = 0;
			_nextFpsInterval += FpsInterval;
		}
	}

	#region Precise Time

	private static long getTicksSinceStartup() => Stopwatch.GetTimestamp();
	private static long getCurrentTicks() => _startTime + getTicksSinceStartup();

	private static long _startTime;
	static Time()
	{
		_startTime = DateTime.UtcNow.Ticks - getTicksSinceStartup();
	}

	public static DateTime GetCurrentPreciseTime()
	{
		return DateTime.FromFileTimeUtc(getCurrentTicks());
	}

	public static float GetPreciseMilisecondsSince(DateTime time)
	{
		return (float)GetCurrentPreciseTime().Subtract(time).TotalMilliseconds;
	}

	#endregion
}
