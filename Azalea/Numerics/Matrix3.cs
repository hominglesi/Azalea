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
    public static bool operator ==(Matrix3 left, Matrix3 right) => left.Equals(right);
    public static bool operator !=(Matrix3 left, Matrix3 right) => !left.Equals(right);

    public override readonly int GetHashCode() => HashCode.Combine(Row0, Row1, Row2);
}
