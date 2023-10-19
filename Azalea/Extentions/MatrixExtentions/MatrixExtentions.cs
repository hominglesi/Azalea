using Azalea.Numerics;
using System;
using System.Numerics;

namespace Azalea.Extentions.MatrixExtentions;

public static class MatrixExtentions
{
	public static void TranslateFromLeft(ref Matrix3 m, Vector2 v)
	{
		m.Row2 += m.Row0 * v.X + m.Row1 * v.Y;
	}

	public static void TranslateFromRight(ref Matrix3 m, Vector2 v)
	{
		//m.Column0 += m.Column2 * v.X
		m.M11 += m.M13 * v.X;
		m.M21 += m.M23 * v.X;
		m.M31 += m.M33 * v.X;

		//m.Column1 += m.Column2 * v.Y
		m.M12 += m.M13 * v.Y;
		m.M22 += m.M23 * v.Y;
		m.M32 += m.M33 * v.Y;
	}

	public static void RotateFromLeft(ref Matrix3 m, float radians)
	{
		float cos = MathF.Cos(radians);
		float sin = MathF.Sin(radians);

		Vector3 row0 = m.Row0 * cos + m.Row1 * sin;
		m.Row1 = m.Row1 * cos - m.Row0 * sin;
		m.Row0 = row0;
	}

	public static void RotateFromRight(ref Matrix3 m, float radians)
	{
		float cos = MathF.Cos(radians);
		float sin = MathF.Sin(radians);

		//Vector3 column0 = m.Column0 * cos + m.Column1 * sin;
		float m11 = m.M11 * cos - m.M12 * sin;
		float m21 = m.M21 * cos - m.M22 * sin;
		float m31 = m.M31 * cos - m.M32 * sin;

		//m.Column1 = m.Column1 * cos - m.Column0 * sin;
		m.M12 = m.M12 * cos + m.M11 * sin;
		m.M22 = m.M22 * cos + m.M21 * sin;
		m.M32 = m.M32 * cos + m.M31 * sin;

		//m.Column0 = row0;
		m.M11 = m11;
		m.M21 = m21;
		m.M31 = m31;
	}

	public static void ScaleFromLeft(ref Matrix3 m, Vector2 v)
	{
		m.Row0 *= v.X;
		m.Row1 *= v.Y;
	}

	public static void ScaleFromRight(ref Matrix3 m, Vector2 v)
	{
		//m.Column0 *= v.X;
		m.M11 *= v.X;
		m.M21 *= v.X;
		m.M31 *= v.X;

		//m.Column1 *= v.Y;
		m.M12 *= v.Y;
		m.M22 *= v.Y;
		m.M32 *= v.Y;
	}
}
