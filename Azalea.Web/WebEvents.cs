using Azalea.Inputs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace Azalea.Web;

internal static partial class WebEvents
{
	private const string ImportString = "JSImports";

	internal static Vector2Int ClientSize;
	internal static Action<Vector2Int>? OnClientResized;

	[JSImport("WebEvents.CheckClientSize", ImportString)]
	internal static partial void CheckClientSize();

	[JSExport]
	internal static void UpdateClientSize(int width, int height)
	{
		ClientSize = new Vector2Int(width, height);
		OnClientResized?.Invoke(ClientSize);
	}

	internal static Action? OnAnimationFrameRequested;

	[JSImport("WebEvents.RequestAnimationFrame", "JSImports")]
	internal static partial void RequestAnimationFrame();

	[JSExport]
	internal static void InvokeAnimationFrameRequested()
		=> OnAnimationFrameRequested?.Invoke();

	[JSImport("WebEvents.GetCurrentPreciseTime", "JSImports")]
	[return: JSMarshalAs<JSType.Date>]
	internal static partial DateTime GetCurrentPreciseTime();

	private static Vector2Int? _mouseMoveChange;
	private static List<MouseButton> _mouseDownButtons = new();
	private static List<MouseButton> _mouseUpButtons = new();
	private static List<Keys> _downKeys = new();
	private static List<Keys> _upKeys = new();

	[JSExport]
	internal static void ReportMouseMove(int x, int y)
		=> _mouseMoveChange = new Vector2Int(x, y);

	[JSExport]
	internal static void ReportMouseDown(int button)
		=> _mouseDownButtons.Add(WebUtils.TranslateMouseButton(button));

	[JSExport]
	internal static void ReportMouseUp(int button)
		=> _mouseUpButtons.Add(WebUtils.TranslateMouseButton(button));

	[JSExport]
	internal static void ReportKeyDown(string key)
		=> _downKeys.Add(WebUtils.TranslateKey(key));

	[JSExport]
	internal static void ReportKeyUp(string key)
		=> _upKeys.Add(WebUtils.TranslateKey(key));

	internal static void HandleEvents()
	{
		if (_mouseMoveChange != null)
		{
			Input.ExecuteMousePositionChange(_mouseMoveChange.Value);
			_mouseMoveChange = null;
		}

		if (_mouseDownButtons.Count > 0)
		{
			foreach (var mouseDownButton in _mouseDownButtons)
				Input.ExecuteMouseButtonStateChange(mouseDownButton, true);

			_mouseDownButtons.Clear();
		}

		if (_mouseUpButtons.Count > 0)
		{
			foreach (var mouseUpButton in _mouseUpButtons)
				Input.ExecuteMouseButtonStateChange(mouseUpButton, false);

			_mouseUpButtons.Clear();
		}

		if (_downKeys.Count > 0)
		{
			foreach (var downKey in _downKeys)
				Input.ExecuteKeyboardKeyStateChange(downKey, true);

			_downKeys.Clear();
		}

		if (_upKeys.Count > 0)
		{
			foreach (var upKey in _upKeys)
				Input.ExecuteKeyboardKeyStateChange(upKey, false);

			_upKeys.Clear();
		}
	}
}
