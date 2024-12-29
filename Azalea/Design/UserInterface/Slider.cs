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
		Add(Body = CreateBody());
		Add(Head = CreateHead());

		Head.MouseDown += onHeadMouseDown;
	}

	public GameObject Head { get; init; }
	protected abstract GameObject CreateHead();
	private float _headLength => getDirectionalValue(Head.DrawSize);

	public GameObject Body { get; init; }
	protected abstract GameObject CreateBody();

	private float _value;
	public Action<float>? OnValueChanged;
	public float Value
	{
		get => _value;
		set
		{
			if (value == _value) return;

			_value = value;

			updateHeadPosition();
			OnValueChanged?.Invoke(Value);
		}
	}

	private bool _sliderRangeCached = false;
	private Vector2 _sliderRange;
	private Vector2 SliderRange
	{
		get
		{
			if (_sliderRangeCached == true)
				return _sliderRange;

			var drawLength = getDirectionalValue(DrawSize);
			_sliderRange = new Vector2(0, drawLength);

			if (HeadInsideBody)
			{
				var halfHeadLength = _headLength / 2;
				_sliderRange.X += halfHeadLength;
				_sliderRange.Y -= halfHeadLength;
			}

			_sliderRangeCached = true;
			return _sliderRange;
		}
	}

	private SliderDirection _direction = SliderDirection.Horizontal;
	public SliderDirection Direction
	{
		get => _direction;
		set
		{
			if (_direction == value) return;
			_direction = value;
			uncacheSliderRange();
		}
	}

	private bool _headInsideBody = true;
	public bool HeadInsideBody
	{
		get => _headInsideBody;
		set
		{
			if (value == _headInsideBody) return;
			_headInsideBody = value;

			updateHeadPosition();
		}
	}

	private void uncacheSliderRange()
	{
		_sliderRangeCached = false;
		updateHeadPosition();
	}

	private void updateHeadPosition()
	{
		var pixelValue = MathUtils.Map(Value, 0, 1, SliderRange.X, SliderRange.Y);

		if (Direction == SliderDirection.Horizontal)
			Head.Position = new Vector2(pixelValue, DrawHeight / 2);
		else
			Head.Position = new Vector2(DrawWidth / 2, pixelValue); ;
	}

	private float getLocalMousePosition()
	{
		var localPosition = ToLocalSpace(Input.MousePosition);
		return getDirectionalValue(localPosition);
	}

	private float getDirectionalValue(Vector2 vector)
		=> Direction == SliderDirection.Horizontal ? vector.X : vector.Y;

	public bool IsHeld { get; private set; }
	private float _heldOffset;

	protected void onHeadMouseDown(MouseDownEvent e)
	{
		IsHeld = true;
		_heldOffset = getDirectionalValue(Head.Position) - getLocalMousePosition();
	}

	protected override bool OnMouseDown(MouseDownEvent e)
	{
		if (IsHeld) return true;

		IsHeld = true;
		_heldOffset = 0;
		return true;
	}

	private Vector2 _lastDrawSize;
	private float _lastHeadLength;
	protected override void Update()
	{
		if (Input.GetMouseButton(0).Released)
			IsHeld = false;

		if (_lastDrawSize != DrawSize || _lastHeadLength != _headLength)
		{
			uncacheSliderRange();
		}

		if (IsHeld)
		{
			var newPosition = getLocalMousePosition() + _heldOffset;
			newPosition = Math.Clamp(newPosition, SliderRange.X, SliderRange.Y);

			Value = MathUtils.Map(newPosition, SliderRange.X, SliderRange.Y, 0, 1);
		}
	}
}

public enum SliderDirection
{
	Horizontal,
	Vertical
}
