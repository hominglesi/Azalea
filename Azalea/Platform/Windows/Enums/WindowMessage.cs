namespace Azalea.Platform.Windows;

internal enum WindowMessage : uint
{
	/// <summary>
	/// Sent after a window has been moved.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	Move = 3,
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
	Paint = 15,
	/// <summary>
	/// Sent as a signal that a window or an application should terminate.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	Close = 16,
	/// <summary>
	/// Sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured.
	/// </summary>
	SetCursor = 32,
	/// <summary>
	/// Sent to a window whose size, position, or place in the Z order is about to change
	/// as a result of a call to the <see cref="WinAPI.SetWindowPos"/> function or another window-management function.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	WindowPosChanging = 70,
	/// <summary>
	/// Associates a new large or small icon with a window.
	/// The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption.
	/// </summary>
	SetIcon = 128,
	/// <summary>
	/// Sent when the size and position of a window's client area must be calculated.
	/// By processing this message, an application can control the content of
	/// the window's client area when the size or position of the window changes.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	NCCalcSize = 131,
	/// <summary>
	/// Sent to a window in order to determine what part of the window corresponds to a particular screen coordinate.
	/// This can happen, for example, when the cursor moves, when a mouse button is pressed or released,
	/// or in response to a call to a function such as <see cref="WinAPI.WindowFromPoint"/>. If the mouse is not captured,
	/// the message is sent to the window beneath the cursor. Otherwise, the message is sent to the window
	/// that has captured the mouse. A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	NCHitTest = 132,
	/// <summary>
	/// The <see cref="SyncPaint"/> message is used to synchronize painting while avoiding linking independent GUI threads.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	SyncPaint = 136,
	/// <summary>
	/// Posted when the user releases the left mouse button while the cursor is within the nonclient area of a window.
	/// This message is posted to the window that contains the cursor.
	/// If a window has captured the mouse, this message is not posted.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	NonclientLeftButtonUp = 162,
	/// <summary>
	/// Sent to the window that registered to receive raw input.
	/// Raw input notifications are available only after the application calls
	/// <see cref="WinAPI.RegisterRawInputDevices"/> with <see cref="RawInputDeviceFlags.DevNotify"/> flag.
	/// A window receives this message through its WindowProc function.
	/// </summary>
	InputDeviceChange = 254,
	/// <summary>
	/// Sent to the window that is getting raw input.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	Input = 255,
	/// <summary>
	/// Posted to the window with the keyboard focus when a nonsystem key is pressed.
	/// A nonsystem key is a key that is pressed when the ALT key is not pressed.
	/// </summary>
	KeyDown = 256,
	/// <summary>
	/// Posted to the window with the keyboard focus when a nonsystem key is released.
	/// A nonsystem key is a key that is pressed when the ALT key is not pressed,
	/// or a keyboard key that is pressed when a window has the keyboard focus.
	/// </summary>
	KeyUp = 257,
	/// <summary>
	/// Posted to the window with the keyboard focus when a <see cref="KeyDown"/> message is translated
	/// by the <see cref="WinAPI.TranslateMessage"/> function.
	/// The <see cref="Char"/> message contains the character code of the key that was pressed.
	/// </summary>
	Char = 258,
	/// <summary>
	/// Posted to the window with the keyboard focus when the user presses the F10 key (which activates
	/// the menu bar) or holds down the ALT key and then presses another key. It also occurs when no
	/// window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the
	/// active window. The window that receives the message can distinguish between these two contexts
	/// by checking the context code in the lParam parameter.
	/// </summary>
	SysKeyDown = 260,
	/// <summary>
	/// Posted to the window with the keyboard focus when the user releases a key that was pressed
	/// while the ALT key was held down. It also occurs when no window currently has the keyboard
	/// focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that
	/// receives the message can distinguish between these two contexts by checking the context code
	/// in the lParam parameter.
	/// </summary>
	SysKeyUp = 261,
	/// <summary>
	/// A window receives this message when the user chooses a command from the Window menu
	/// (formerly known as the system or control menu) or when the user chooses the maximize button, minimize button,
	/// restore button, or close button.
	/// </summary>
	SysCommand = 274,
	/// <summary>
	/// Posted to a window when the cursor moves. If the mouse is not captured, the message is posted
	/// to the window that contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	MouseMove = 512,
	/// <summary>
	/// Posted when the user presses the left mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	LeftButtonDown = 513,
	/// <summary>
	/// Posted when the user releases the left mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	LeftButtonUp = 514,
	/// <summary>
	/// Posted when the user double-clicks the left mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	LeftButtonDoubleClick = 515,
	/// <summary>
	/// Posted when the user presses the right mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	RightButtonDown = 516,
	/// <summary>
	/// Posted when the user releases the right mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	RightButtonUp = 517,
	/// <summary>
	/// Posted when the user double-clicks the right mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	RightButtonDoubleClick = 518,
	/// <summary>
	/// Posted when the user presses the middle mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	MiddleButtonDown = 519,
	/// <summary>
	/// Posted when the user releases the middle mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	MiddleButtonUp = 520,
	/// <summary>
	/// Posted when the user double-clicks the middle mouse button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	MiddleButtonDoubleClick = 521,
	/// <summary>
	/// Sent to the focus window when the mouse wheel is rotated.
	/// The <see cref="WinAPI.DefWindowProc"/> function propagates the message to the window's parent.
	/// There should be no internal forwarding of the message, since <see cref="WinAPI.DefWindowProc"/>
	/// propagates it up the parent chain until it finds a window that processes it.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	MouseWheel = 522,
	/// <summary>
	/// Posted when the user presses the first or second X button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	XButtonDown = 523,
	/// <summary>
	/// Posted when the user releases the first or second X button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	XButtonUp = 524,
	/// <summary>
	/// Posted when the user double-clicks the first or second X button while the cursor is in the client area of a window.
	/// If the mouse is not captured, the message is posted to the window beneath the cursor.
	/// Otherwise, the message is posted to the window that has captured the mouse.
	/// A window receives this message through its <see cref="WindowProcedure"/> function.
	/// </summary>
	XButtonDoubleClick = 525,
	/// <summary>
	/// Sent when the user drops a file on the window of an application that
	/// has registered itself as a recipient of dropped files.
	/// </summary>
	DropFiles = 563
}
