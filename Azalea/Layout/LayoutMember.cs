using Azalea.Graphics;

namespace Azalea.Layout;

public abstract class LayoutMember(Invalidation invalidation, InvalidationSource source = InvalidationSource.Default)
{
	public readonly Invalidation Invalidation = invalidation;
	public readonly InvalidationSource Source = source;
	internal GameObject? Parent;
	internal LayoutMember? Next;

	public bool IsValid { get; private set; }

	public bool Invalidate()
	{
		if (IsValid == false) return false;

		IsValid = false;
		return true;
	}

	public void Validate()
	{
		if (IsValid == true) return;

		IsValid = true;
		Parent?.ValidateSuperTree(Invalidation);
	}
}
