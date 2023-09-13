namespace Azalea.Caching;

public class Cached
{
	public bool IsValid { get; private set; }

	public bool Invalidate()
	{
		if (IsValid == false) return false;

		IsValid = false;
		return true;
	}

	public void Validate()
	{
		IsValid = true;
	}
}
