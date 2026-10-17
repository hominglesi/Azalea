using Azalea.Graphics;

namespace Azalea.Layout;

public class LayoutValue(Invalidation invalidation, InvalidationSource source = InvalidationSource.Default)
	: LayoutMember(invalidation, source);


