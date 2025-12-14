using Azalea.Design.Containers;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azalea.Design.Explorer;
internal abstract class ResourceExplorer : FlexContainer
{
	private IResourceStore? _store;
	public IResourceStore? Store
	{
		get => _store;
		set
		{
			if (_store == value) return;

			_store = value;

			ShowDirectory();
		}
	}

	public Action<string>? SubPathChanged;

	public string SubPath { get; private set; } = "";

	public void ShowDirectory(string subPath = "")
	{
		Debug.Assert(_store is not null);

		SubPath = subPath;

		ClearItems();

		if (subPath != "")
		{
			var returnItem = AddReturn();
			returnItem.ItemSelected += (_, _) => goBackwards();
		}

		List<string> directories = new();
		List<string> files = new();

		foreach (var (resourcePath, isDirectory) in _store.GetAvalibleResources(subPath))
		{
			if (isDirectory)
				directories.Add(resourcePath);
			else
				files.Add(resourcePath);
		}

		foreach (var directory in directories)
		{
			var item = AddItem(directory, true);
			item.ItemSelected += itemSelected;
		}

		foreach (var file in files)
		{
			var item = AddItem(file, false);
			item.ItemSelected += itemSelected;
		}

		PerformLayout();
		SubPathChanged?.Invoke(SubPath);
	}

	protected abstract ResourceItem AddItem(string path, bool isDirectory);
	protected abstract ResourceItem AddReturn();

	public Action<string, bool>? ItemSelected;

	private void itemSelected(string path, bool isDirectory)
	{
		var fullPath = PathUtils.CombinePaths(SubPath, path);

		if (isDirectory)
			ShowDirectory(fullPath + "/");

		ItemSelected?.Invoke(fullPath, isDirectory);
	}

	private void goBackwards()
	{
		var dirCount = SubPath.Count(c => c == '/');
		if (dirCount <= 1)
		{
			ShowDirectory();
			return;
		}

		var targetSlash = SubPath[0..^1].LastIndexOf('/') + 1;
		ShowDirectory(SubPath[0..targetSlash]);
	}

	protected abstract void ClearItems();

	protected abstract class ResourceItem : Composition
	{
		public Action<string, bool>? ItemSelected { get; set; }
		public string Path { get; init; }
		public bool IsDirectory { get; init; }

		protected ResourceItem(string path, bool isDirectory)
		{
			Path = path;
			IsDirectory = isDirectory;
		}

		protected void InvokeClicked()
			=> ItemSelected?.Invoke(Path, IsDirectory);
	}
}
