using Azalea.Graphics.Rendering;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Text;
using System;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class SpriteText : GameObject
{
	private TextLayoutProvider _layoutProvider = new();

	public FontUsage Font
	{
		get => _layoutProvider.Font;
		set
		{
			if (_layoutProvider.Font == value)
				return;

			_layoutProvider.Font = value;

			if (string.IsNullOrEmpty(Text) == false)
				base.Size = _layoutProvider.GetSize();
		}
	}

	public string Text
	{
		get => _layoutProvider.Text;
		set
		{
			if (_layoutProvider.Text == value)
				return;

			_layoutProvider.Text = value;
			base.Size = _layoutProvider.GetSize();
		}
	}

	public new float Width
	{
		get => base.Width;
		set => throw new InvalidOperationException($"Cannot set {nameof(Width)} of {nameof(SpriteText)}");
	}

	public new float Height
	{
		get => base.Height;
		set => throw new InvalidOperationException($"Cannot set {nameof(Height)} of {nameof(SpriteText)}");
	}

	public new Vector2 Size
	{
		get => base.Size;
		set => throw new InvalidOperationException($"Cannot set {nameof(Size)} of {nameof(SpriteText)}");
	}

	public override void Draw(RenderCoordinator coordinator)
	{
		coordinator.BindShader(GameHost.Instance.Loader.DefaultTextShader);

		foreach (var character in _layoutProvider.GetCharacters())
		{
			var quad = ToScreenSpace(character.DrawRectangle);

			character.Texture.Bind(coordinator);

			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, quad, DrawColorInfo.Color, character.Texture.GetUVCoordinates());
		}
	}
}
