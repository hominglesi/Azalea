using Azalea.Amends;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Platform;

namespace Azalea.Editing;
public static class Editor
{
	public static EditorContainer? Instance => GameHost.Main.EditorContainer;

	internal static void InspectTemplate(GameObject template)
	{
		if (Instance is null) return;

		Instance.Expanded.TemplateEditor.InspectObject(template);
	}

	internal static void FocusTemplateEditor()
	{
		if (Instance is null) return;

		Instance.Expanded.FocusTemplateEditor();
	}

	internal static void InspectObject(GameObject obj)
	{
		if (Instance is null) return;

		Instance.Expanded.LegacyInspector.SetObservedObject(obj);
	}

	internal static void HighlightObject(GameObject obj)
	{
		if (Instance is null) return;

		Instance.Expanded.Add(new HollowBox()
		{
			Depth = -5000,
			Position = obj.ScreenSpaceDrawQuad.TopLeft,
			Size = obj.ScreenSpaceDrawQuad.BottomRight - obj.ScreenSpaceDrawQuad.TopLeft,
		}.Wait(0.75f).Then().Execute(obj => Instance.Expanded.Remove(obj)));
	}

	public static void FocusConsole()
	{
		if (Instance is null) return;

		Input.ChangeFocus(Instance.Console);
	}

	public static void AddConsoleCommand(string keyword, EditorConsole.ConsoleCommandDelegate command)
	{
		if (Instance is null) return;

		Instance.Console.AddCommand(keyword, command);
	}

	public static void RemoveConsoleCommand(string keyword)
	{
		if (Instance is null) return;

		Instance.Console.RemoveCommand(keyword);
	}

	public static void ExecuteConsoleQuery(string query)
	{
		if (Instance is null) return;

		Instance.Console.ExecuteQuery(query);
	}
}
