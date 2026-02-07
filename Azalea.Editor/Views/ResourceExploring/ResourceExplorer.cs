using Azalea.Design.Containers;
using Azalea.IO.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azalea.Editor.Views.ResourceExploring;
public abstract class ResourceExplorer : Composition
{
	private readonly IResourceStore _store;
	private readonly List<string> _history = [];
	private int _historyIndex = -1;

	public string DisplayedDirectory { get; private set; }
	public Action<string>? DisplayDirectoryChanged;

	public ResourceExplorer(IResourceStore store, string displayedDirectory = "")
	{
		Debug.Assert(store is not null);
		_store = store;

		SetDisplayedDirectory(displayedDirectory);
		WriteToHistory(displayedDirectory);
	}

	public void SyncData(ResourceExplorer other)
	{
		_history.Clear();
		foreach (var item in other._history)
			_history.Add(item);

		_historyIndex = other._historyIndex;
		SetDisplayedDirectory(other.DisplayedDirectory);
	}

	protected abstract void AddResourceItem(string path);
	protected abstract void ClearResourceItems();

	[MemberNotNull(nameof(DisplayedDirectory))]
	public void SetDisplayedDirectory(string path)
	{
		if (path == DisplayedDirectory)
			return;
		ClearResourceItems();

		foreach (var item in _store.GetItems(path))
			AddResourceItem(item);

		DisplayedDirectory = path;
		DisplayDirectoryChanged?.Invoke(path);
	}

	public void MoveBackward()
	{
		if (_historyIndex == 0)
			return;

		_historyIndex--;

		SetDisplayedDirectory(_history[_historyIndex]);
	}

	public void MoveForward()
	{
		if (_historyIndex == _history.Count - 1)
			return;

		_historyIndex++;

		SetDisplayedDirectory(_history[_historyIndex]);
	}

	public void WriteToHistory(string path)
	{
		_historyIndex++;

		while (_historyIndex < _history.Count)
			_history.RemoveAt(_historyIndex);

		_history.Add(path);
	}
}
