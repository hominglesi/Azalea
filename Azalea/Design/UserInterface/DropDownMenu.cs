using Azalea.Design.Containers;
using Azalea.Inputs;
using System;

namespace Azalea.Design.UserInterface;
public abstract class DropDownMenu<T> : Composition
	where T : IEquatable<T>
{
	#region Selected Value

	private T? _selectedValue;
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

	public Action<T?>? OnSelectedChanged;

	#endregion

	#region Expanded

	public bool IsExpanded { get; private set; }

	private DropDownExpanded? _expandedSegment;
	protected DropDownExpanded ExpandedSegment
	{
		get => _expandedSegment ??= CreateExpandedSegment();
	}

	protected abstract DropDownExpanded CreateExpandedSegment();

	public virtual void Expand()
	{
		if (IsExpanded)
			return;

		Add(ExpandedSegment);
		IsExpanded = true;
	}

	public virtual void Contract()
	{
		if (IsExpanded == false || ExpandedSegment == null)
			return;

		Remove(ExpandedSegment);
		IsExpanded = false;
	}

	public abstract class DropDownExpanded : Composition
	{

	}

	#endregion

	protected override void Update()
	{
		if (Input.GetMouseButton(MouseButton.Left).Up
			|| Input.GetMouseButton(MouseButton.Right).Up
			|| Input.GetMouseButton(MouseButton.Middle).Up)
		{
			foreach (var item in Input.GetHoveredObjects())
				if (item == this || item == ExpandedSegment)
					return;

			Contract();
		}
	}
}
