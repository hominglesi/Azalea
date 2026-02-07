using Azalea.Design.Containers;
using Azalea.Inputs.Events;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Design.Controls;
public abstract class AbstractFileInput : Composition
{
	public AcceptedFileFlags AcceptedFiles { get; set; } = AcceptedFileFlags.File;

	protected List<string> SelectedFilePaths = [];

	public string? SelectedPath
	{
		get
		{
			if (SelectedFilePaths.Count > 0)
				return SelectedFilePaths[0];

			return null;
		}
	}

	public IEnumerable<string> SelectedPaths
	{
		get
		{
			foreach (var filePath in SelectedFilePaths)
				yield return filePath;
		}
	}

	public override bool AcceptsFiles => true;
	protected override bool OnFileDropped(FileDroppedEvent e)
	{
		SelectedFilePaths.Clear();

		if (e.FilePaths.Length > 0)
		{
			for (int i = 0; i < e.FilePaths.Length; i++)
			{
				var path = e.FilePaths[i];

				if (File.Exists(path))
				{
					if (AcceptedFiles.HasFlag(AcceptedFileFlags.MultipleFiles) ||
						(AcceptedFiles.HasFlag(AcceptedFileFlags.File) && i == 0))
					{
						SelectedFilePaths.Add(path);
					}
				}
				else if (Directory.Exists(path))
				{
					if (AcceptedFiles.HasFlag(AcceptedFileFlags.MultipleDirectories) ||
						(AcceptedFiles.HasFlag(AcceptedFileFlags.Directory) && i == 0))
					{
						SelectedFilePaths.Add(path);
					}
				}
			}
		}

		return true;
	}
}

[Flags]
public enum AcceptedFileFlags
{
	File = 1,
	MultipleFiles = 2,
	Directory = 4,
	MultipleDirectories = 8
}
