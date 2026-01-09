namespace Azalea.Inputs.Events;
public class FileDroppedEvent(string[] filePaths) : InputEvent
{
	public readonly string[] FilePaths = filePaths;
}
