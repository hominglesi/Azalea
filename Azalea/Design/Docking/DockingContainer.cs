﻿using Azalea.Design.Containers;
using Azalea.Graphics;
using System;
using System.Collections.Generic;

namespace Azalea.Design.Docking;
public abstract class DockingContainer : ContentContainer
{
	protected List<Dockable> Dockables = new();
	protected Dockable? FocusedDockable;

	public void AddDockable(string name, GameObject content)
	{
		var dockable = new Dockable(name, content);

		Dockables.Add(dockable);

		UpdateDockablesNavigation();

		if (Dockables.Count == 1)
			FocusDockable(dockable);
	}

	public void ClearDockables()
	{
		Dockables.Clear();

		UpdateDockablesNavigation();

		FocusDockable(null);
	}

	protected abstract void UpdateDockablesNavigation();

	protected void FocusDockable(Dockable? dockable)
	{
		if (dockable == FocusedDockable) return;

		ContentComposition.ClearInternal();

		if (dockable is not null)
			ContentComposition.AddInternal(dockable.Content);

		FocusedDockable = dockable;

		UpdateDockablesNavigation();
	}

	public void FocusContent(GameObject content)
	{
		foreach (var dockable in Dockables)
		{
			if (dockable.Content == content)
			{
				FocusDockable(dockable);
				return;
			}
		}

		throw new InvalidOperationException("Cannot focus content that is not already part of this docking container");
	}

	protected class Dockable
	{
		public string Name { get; }
		public GameObject Content { get; }

		public Dockable(string name, GameObject content)
		{
			Name = name;
			Content = content;
		}
	}
}
