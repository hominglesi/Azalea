using Azalea.Inputs;
using System;

namespace Azalea.Platform;
public interface ITrayIcon
{
	Action<MouseButton>? OnClick { get; set; }
	Action<MouseButton>? OnDoubleClick { get; set; }

	void Destroy();
}
