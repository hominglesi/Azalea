using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal partial class WindowsWaitableTimer : Disposable
{
	private readonly nint _timer;
	private readonly long _interval;

	public WindowsWaitableTimer(int milisecondInterval)
	{
		// Timer interval is in 100ns units
		// Negative value means that the value is relative to current time
		_interval = milisecondInterval * -10_000L;

		_timer = createWaitableTimerExW(IntPtr.Zero, null,
			TimerFlags.ManualReset | TimerFlags.HighResolution, TimerAccess.All);

		// If high resolution is not supported we fallback to a regular timer
		if (_timer == IntPtr.Zero)
		{
			_timer = createWaitableTimerExW(IntPtr.Zero, null,
			TimerFlags.ManualReset, TimerAccess.All);
		}

		if (_timer == IntPtr.Zero)
			throw new Exception("Could not create waitable timer!");
	}

	public void Start()
	{
		long dueTime = _interval;
		setWaitableTimer(_timer, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false);
	}

	public void Wait()
	{
		waitForSingleObject(_timer, uint.MaxValue);

		long dueTime = _interval;
		setWaitableTimer(_timer, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false);
	}

	protected override void OnDispose()
	{
		closeHandle(_timer);
	}

	enum TimerFlags : uint
	{
		ManualReset = 0x00000001,
		HighResolution = 0x00000002
	}

	enum TimerAccess : uint
	{
		All = 0x1F0003
	}

	[LibraryImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	private static partial bool closeHandle(IntPtr hObject);

	[LibraryImport("kernel32.dll", EntryPoint = "CreateWaitableTimerExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
	private static partial IntPtr createWaitableTimerExW(IntPtr lpTimerAttributes,
		string? lpTimerName, TimerFlags dwFlags, TimerAccess dwDesiredAccess);

	[LibraryImport("kernel32.dll", EntryPoint = "SetWaitableTimer", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	private static partial bool setWaitableTimer(IntPtr hTimer, ref long pDueTime,
		int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine,
		[MarshalAs(UnmanagedType.U1)] bool fResume);

	[LibraryImport("kernel32.dll", EntryPoint = "WaitForSingleObject", SetLastError = true)]
	private static partial uint waitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
}
