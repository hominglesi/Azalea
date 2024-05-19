using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.Design.UserInterface;
public abstract class Slider : Composition
{
	public Slider()
	{
		Add(_body = CreateBody());
		Add(_head = CreateHead());

		_head.MouseDown += onHeadMouseDown;
	}

	protected abstract GameObject CreateHead();
	private GameObject _head;
	public GameObject Head => _head;
	private float _headLength => _direction == SliderDirection.Horizontal ? _head.DrawWidth : _head.DrawHeight;
	protected abstract GameObject CreateBody();
	private GameObject _body;
	public GameObject Body => _body;

	private void setHeadPosition(float pos)
	{
		if (_direction == SliderDirection.Horizontal)
			_head.Position = new Vector2(pos, DrawHeight / 2);
		else
			_head.Position = new Vector2(DrawWidth / 2, pos); ;
	}
	private float getLocalMousePosition()
	{
		var localPosition = ToLocalSpace(Input.MousePosition);
		if (_direction == SliderDirection.Horizontal)
			return localPosition.X;
		else
			return localPosition.Y;
	}

	private SliderDirection _direction = SliderDirection.Horizontal;
	public SliderDirection Direction
	{
		get => _direction;
		set
		{
			if (_direction == value) return;

			_direction = value;
		}
	}

	public Action<float>? OnValueChanged;

	public float Value
	{
		get => MathUtils.Map(SliderPosition, _sliderRange.X, _sliderRange.Y, 0, 1);
		set
		{
			var newPosition = MathUtils.Map(value, 0, 1, _sliderRange.X, _sliderRange.Y);
			if (SliderPosition == newPosition) return;

			SliderPosition = newPosition;
		}
	}

	private Vector2 calculateSliderRange(float length)
	{
		var range = new Vector2(0, length);
		if (_ignoreHeadSize || _headLength > length) return range;

		var headHalf = _headLength / 2;
		range.X += headHalf;
		range.Y -= headHalf;

		return range;
	}

	private bool _ignoreHeadSize;
	private float _sliderPosition;
	private Vector2 _sliderRange;
	private float _heldOffset;

	private float SliderPosition
	{
		get => _sliderPosition;
		set
		{
			setHeadPosition(value);

			if (Precision.AlmostEquals(_sliderPosition, value)) return;

			_sliderPosition = value;
			OnValueChanged?.Invoke(Value);
		}
	}

	private bool _isHeld;

	protected void onHeadMouseDown(MouseDownEvent e)
	{
		_isHeld = true;
		_heldOffset = SliderPosition - getLocalMousePosition();
	}

	private Vector2 _lastDrawSize;
	private float _lastSliderLength;

	protected override bool OnMouseDown(MouseDownEvent e)
	{
		if (_isHeld) return true;

		SliderPosition = Math.Clamp(getLocalMousePosition(), _sliderRange.X, _sliderRange.Y);
		_isHeld = true;
		_heldOffset = 0;
		return true;
	}

	protected override void Update()
	{
		if (Input.GetMouseButton(0).Released)
			_isHeld = false;

		if (_lastDrawSize != DrawSize || _lastSliderLength != _headLength)
		{
			var drawLength = Direction == SliderDirection.Horizontal ? DrawSize.X : DrawSize.Y;
			var oldRange = _sliderRange;
			_sliderRange = calculateSliderRange(drawLength);

			SliderPosition = MathUtils.Map(SliderPosition, oldRange.X, oldRange.Y, _sliderRange.X, _sliderRange.Y);

			_lastDrawSize = DrawSize;
			_lastSliderLength = _headLength;
		}

		if (_isHeld)
		{
			var newPosition = getLocalMousePosition() + _heldOffset;

			if (newPosition == SliderPosition) return;

			SliderPosition = Math.Clamp(newPosition, _sliderRange.X, _sliderRange.Y);
		}
	}
}

public enum SliderDirection
{
	Horizontal,
	Vertical
}
