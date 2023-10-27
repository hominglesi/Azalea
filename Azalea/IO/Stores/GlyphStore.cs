using Azalea.Graphics.Textures;
using Azalea.Text;
using SharpFNT;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Azalea.IO.Stores;

public class GlyphStore : IResourceStore<TextureData>, IGlyphStore
{
	protected readonly string? AssetName;

	protected readonly IResourceStore<TextureData>? TextureLoader;

	public string? FontName { get; }

	public float? Baseline => Font?.Common.Base;

	protected readonly ResourceStore<byte[]> Store;

	protected BitmapFont? Font;

	public GlyphStore(ResourceStore<byte[]> store, string? assetName = null, IResourceStore<TextureData>? textureLoader = null)
	{
		Store = new ResourceStore<byte[]>(store);

		Store.AddExtention("bin");
		Store.AddExtention("fnt");

		AssetName = assetName;
		TextureLoader = textureLoader;

		FontName = assetName?.Split('/').Last();

		Font = LoadFont();
	}

	public BitmapFont? LoadFont()
	{
		try
		{
			BitmapFont font;

			using var s = Store.GetStream($@"{AssetName}");

			font = BitmapFont.FromStream(s, FormatHint.Binary, false);

			return font;
		}
		catch
		{
			return null;
		}
	}

	public bool HasGlyph(char c) => Font?.Characters.ContainsKey(c) == true;

	public virtual TextureData? GetPageImage(int page)
	{
		if (TextureLoader != null)
			return TextureLoader.Get(GetFilenameForPage(page));

		using var stream = Store.GetStream(GetFilenameForPage(page));
		Debug.Assert(stream != null);
		return new TextureData(stream);
	}

	protected string GetFilenameForPage(int page)
	{
		Debug.Assert(Font != null);
		return $@"{AssetName}_{page.ToString().PadLeft((Font.Pages.Count - 1).ToString().Length, '0')}.png";
	}
	public CharacterGlyph? Get(char character)
	{
		if (Font == null) return null;

		Debug.Assert(Baseline != null);

		var bmCharacter = Font.GetCharacter(character);

		return new CharacterGlyph(character, bmCharacter.XOffset, bmCharacter.YOffset, bmCharacter.XAdvance, Baseline.Value, this);
	}

	public int GetKerning(char left, char right) => Font?.GetKerningAmount(left, right) ?? 0;

	public TextureData? Get(string name)
	{
		if (Font == null) return null;

		if (name.Length > 1 && !name.StartsWith($@"{FontName}/", StringComparison.Ordinal))
			return null;

		return Font.Characters.TryGetValue(name.Last(), out Character? c) ? LoadCharacter(c) : null;
	}

	protected int LoadedGlyphCount;

	protected virtual TextureData LoadCharacter(Character character)
	{
		var page = GetPageImage(character.Page);
		Debug.Assert(page != null);
		LoadedGlyphCount++;

		var source = page.Data;
		var target = new byte[character.Width * character.Height * 4];

		int readableHeight = Math.Min(character.Height, page.Height - character.Y);
		int readableWidth = Math.Min(character.Width, page.Width - character.X);

		int pageSize = page.Width * page.Height;

		//THIS CODE IS VERY WEIRD BECAUSE STBI LOADS IMAGES UPSIDE DOWN SO WE NEED TO ACCOUNT FOR THAT

		for (int y = 0; y < character.Height; y++)
		{
			int readOffset = (pageSize - ((character.Y + y) * page.Width) - page.Width + character.X) * 4;
			int targetOffset = y * character.Width * 4;

			for (int x = 0; x < character.Width * 4; x += 4)
			{
				var sourcePixel = readOffset + x;
				var targetPixel = targetOffset + x;

				if (x / 4 < readableWidth && y / 4 < readableHeight)
				{
					target[targetPixel] = source[sourcePixel];
					target[targetPixel + 1] = source[sourcePixel + 1];
					target[targetPixel + 2] = source[sourcePixel + 2];
					target[targetPixel + 3] = source[sourcePixel + 3];
				}
				else
				{
					target[targetPixel] = byte.MaxValue;
					target[targetPixel + 1] = byte.MaxValue;
					target[targetPixel + 2] = byte.MaxValue;
					target[targetPixel + 3] = byte.MinValue;
				}
			}
		}

		var result = new ImageResult()
		{
			Width = character.Width,
			Height = character.Height,
			Data = target,
			Comp = ColorComponents.RedGreenBlueAlpha,
			SourceComp = ColorComponents.RedGreenBlueAlpha
		};
		return new TextureData(result);
	}



	public Stream GetStream(string name) => throw new NotImplementedException();
	public IEnumerable<string> GetAvalibleResources() => Font?.Characters.Keys.Select(k => $"{FontName}/{(char)k}") ?? Enumerable.Empty<string>();

}
