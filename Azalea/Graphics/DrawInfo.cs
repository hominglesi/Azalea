using Azalea.Extentions.MatrixExtentions;
using Azalea.Numerics;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.Graphics;

public struct DrawInfo(Matrix3? matrix = null, Matrix3? matrixInverse = null)
{
	public Matrix3 Matrix = matrix ?? Matrix3.Identity;
	public Matrix3 MatrixInverse = matrixInverse ?? Matrix3.Identity;

	public void ApplyTranslation(Vector2 translation)
	{
		MatrixExtentions.TranslateFromLeft(ref Matrix, translation);
		MatrixExtentions.TranslateFromRight(ref MatrixInverse, -translation);
	}

	public void ApplyRotation(float rotation)
	{
		float radians = MathUtils.DegreesToRadians(rotation);
		if (AzaleaSettings.RotateClockwise)
		{
			MatrixExtentions.RotateFromLeft(ref Matrix, radians);
			MatrixExtentions.RotateFromRight(ref MatrixInverse, -radians);
		}
		else
		{
			MatrixExtentions.RotateFromLeft(ref Matrix, -radians);
			MatrixExtentions.RotateFromRight(ref MatrixInverse, radians);
		}
	}

	public void ApplyScale(Vector2 scale)
	{
		if (scale.X == 0) scale.X = Precision.FLOAT_EPSILON;
		if (scale.Y == 0) scale.Y = Precision.FLOAT_EPSILON;

		MatrixExtentions.ScaleFromLeft(ref Matrix, scale);
		MatrixExtentions.ScaleFromRight(ref MatrixInverse, Vector2.Divide(Vector2.One, scale));
	}

	public void ApplyOrigin(Vector2 origin)
	{
		MatrixExtentions.TranslateFromLeft(ref Matrix, -origin);
		MatrixExtentions.TranslateFromRight(ref MatrixInverse, origin);
	}
}
