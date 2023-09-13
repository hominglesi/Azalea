using Azalea.Layout;
using System.Diagnostics;

namespace Azalea.Graphics;

internal struct InvalidationList
{
	private Invalidation selfInvalidation;
	private Invalidation parentInvalidation;
	private Invalidation childInvalidation;

	public InvalidationList(Invalidation initialState)
	{
		this = default;

		invalidate(selfInvalidation, initialState, out selfInvalidation);
		invalidate(parentInvalidation, initialState, out parentInvalidation);
		invalidate(childInvalidation, initialState, out childInvalidation);
	}

	public bool Invalidate(InvalidationSource source, Invalidation flags)
	{
		Debug.Assert(source is InvalidationSource.Self or InvalidationSource.Parent or InvalidationSource.Child);

		return source switch
		{
			InvalidationSource.Self => invalidate(selfInvalidation, flags, out selfInvalidation),
			InvalidationSource.Parent => invalidate(parentInvalidation, flags, out parentInvalidation),
			_ => invalidate(childInvalidation, flags, out childInvalidation),
		};
	}

	public bool Validate(Invalidation validation)
	{
		return validate(selfInvalidation, validation, out selfInvalidation)
			| validate(parentInvalidation, validation, out parentInvalidation)
			| invalidate(childInvalidation, validation, out childInvalidation);
	}

	private bool invalidate(Invalidation target, Invalidation flags, out Invalidation result)
	{
		result = target | (flags & Invalidation.Layout);
		return (target & flags) != flags;
	}

	private bool validate(Invalidation target, Invalidation flags, out Invalidation result)
	{
		result = target & ~flags;
		return (target & flags) != 0;
	}
}
