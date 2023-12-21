using Azalea.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Azalea.Debugging;
public static class PerformanceTrace
{
	public static bool Enabled { get; set; } = false;

	private static readonly long _startTime;
	private static readonly List<TraceEvent> _events;

	static PerformanceTrace()
	{
		_startTime = Time.GetCurrentPreciseTime().Ticks;
		_events = new List<TraceEvent>();
	}

	public static long StartEvent() => getCurrentMs();

	public static void AddEvent(long startTime, string name)
	{
		if (Enabled == false) return;

		var currentTime = getCurrentMs();

		var duration = currentTime - startTime;

		var Event = new TraceEvent(startTime, duration, name);
		_events.Add(Event);
	}

	private static long _lastCurrenMs = -1;
	private static long getCurrentMs()
	{
		if (Enabled == false) return 0;

		var currentMs = Time.GetCurrentPreciseTime().Ticks - _startTime;

		if (currentMs == _lastCurrenMs)
			currentMs++;

		_lastCurrenMs = currentMs;
		return currentMs;
	}

	public static void RunAndTrace(Action action, string name)
	{
		if (Enabled == false)
		{
			action.Invoke();
			return;
		}

		var start = StartEvent();
		action.Invoke();
		AddEvent(start, name);
	}


	public static void SaveEventsTo(string path)
	{
		if (Enabled == false) return;

		var json = JsonSerializer.Serialize(_events);
		File.WriteAllText(path, json);
	}

	private struct TraceEvent
	{
		public string name { get; set; } = "Not-named";
		public string cat { get; set; } = "function";
		public string ph { get; set; } = "X";
		public int pid { get; set; } = 0;
		public int tid { get; set; } = 0;
		public long ts { get; set; } = 0;
		public long dur { get; set; } = 0;

		public TraceEvent(long startTime, long duration, string name)
		{
			ts = startTime;
			dur = duration;
			this.name = name;
		}
	}
}
