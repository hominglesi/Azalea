using Azalea.Numerics;
using System.Numerics;

namespace Azalea.Graphics;

public static class Vector2Extentions
{
    public static Vector2 Transform(Vector2 pos, Matrix3 mat)
    {
        Transform(ref pos, ref mat, out Vector2 result);
        return result;
    }

    public static void Transform(ref Vector2 pos, ref Matrix3 mat, out Vector2 result)
    {
        result.X = mat.Row0.X * pos.X + mat.Row1.X * pos.Y + mat.Row2.X;
        result.Y = mat.Row0.Y * pos.X + mat.Row1.Y * pos.Y + mat.Row2.Y;
    }
}
