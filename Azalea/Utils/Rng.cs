﻿using Azalea.Graphics.Colors;
using System;
using System.Collections.Generic;

using Vector2Struct = System.Numerics.Vector2;

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
	public static Vector2Struct Direction()
	{
		var output = new Vector2Struct(Float(-1, 1), Float(-1, 1));
		return Vector2Struct.Normalize(output);
	}

	/// <summary>
	/// Generates a random Color structure
	/// </summary>
	/// <returns>A random Color structure</returns>
	public static Color Color()
	{
		return new Color(Byte(), Byte(), Byte());
	}

	public static Vector2Struct Vector2(Vector2Struct range)
	{
		return new Vector2Struct(Float(range.X, range.Y), Float(range.X, range.Y));
	}

	public static Vector2Struct Vector2(Vector2Struct rangeX, Vector2Struct rangeY)
	{
		return new Vector2Struct(Float(rangeX.X, rangeX.Y), Float(rangeY.X, rangeY.Y));
	}

	/// <summary>
	/// Returns a random object from a List collection
	/// </summary>
	public static T Random<T>(this IList<T> list)
	{
		if (list.Count <= 0) throw new ArgumentException("Cannot get a random value from an empty list");

		return list[Int(list.Count)];
	}
}
