using Azalea.Amends;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Platform;
using System;

namespace Azalea.Editing;
public static class Editor
{
	private static EditorContainer? _instance;
	public static EditorContainer Instance => _instance
		??= GameHost.Main.EditorContainer
		?? throw new Exception("Editor has not been enabled!");

	internal static void InspectTemplate(GameObject template)
		=> Instance.Expanded.TemplateEditor.InspectObject(template);

	internal static void FocusTemplateEditor()
		=> Instance.Expanded.FocusTemplateEditor();

	internal static void InspectObject(GameObject obj)
		=> Instance.Expanded.LegacyInspector.SetObservedObject(obj);

	internal static void HighlightObject(GameObject obj)
	{
		Instance.Expanded.Add(new HollowBox()
		{
			Depth = -5000,
			Position = obj.ScreenSpaceDrawQuad.TopLeft,
			Size = obj.ScreenSpaceDrawQuad.BottomRight - obj.ScreenSpaceDrawQuad.TopLeft,
		}.Wait(0.75f).Then().Execute(obj => Instance.Expanded.Remove(obj)));
	}

	public static void FocusConsole()
		=> Input.ChangeFocus(Instance.Console);

	public static void AddConsoleCommand(string keyword, EditorConsole.ConsoleCommandDelegate command)
		=> Instance.Console.AddCommand(keyword, command);

	public static void RemoveConsoleCommand(string keyword)
		=> Instance.Console.RemoveCommand(keyword);

	public static void ExecuteConsoleQuery(string query)
		=> Instance.Console.ExecuteQuery(query);
}
