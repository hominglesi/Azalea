using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Scenes.Physics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Scenes;

public class Scene2D : Leaf, IScene
{
	private int _nextUpdatable = 0;
	private List<Leaf>? _updateables;
	private int _nextDrawable = 0;
	private List<Leaf>? _drawables;

	private PhysicsWorld2D? _physics;

	public override void Invalidate(Invalidation invalidation, Leaf? source = null)
	{
		if (invalidation.HasFlag(Invalidation.Physics))
			_physics ??= new();

		if (_physics is not null && invalidation.HasFlag(Invalidation.Presence) && source is not null)
		{
			if (source.Parent is not null)
				_physics.Add(source, PhysicsWorld2D.FindOwner(source));
			else
				_physics.Remove(source);
		}

		base.Invalidate(invalidation, source);
	}

	public override void Validate()
	{
		if (Invalidations == Invalidation.None)
			return;

		var presence = Invalidations.HasFlag(Invalidation.Presence);

		if (presence)
		{
			_nextUpdatable = 0;
			_nextDrawable = 0;
		}

		validate(this, presence);

		removeStale(ref _updateables, ref _nextUpdatable);
		removeStale(ref _drawables, ref _nextDrawable);
	}

	private void validate(Leaf leaf, bool presence)
	{
		if (presence && leaf is not Scene2D)
		{
			var childOverrides = GetOverrideFlags(leaf);
			if (childOverrides.HasFlag(OverrideFlags.Update))
				insertOverride(leaf, ref _updateables, ref _nextUpdatable);

			if (childOverrides.HasFlag(OverrideFlags.Draw))
				insertOverride(leaf, ref _drawables, ref _nextDrawable);
		}

		if (leaf.Invalidations.HasFlag(Invalidation.Transform))
			leaf.RecalculateTransform();

		foreach (var child in leaf.Children)
			if (child.Invalidations != Invalidation.None)
				validate(child, presence);

		leaf.Invalidations = Invalidation.None;
	}

	private static void insertOverride(Leaf leaf, ref List<Leaf>? list, ref int next)
	{
		list ??= [];

		if (list.Count < (next + 1))
			list.Add(leaf);
		else
			list[next] = leaf;

		next++;
	}

	private static void removeStale(ref List<Leaf>? list, ref int next)
	{
		if (list is null)
			return;

		while (list.Count > next)
			list.RemoveAt(next);
	}

	void IScene.FixedUpdate()
	{
		Validate();
		_physics?.ProcessStep();
	}

	void IScene.Draw(RenderCoordinator coordinator)
	{
		if (_drawables is null) return;

		Validate();

		foreach (var drawable in _drawables)
			drawable.Draw(coordinator);
	}

	void IScene.UpdateSize(Vector2 newSize) => Size = newSize;
	void IScene.SetApplication(Application? app) { }
	IEnumerable<ISceneObject> IScene.BuildNonPositionalInputQueue() => [];
	IEnumerable<ISceneObject> IScene.BuildPositionalInputQueue(Vector2 position) => [];
}
