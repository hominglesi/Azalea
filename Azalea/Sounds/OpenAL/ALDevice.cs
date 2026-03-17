using Azalea.Utils;
using System;

namespace Azalea.Sounds.OpenAL;
internal unsafe partial class ALDevice : Disposable
{
	public readonly ALAudioManager _audioManager;
	public readonly IntPtr Handle;

	public ALDevice(ALAudioManager audioManager, IntPtr handle)
	{
		_audioManager = audioManager;
		Handle = handle;
	}

	public void Reopen(string? newDeviceName, int[] attibutes)
		=> _audioManager.ReopenDevice(Handle, newDeviceName, attibutes);
	public bool IsConnected() => _audioManager.GetDeviceConnected(Handle);
	public int GetFrequency() => _audioManager.GetDeviceFrequency(Handle);
	public bool GetHRTF() => _audioManager.GetDeviceHRTF(Handle);
	public string GetDeviceName() => _audioManager.GetDeviceName(Handle);

	protected override void OnDispose() => _audioManager.CloseDevice(Handle);
}
