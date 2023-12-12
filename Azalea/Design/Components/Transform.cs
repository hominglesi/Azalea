using Azalea.Graphics;
using System;
using System.Numerics;

namespace Azalea.Design.Components;
public class Transform : Component
{
	internal Action? OnGeomentryChanged;
	internal Action<Axes>? OnSizeChanged;

	private float _x;
	public float X
	{
		get => _x;
		set
		{
			if (value == _x) return;

			_x = value;

			OnGeomentryChanged?.Invoke();
		}
	}

	private float _y;
	public float Y
	{
		get => _y;
		set
		{
			if (value == _y) return;

			_y = value;

			OnGeomentryChanged?.Invoke();
		}
	}

	public Vector2 Position
	{
		get => new(_x, _y);
		set
		{
			if (value == Position) return;

			_x = value.X;
			_y = value.Y;

			OnGeomentryChanged?.Invoke();
		}
	}

	private float _width;
	public float Width
	{
		get => _width;
		set
		{
			if (value == _width) return;

			_width = value;

			OnSizeChanged?.Invoke(Axes.X);
		}
	}

	private float _height;
	public float Height
	{
		get => _height;
		set
		{
			if (value == _height) return;

			_height = value;

			OnSizeChanged?.Invoke(Axes.Y);
		}
	}

	public Vector2 Size
	{
		get => new(_width, _height);
		set
		{
			if (value == Size) return;

			var changedAxes = Axes.None;

			if (_width != value.X)
				changedAxes |= Axes.X;

			if (_height != value.Y)
				changedAxes |= Axes.Y;

			_width = value.X;
			_height = value.Y;

			OnSizeChanged?.Invoke(changedAxes);
		}
	}

	private float _rotation;
	public float Rotation
	{
		get => _rotation;
		set
		{
			if (value == _rotation) return;

			_rotation = value;

			OnGeomentryChanged?.Invoke();
		}
	}
}
