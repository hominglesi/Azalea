using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System;
using System.Collections.Generic;

namespace Azalea.Design.Controls;
public abstract class AbstractDropDownMenu<T> : Composition
	where T : IEquatable<T>
{
	private readonly List<T> _values = [];

	public IEnumerable<T> Values
	{
		get
		{
			foreach (var item in _values)
				yield return item;
		}
		set
		{
			ClearOptionsInternal();
			_values.Clear();

			foreach (var item in value)
			{
				AddOptionInternal(item);
				_values.Add(item);
			}
		}
	}

	protected abstract void AddOptionInternal(T item);
	protected abstract void RemoveOptionInternal(T item);
	protected abstract void ClearOptionsInternal();

	private T? _selectedValue;
	public Action<T?>? OnSelectedChanged;
	public T? SelectedValue
	{
		get => _selectedValue;
		set
		{
			if ((_selectedValue is null && value is null) ||
				(_selectedValue is not null && _selectedValue.Equals(value)))
				return;

			_selectedValue = value;

			OnSelectedChanged?.Invoke(_selectedValue);
		}
	}

	public bool IsExpanded { get; private set; } = false;

	private GameObject? _expandedSegment;
	protected GameObject ExpandedSegment
	{
		get => _expandedSegment ??= CreateExpandedSegment();
	}

	protected abstract GameObject CreateExpandedSegment();

	protected abstract void ExpandImplementation();
	public void Expand()
	{
		if (IsExpanded)
			return;

		ExpandImplementation();
		IsExpanded = true;
	}

	protected abstract void ContractImplementation();
	public virtual void Contract()
	{
		if (IsExpanded == false || ExpandedSegment == null)
			return;

		ContractImplementation();
		IsExpanded = false;
	}

	protected override bool OnClick(ClickEvent e)
	{
		if (IsExpanded)
			Contract();
		else
			Expand();

		return true;
	}

	protected override void Update()
	{
		if (Input.GetMouseButton(MouseButton.Left).Up)
		{
			foreach (var item in Input.GetHoveredObjects())
				if (item == this || item == ExpandedSegment)
					return;

			Contract();
		}
	}
}
