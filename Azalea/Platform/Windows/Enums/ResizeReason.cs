namespace Azalea.Platform.Windows;
internal enum ResizeReason
{
	/// <summary>
	/// The window has been maximized.
	/// </summary>
	Maximized = 2,
	/// <summary>
	/// The window has been minimized.
	/// </summary>
	Minimized = 1,
	/// <summary>
	/// The window has been resized,
	/// but neither the <see cref="Minimized"/> nor <see cref="Maximized"/> value applies.
	/// </summary>
	Restored = 0,
}
