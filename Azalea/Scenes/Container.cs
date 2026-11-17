using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using System.Numerics;

namespace Azalea.Scenes;

public class Container : Leaf
{
	public bool Masking
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Tree);
		}
	}

	private RectangleInt _maskingRectangle;

	public ColorQuad? BackgroundColor
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			// Invalidate here
		}
	}

	public override void Validate()
	{
		if (Masking)
		{
			var scissorDrawInfo = DrawInfo;
			scissorDrawInfo.ApplyTranslation(ChildOffset);

			var scissorQuad = new Quad(Vector2.Zero, ChildSize) * scissorDrawInfo.Matrix;
			var scissorRect = scissorQuad.AABBFloat;

			_maskingRectangle = new RectangleInt(
				(int)float.Ceiling(scissorRect.Position.X), (int)float.Ceiling(scissorRect.Position.Y),
				(int)float.Ceiling(scissorRect.Size.X), (int)float.Ceiling(scissorRect.Size.Y));
		}
	}

	public override void Draw(RenderCoordinator coordinator)
	{
		if (BackgroundColor is not null)
		{
			coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
			coordinator.BindTexture(Assets.WhitePixelNative);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, WorldQuad, BackgroundColor.Value, Rectangle.One);
		}

		if (Masking)
			coordinator.PushScissor(_maskingRectangle);
	}

	public override void DrawAfterChildren(RenderCoordinator coordinator)
	{
		if (Masking)
			coordinator.PopScissor();
	}

	public float ChildAspectRatio
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public Vector2 ChildSpace
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			Invalidate(Invalidation.Transform);
		}
	}

	public override Vector2 ChildSize
	{
		get
		{
			if (ChildAspectRatio == 0) return LocalSize;

			var neededHeight = LocalSize.X / ChildAspectRatio;
			if (neededHeight <= LocalSize.Y)
				return new(LocalSize.X, neededHeight);

			var neededWidth = LocalSize.Y * ChildAspectRatio;
			return new(neededWidth, LocalSize.Y);
		}
	}

	public override Vector2 ChildOffset => (LocalSize - ChildSize) / 2;
	public override Vector2 ChildScale
	{
		get
		{
			if (ChildSpace == Vector2.Zero) return Vector2.One;

			return ChildSize / ChildSpace;
		}
	}
}
