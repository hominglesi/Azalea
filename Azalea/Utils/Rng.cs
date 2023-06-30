using Azalea.Graphics;
using System;
using System.Numerics;

namespace Azalea.Utils;

/// <summary>
/// Class used for generating simple random values
/// </summary>
public static class Rng
{
    private static readonly Random _random = new();

    /// <summary>
    /// Generates a random int value between 0 and <paramref name="maxNumber">
    /// </summary>
    /// <param name="maxNumber">Top boundary of returned integer value</param>
    /// <param name="inclusive">Defines if top boundary of returned integer value is inclusive</param>
    /// <returns>A random integer value between 0 and <paramref name="maxNumber"></returns>
    public static int Int(int maxNumber, bool inclusive = false)
    {
        if (inclusive) maxNumber += 1;
        return _random.Next(maxNumber);
    }

    /// <summary>
    /// Generates a random int value between <paramref name="minNumber"> and <paramref name="maxNumber">
    /// </summary>
    /// <param name="minNumber">Bottom boundary of returned integer value</param>
    /// <param name="maxNumber">Top boundary of returned integer value</param>
    /// <param name="inclusive">Defines if top boundary of returned integer value is inclusive</param>
    /// <returns>A random integer value between <paramref name="minNumber"> and <paramref name="maxNumber"></returns>
    public static int Int(int minNumber, int maxNumber, bool inclusive = false)
    {
        if (inclusive) maxNumber += 1;
        return _random.Next(minNumber, maxNumber);
    }

    /// <summary>
    /// Generates a random byte value
    /// </summary>
    /// <returns></returns>
    public static byte Byte()
    {
        return (byte)Int(byte.MaxValue, true);
    }

    /// <summary>
    /// Generates a random floating-point value between 0 and 1
    /// </summary>
    /// <returns>A random floating-point value between 0 and 1</returns>
    public static float Float()
    {
        return (float)_random.NextDouble();
    }

    /// <summary>
    /// Generates a random floating-point value between 0 and <paramref name="maxNumber">
    /// </summary>
    /// <param name="maxNumber">Top boundary of returned floating-point value</param>
    /// <returns>A random floating-point value between 0 and <paramref name="maxNumber"></returns>
    public static float Float(float maxNumber)
    {
        return Float() * maxNumber;
    }

    /// <summary>
    /// Generates a random floating-point value between <paramref name="minNumber"> and <paramref name="maxNumber">
    /// </summary>
    /// <param name="minNumber">Bottom boundary of returned floating-point value</param>
    /// <param name="maxNumber">Top boundary of returned floating-point value</param>
    /// <returns>A random floating-point value between <paramref name="minNumber"> and <paramref name="maxNumber"></returns>
    public static float Float(float minNumber, float maxNumber)
    {
        return Float() * (maxNumber - minNumber) + minNumber;
    }

    /// <summary>
    /// Generates a random Vector2 structure containing 2D direction vector
    /// </summary>
    /// <returns>A random Vector2 structure containing 2D direction vector</returns>
    public static Vector2 Direction()
    {
        var output = new Vector2(Float(-1, 1), Float(-1, 1));
        return Vector2.Normalize(output);
    }

    /// <summary>
    /// Generates a random Color structure
    /// </summary>
    /// <returns>A random Color structure</returns>
    public static Color Color()
    {
        return new Color(Byte(), Byte(), Byte());
    }
}
