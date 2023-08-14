using Azalea.Graphics;

namespace Azalea.Layout;

public class LayoutValue : LayoutMember
{
    public LayoutValue(Invalidation invalidation, InvalidationSource source = InvalidationSource.Default)
        : base(invalidation, source) { }
}


