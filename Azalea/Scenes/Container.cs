using Azalea.Graphics.Colors;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using System.Numerics;

namespace Azalea.Scenes;

public class Container : Leaf
{
	public bool Masking { get; set; }

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

	public override void Draw(RenderCoordinator coordinator)
	{
		if (BackgroundColor is not null)
		{
			coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
			coordinator.BindTexture(Assets.WhitePixelNative);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, WorldQuad, BackgroundColor.Value, Rectangle.One);
		}

		if (Masking)
		{
			var pos = ChildOffset;
			var size = GetChildSpace(false);

			var maskingRect = new RectangleInt(
				(int)float.Ceiling(pos.X), (int)float.Ceiling(pos.Y),
				(int)float.Ceiling(size.X), (int)float.Ceiling(size.Y));

			coordinator.PushScissor(maskingRect);
		}
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

	public override Vector2 GetChildSpace(bool childPerspective = true)
	{
		if (childPerspective && ChildSpace != Vector2.Zero)
			return ChildSpace;

		if (ChildAspectRatio == 0) return LocalSize;

		var neededHeight = LocalSize.X / ChildAspectRatio;
		if (neededHeight <= LocalSize.Y)
			return new(LocalSize.X, neededHeight);

		var neededWidth = LocalSize.Y * ChildAspectRatio;
		return new(neededWidth, LocalSize.Y);
	}

	public override Vector2 ChildOffset => (LocalSize - GetChildSpace(false)) / 2;
	public override Vector2 ChildScale
	{
		get
		{
			if (ChildSpace == Vector2.Zero) return Vector2.One;

			return GetChildSpace(false) / ChildSpace;
		}
	}
}
