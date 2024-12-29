using System;
using System.IO;

namespace Azalea.Platform;
public static class Window
{
	private static IWindow? _instance;
	public static IWindow Instance => _instance ??= GameHost.Main.Window;

	public static Vector2Int Size
	{
		get => Instance.Size;
		set => Instance.Size = value;
	}

	public static Vector2Int ClientSize
	{
		get => Instance.ClientSize;
		set => Instance.ClientSize = value;
	}

	public static Vector2Int Position
	{
		get => Instance.Position;
		set => Instance.Position = value;
	}

	public static Vector2Int ClientPosition
	{
		get => Instance.ClientPosition;
		set => Instance.ClientPosition = value;
	}

	public static WindowState State
	{
		get => Instance.State;
		set => Instance.State = value;
	}

	public static string Title
	{
		get => Instance.Title;
		set => Instance.Title = value;
	}

	public static bool Resizable
	{
		get => Instance.Resizable;
		set => Instance.Resizable = value;
	}

	public static bool VSync
	{
		get => Instance.VSync;
		set => Instance.VSync = value;
	}

	public static bool CanChangeVSync
		=> Instance.CanChangeVSync;

	public static bool CursorVisible
	{
		get => Instance.CursorVisible;
		set => Instance.CursorVisible = value;
	}

	public static void Center() => Instance.Center();
	public static void Focus() => Instance.Focus();
	public static void RequestAttention() => Instance.RequestAttention();
	public static void SetIconFromStream(Stream? imageStream) => Instance.SetIconFromStream(imageStream);

	public static Action? Closing
	{
		get => Instance.Closing;
		set => Instance.Closing = value;
	}

	public static void Close() => Instance.Close();
	public static void PreventClosure() => Instance.PreventClosure();
}
