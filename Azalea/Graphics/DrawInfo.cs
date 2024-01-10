using Azalea.Extentions.MatrixExtentions;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.Graphics;

public struct DrawInfo
{
	public Matrix3 Matrix;
	public Matrix3 MatrixInverse;

	public DrawInfo(Matrix3? matrix = null, Matrix3? matrixInverse = null)
	{
		Matrix = matrix ?? Matrix3.Identity;
		MatrixInverse = matrixInverse ?? Matrix3.Identity;
	}

	public void ApplyTransformations(Vector2 translation, Vector2 scale, float rotation, Vector2 shear, Vector2 origin)
	{
		if (translation != Vector2.Zero)
		{
			MatrixExtentions.TranslateFromLeft(ref Matrix, translation);
			MatrixExtentions.TranslateFromRight(ref MatrixInverse, -translation);
		}

		if (rotation != 0)
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

		if (shear != Vector2.Zero)
		{
			throw new NotImplementedException();
		}

		if (scale != Vector2.One)
		{
			if (scale.X == 0) scale.X = Precision.FLOAT_EPSILON;
			if (scale.Y == 0) scale.Y = Precision.FLOAT_EPSILON;

			MatrixExtentions.ScaleFromLeft(ref Matrix, scale);
			MatrixExtentions.ScaleFromRight(ref MatrixInverse, Vector2.Divide(Vector2.One, scale));
		}

		if (origin != Vector2.Zero)
		{
			MatrixExtentions.TranslateFromLeft(ref Matrix, -origin);
			MatrixExtentions.TranslateFromRight(ref MatrixInverse, origin);
		}
	}
}
