using System;
using System.Collections;
using System.Collections.Generic;

namespace Azalea.Lists;

public class SortedList<T> : ICollection<T>, IReadOnlyList<T>
{
    private readonly List<T> list;

    public int Count => list.Count;
    bool ICollection<T>.IsReadOnly => ((ICollection<T>)list).IsReadOnly;

    public IComparer<T> Comparer { get; }

    public T this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }

    public SortedList()
        : this(Comparer<T>.Default) { }

    public SortedList(IComparer<T> comparer)
    {
        list = new List<T>();
        Comparer = comparer;
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (var i in collection)
            list.Add(i);
    }

    public void RemoveRange(int index, int count) => list.RemoveRange(index, count);

    public int Add(T value) => addInternal(value);

    private int addInternal(T value)
    {
        int index = list.BinarySearch(value, Comparer);
        if (index < 0)
            index = ~index;

        list.Insert(index, value);

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

    public void RemoveAt(int index) => list.RemoveAt(index);
    public int RemoveAll(Predicate<T> match) => list.RemoveAll(match);
    public void Clear() => list.Clear();
    public bool Contains(T item) => IndexOf(item) >= 0;
    public int BinarySearch(T value) => list.BinarySearch(value, Comparer);
    public int IndexOf(T value) => list.IndexOf(value);
    public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
    public T? Find(Predicate<T> match) => list.Find(match);
    public IEnumerable<T> FindAll(Predicate<T> match) => list.FindAll(match);
    public T? FindLast(Predicate<T> match) => list.FindLast(match);
    public int FindIndex(Predicate<T> match) => list.FindIndex(match);

    public void Sort() => list.Sort(Comparer);
    public override string ToString() => $"{GetType()} ({Count} items)";

    void ICollection<T>.Add(T item) => Add(item);
    public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
