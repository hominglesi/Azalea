using System;

namespace Azalea.Graphics;

public struct DrawColorInfo : IEquatable<DrawColorInfo>
{
    public Color Color;
    public float Alpha;

    public DrawColorInfo(Color color, float alpha)
    {
        Color = color;
        Alpha = alpha;
    }

    public readonly Color AlphaAdjustedColor => new(Color.R, Color.G, Color.B, (byte)(Color.A * Alpha));

    public readonly bool Equals(DrawColorInfo other) => Alpha == other.Alpha && Color == other.Color;
}
