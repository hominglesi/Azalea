using System;

namespace Azalea.Layout;

[Flags]
public enum InvalidationSource
{
    Self = 1,
    Parent = 1 << 1,
    Child = 1 << 2,
    Default = Self | Parent
}
