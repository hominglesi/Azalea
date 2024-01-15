using Azalea.Graphics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Design.Containers;
public class ContentContainer : Composition
{
	protected CompositeGameObject ContentComposition { get; init; }

	public ContentContainer()
	{
		AddInternal(ContentComposition = new CompositeGameObject());
	}

	private Vector2 _lastDrawSize = new(-1);
	private Vector2 _lastContentDrawSize = new(-1);

	protected override void Update()
	{
		base.Update();

		if (_lastDrawSize != DrawSize || _lastContentDrawSize != ContentComposition.DrawSize)
		{
			UpdateContentLayout();

			_lastDrawSize = DrawSize;
			_lastContentDrawSize = ContentComposition.DrawSize;
		}
	}

	protected virtual void UpdateContentLayout()
		=> ContentComposition.Size = DrawSize;


	protected override IReadOnlyList<GameObject> PublicChildren => ContentComposition.InternalChildren;
	public override void Add(GameObject gameObject) => ContentComposition.AddInternal(gameObject);
	public override bool Remove(GameObject gameObject) => ContentComposition.RemoveInternal(gameObject);
	public override void Clear() => ContentComposition.ClearInternal();
}
