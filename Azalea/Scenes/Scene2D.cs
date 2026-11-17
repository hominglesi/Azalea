using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Scenes.Physics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Scenes;

public class Scene2D : Container, IScene
{
	private int _nextUpdatable = 0;
	private List<Leaf>? _updateables;
	private int _nextDrawable = 0;
	private List<Drawable>? _drawables;

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
		var overrideFlags = GetOverrideFlags(leaf);

		if (presence)
		{
			if (overrideFlags.HasFlag(OverrideFlags.Update))
				insertObject(leaf, ref _updateables, ref _nextUpdatable);

			if (overrideFlags.HasFlag(OverrideFlags.Draw))
				insertObject(new Drawable(leaf, false), ref _drawables, ref _nextDrawable);
		}

		if (leaf.Invalidations.HasFlag(Invalidation.Transform))
			leaf.RecalculateTransform();

		foreach (var child in leaf.Children)
			if (child.Invalidations != Invalidation.None)
				validate(child, presence);

		if (overrideFlags.HasFlag(OverrideFlags.Validate) && leaf is not Scene2D)
			leaf.Validate();

		if (presence)
		{
			if (overrideFlags.HasFlag(OverrideFlags.DrawAfterChildren))
				insertObject(new Drawable(leaf, true), ref _drawables, ref _nextDrawable);
		}

		leaf.Invalidations = Invalidation.None;
	}

	private static void insertObject<T>(T obj, ref List<T>? list, ref int next)
	{
		list ??= [];

		if (list.Count < (next + 1))
			list.Add(obj);
		else
			list[next] = obj;

		next++;
	}

	private static void removeStale<T>(ref List<T>? list, ref int next)
	{
		if (list is null)
			return;

		while (list.Count > next)
			list.RemoveAt(next);
	}

	void IScene.RunUpdate()
	{
		if (_updateables is null) return;

		Validate();

		foreach (var updateable in _updateables)
			updateable.Update();
	}

	void IScene.RunFixedUpdate()
	{
		Validate();
		_physics?.ProcessStep();
	}

	void IScene.RunDraw(RenderCoordinator coordinator)
	{
		if (_drawables is null) return;

		Validate();

		foreach (var drawable in _drawables)
		{
			if (drawable.AfterChildren == false)
				drawable.Target.Draw(coordinator);
			else
				drawable.Target.DrawAfterChildren(coordinator);
		}
	}

	void IScene.UpdateSize(Vector2 newSize) => Size = newSize;
	void IScene.SetApplication(Application? app) { }
	IEnumerable<ISceneObject> IScene.BuildNonPositionalInputQueue() => [];
	IEnumerable<ISceneObject> IScene.BuildPositionalInputQueue(Vector2 position) => [];

	readonly struct Drawable(Leaf target, bool afterChildren)
	{
		public readonly Leaf Target = target;
		public readonly bool AfterChildren = afterChildren;
	}
}
