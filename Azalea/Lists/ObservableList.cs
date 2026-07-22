using System;
using System.Collections;
using System.Collections.Generic;

namespace Azalea.Lists;
public class ObservableList<T> : IEnumerable<T>
{
	private readonly List<T> _list;

	public ObservableList()
	{
		_list = [];
	}

	public Action<T>? OnItemAdded;
	public void Add(T item)
	{
		_list.Add(item);
		OnItemAdded?.Invoke(item);
	}

	public Action<T>? OnItemRemoved;
	public void Remove(T item)
	{
		_list.Remove(item);
		OnItemRemoved?.Invoke(item);
	}

	public Action? OnCleared;
	public void Clear()
	{
		_list.Clear();
		OnCleared?.Invoke();
	}

	public int Count => _list.Count;

	public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
