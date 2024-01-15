namespace Azalea.Platform.Windows;

internal enum WindowMessage : uint
{
	/// <summary>
	/// Sent to a window after its size has changed.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	Size = 5,

	/// <summary>
	/// The <see cref="Paint"/> message is sent when the system or another application makes a request to
	/// paint a portion of an application's window. The message is sent when the <see cref="WinAPI.UpdateWindow"/>
	/// or <see cref="WinAPI.RedrawWindow"/> function is called, or by the DispatchMessage function when the
	/// application obtains a <see cref="WindowMessage.Paint"/> message by using the <see cref="WinAPI.GetMessage"/> or PeekMessage function.
	/// </summary>
	Paint = 15
}
