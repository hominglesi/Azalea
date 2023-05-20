using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics;

/// <summary>
/// Represents a color with 4 floating-point components (Red, Green, Blue, Alpha)
/// </summary>
public struct Color
{
    /// <summary>
    /// The red component of this Color
    /// </summary>
    public float R;

    /// <summary>
    /// The green component of this Color
    /// </summary>
    public float G;

    /// <summary>
    /// The blue color of this Color
    /// </summary>
    public float B;

    /// <summary>
    /// The alpha component of this Color
    /// </summary>
    public float A;

    /// <summary>
    /// Constructs a new Color from the specified components
    /// </summary>
    /// <param name="r">The red component of the new Color</param>
    /// <param name="g">The green component of the new Color</param>
    /// <param name="b">The blue component of the new Color</param>
    /// <param name="a">The alphe component of the new Color</param>
    public Color(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Constructs a new Color from the specified components
    /// </summary>
    /// <param name="r">The red component of the new Color</param>
    /// <param name="g">The green component of the new Color</param>
    /// <param name="b">The blue component of the new Color</param>
    public Color(float r, float g, float b) : this(r, g, b, 1f) { }

    public override bool Equals(object? obj)
    {
        if (obj is not Color color) return false;

        return R == color.R && G == color.G && B == color.B && A == color.A;
    }
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public override int GetHashCode() => (R, G, B, A).GetHashCode();
}
