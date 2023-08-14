using System;
using System.Collections;
using System.Collections.Generic;

namespace Azalea.Lists;

public class SortedList<T> : ICollection<T>, IReadOnlyList<T>
{
    private readonly List<T> _list;

    public int Count => _list.Count;
    bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    public IComparer<T> Comparer { get; }

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public SortedList()
        : this(Comparer<T>.Default) { }

    public SortedList(IComparer<T> comparer)
    {
        _list = new List<T>();
        Comparer = comparer;
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (var i in collection)
            _list.Add(i);
    }

    public void RemoveRange(int index, int count) => _list.RemoveRange(index, count);

    public int Add(T value) => addInternal(value);

    private int addInternal(T value)
    {
        int index = _list.BinarySearch(value, Comparer);
        if (index < 0)
            index = ~index;

        _list.Insert(index, value);

        return index;
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0)
            return false;

        RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index) => _list.RemoveAt(index);
    public int RemoveAll(Predicate<T> match) => _list.RemoveAll(match);
    public void Clear() => _list.Clear();
    public bool Contains(T item) => IndexOf(item) >= 0;
    public int BinarySearch(T value) => _list.BinarySearch(value, Comparer);
    public int IndexOf(T value) => _list.IndexOf(value);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public T? Find(Predicate<T> match) => _list.Find(match);
    public IEnumerable<T> FindAll(Predicate<T> match) => _list.FindAll(match);
    public T? FindLast(Predicate<T> match) => _list.FindLast(match);
    public int FindIndex(Predicate<T> match) => _list.FindIndex(match);

    public void Sort() => _list.Sort(Comparer);
    public override string ToString() => $"{GetType()} ({Count} items)";

    void ICollection<T>.Add(T item) => Add(item);
    public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
