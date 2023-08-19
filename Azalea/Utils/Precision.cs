using System;

namespace Azalea.Utils;

public static class Precision
{
    public const float FLOAT_EPSILON = 1e-3f;

    public static bool DefinitelyBigger(float value1, float value2, float acceptableDifference = FLOAT_EPSILON)
        => value1 - acceptableDifference > value2;

    public static bool AlmostEquals(float value1, float value2, float acceptableDifference = FLOAT_EPSILON)
        => Math.Abs(value1 - value2) <= acceptableDifference;
}
