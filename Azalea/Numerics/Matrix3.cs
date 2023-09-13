using System;
using System.Numerics;

namespace Azalea.Numerics;

public struct Matrix3 : IEquatable<Matrix3>
{
	public Vector3 Row0;
	public Vector3 Row1;
	public Vector3 Row2;

	public static readonly Matrix3 Identity = new(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);
	public static readonly Matrix3 Zero = new(Vector3.Zero, Vector3.Zero, Vector3.Zero);

	public Matrix3(Vector3 row0, Vector3 row1, Vector3 row2)
	{
		Row0 = row0;
		Row1 = row1;
		Row2 = row2;
	}

	public float M11 { readonly get => Row0.X; set => Row0.X = value; }
	public float M12 { readonly get => Row0.Y; set => Row0.Y = value; }
	public float M13 { readonly get => Row0.Z; set => Row0.Z = value; }
	public float M21 { readonly get => Row1.X; set => Row1.X = value; }
	public float M22 { readonly get => Row1.Y; set => Row1.Y = value; }
	public float M23 { readonly get => Row1.Z; set => Row1.Z = value; }
	public float M31 { readonly get => Row2.X; set => Row2.X = value; }
	public float M32 { readonly get => Row2.Y; set => Row2.Y = value; }
	public float M33 { readonly get => Row2.Z; set => Row2.Z = value; }

	public readonly bool Equals(Matrix3 other)
		=> Row0.Equals(other.Row0) && Row1.Equals(other.Row1) && Row2.Equals(other.Row2);
	public override readonly bool Equals(object? obj)
		=> obj is not null && obj is Matrix3 mObj && Equals(mObj);
	public static Matrix3 operator *(Matrix3 left, Matrix3 right) => Mult(left, right);
	public static bool operator ==(Matrix3 left, Matrix3 right) => left.Equals(right);
	public static bool operator !=(Matrix3 left, Matrix3 right) => !left.Equals(right);

	public override readonly int GetHashCode() => HashCode.Combine(Row0, Row1, Row2);

	public static Matrix3 Mult(Matrix3 left, Matrix3 right)
	{
		Matrix3 result;
		Mult(ref left, ref right, out result);
		return result;
	}

	public static void Mult(ref Matrix3 left, ref Matrix3 right, out Matrix3 result)
	{
		float lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z,
			lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z,
			lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z,
			rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z,
			rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z,
			rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z;

		result.Row0.X = ((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31);
		result.Row0.Y = ((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32);
		result.Row0.Z = ((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33);
		result.Row1.X = ((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31);
		result.Row1.Y = ((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32);
		result.Row1.Z = ((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33);
		result.Row2.X = ((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31);
		result.Row2.Y = ((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32);
		result.Row2.Z = ((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33);
	}
}
