using System;

namespace Azalea.Graphics;

public struct DrawColorInfo : IEquatable<DrawColorInfo>
{
    public float Alpha;

    public DrawColorInfo(float alpha)
    {
        Alpha = alpha;
    }

    public readonly bool Equals(DrawColorInfo other) => Alpha == other.Alpha;
}
