using Azalea.Graphics;

namespace Azalea.Layout;

public abstract class LayoutMember
{
	public readonly Invalidation Invalidation;
	public readonly InvalidationSource Source;
	internal GameObject? Parent;
	internal LayoutMember? Next;

	public LayoutMember(Invalidation invalidation, InvalidationSource source = InvalidationSource.Default)
	{
		Invalidation = invalidation;
		Source = source;
	}

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
