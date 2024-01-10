using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Numerics;
public struct Matrix2 : IEquatable<Matrix2>
{
	public Vector2 Row0;
	public Vector2 Row1;

	public static readonly Matrix2 Identity = new(Vector2.UnitX, Vector2.UnitY);
	public static readonly Matrix2 Zero = new(Vector2.Zero, Vector2.Zero);

	public Matrix2(Vector2 row0, Vector2 row1)
	{
		Row0 = row0;
		Row1 = row1;
	}

	public float M11 { readonly get => Row0.X; set => Row0.X = value; }
	public float M12 { readonly get => Row0.Y; set => Row0.Y = value; }
	public float M21 { readonly get => Row1.X; set => Row1.X = value; }
	public float M22 { readonly get => Row1.Y; set => Row1.Y = value; }


	public readonly bool Equals(Matrix2 other)
		=> Row0.Equals(other.Row0) && Row1.Equals(other.Row1);
	public override readonly bool Equals(object? obj)
		=> obj is not null && obj is Matrix2 mObj && Equals(mObj);
	public static Matrix2 operator *(Matrix2 left, Matrix2 right) => Mult(left, right);
	public static bool operator ==(Matrix2 left, Matrix2 right) => left.Equals(right);
	public static bool operator !=(Matrix2 left, Matrix2 right) => !left.Equals(right);

	public override readonly int GetHashCode() => HashCode.Combine(Row0, Row1);

	public static Matrix2 Mult(Matrix2 left, Matrix2 right)
	{
		Matrix2 result;
		Mult(ref left, ref right, out result);
		return result;
	}

	public static void Mult(ref Matrix2 left, ref Matrix2 right, out Matrix2 result)
	{
		float lM11 = left.Row0.X, lM12 = left.Row0.Y,
			lM21 = left.Row1.X, lM22 = left.Row1.Y,
			rM11 = right.Row0.X, rM12 = right.Row0.Y,
			rM21 = right.Row1.X, rM22 = right.Row1.Y;

		result.Row0.X = ((lM11 * rM11) + (lM12 * rM21));
		result.Row0.Y = ((lM11 * rM12) + (lM12 * rM22));
		result.Row1.X = ((lM21 * rM11) + (lM22 * rM21));
		result.Row1.Y = ((lM21 * rM12) + (lM22 * rM22));
	}
}
