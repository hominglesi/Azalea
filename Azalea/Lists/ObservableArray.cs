using System;
using System.Collections;
using System.Collections.Generic;

namespace Azalea.Lists;

public class ObservableArray<T> : IReadOnlyList<T>, IEquatable<ObservableArray<T>>, INotifyArrayChanged
{
	public event Action? ArrayElementsChanged;

	private readonly T[] _wrappedArray;

	public ObservableArray(T[] arrayToWarp)
	{
		ArgumentNullException.ThrowIfNull(arrayToWarp);
		_wrappedArray = arrayToWarp;
	}

	public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_wrappedArray).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _wrappedArray.GetEnumerator();

	public bool Equals(ObservableArray<T>? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;

		return _wrappedArray == other._wrappedArray;
	}

	public override bool Equals(object? obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;

		return obj.GetType() == GetType() && Equals((ObservableArray<T>)obj);
	}

	public override int GetHashCode() => HashCode.Combine(_wrappedArray);

	public int Count => _wrappedArray.Length;

	public T this[int index]
	{
		get => _wrappedArray[index];
		set
		{
			if (EqualityComparer<T>.Default.Equals(_wrappedArray[index], value))
				return;

			var previousValue = _wrappedArray[index];
			if (previousValue is INotifyArrayChanged previousNotifier)
				previousNotifier.ArrayElementsChanged -= OnArrayElementsChanged;

			_wrappedArray[index] = value;
			if (value is INotifyArrayChanged notifier)
				notifier.ArrayElementsChanged += OnArrayElementsChanged;

			OnArrayElementsChanged();
		}
	}

	protected void OnArrayElementsChanged()
	{
		ArrayElementsChanged?.Invoke();
	}
}
